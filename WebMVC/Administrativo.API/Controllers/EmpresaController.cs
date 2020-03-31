using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Administrativo.API.Infrastructure;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Administrativo.API.IntegrationEvents;
using Administrativo.API.IntegrationEvents.Events;
using Administrativo.API.Model;
using Microsoft.Extensions.Localization;
using Administrativo.API.TO;
using Microsoft.AspNetCore.Authorization;

namespace Administrativo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private const string cachePrefix = "EMPRESA#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<EmpresaController> _localizer;

        public EmpresaController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<EmpresaController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza uma empresa.
        /// </summary>
        /// <param name="empresaToSave">
        /// Objeto que representa a Empresa
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarEmpresa([FromBody]EmpresaItem empresaToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeEmpresa(empresaToSave.Nome_Empresa, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            if (!ruleValidaCodExternoEmpresa(empresaToSave.CodExterno_Empresa, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<EmpresaItem>().Any(e => e.id_Empresa == empresaToSave.id_Empresa))
            {
                _administrativoContext.EmpresaItems.Update(empresaToSave);
            }
            else
            {
                _administrativoContext.EmpresaItems.Add(empresaToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var empresaSaveEvent = new EmpresaSaveIE(           
                empresaToSave.Nome_Empresa, empresaToSave.CodExterno_Empresa, empresaToSave.Endereco_Empresa,
                empresaToSave.Complemento_Empresa, empresaToSave.Numero_Empresa, empresaToSave.Bairro_Empresa,
                empresaToSave.Cidade_Empresa, empresaToSave.Estado_Empresa, empresaToSave.Cep_Empresa,
                empresaToSave.Fone_Empresa, empresaToSave.Contato_Empresa, empresaToSave.CGC_Empresa,
                empresaToSave.InscricaoMunicipal_Empresa, empresaToSave.InscricaoEstadual_Empresa, empresaToSave.CNES_Empresa);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndEmpresaContextChangesAsync(empresaSaveEvent, empresaToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeEmpresaUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(empresaSaveEvent);


            return CreatedAtAction(nameof(SalvarEmpresa), empresaToSave.id_Empresa);
        }

        /// <summary>
        /// Exclui uma empresa.
        /// </summary>
        /// <param name="id_Empresa">
        /// Identificador da empresa
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirEmpresa(int id_Empresa )
        {

            if (id_Empresa < 1)
            {
                return BadRequest();
            }

            var empresaToDelete = _administrativoContext.EmpresaItems
                .OfType<EmpresaItem>()
                .SingleOrDefault(c => c.id_Empresa == id_Empresa);

            if (empresaToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.EmpresaItems.Remove(empresaToDelete);

            //Create Integration Event to be published through the Event Bus
            var EmpresaExclusaoEvent = new EmpresaExclusaoIE(empresaToDelete.id_Empresa);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndEmpresaContextChangesAsync(EmpresaExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(EmpresaExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirEmpresa), null);
        }

        /// <summary>
        /// Consulta uma empresa.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador de empresa
        /// </param>
        [HttpGet]
        [Route("items/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<EmpresaItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarEmpresaPorIdEmpresa(int IdEmpresa)
        {
            List<ConsultarEmpresaPorIdEmpresaTO> l_ListEmpresaTO = new List<ConsultarEmpresaPorIdEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarEmpresaPorIdEmpresaTO> mycache = new Cache<ConsultarEmpresaPorIdEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListEmpresaTO = await mycache.GetListAsync("ConsultarEmpresaPorIdEmpresa_" + cachePrefix + IdEmpresa.ToString());
                if (l_ListEmpresaTO.Count == 0)
                {
                    ConsultarEmpresaPorIdEmpresaTO sqlClass = new ConsultarEmpresaPorIdEmpresaTO();
                    sqlClass.ConsultarEmpresaPorIdEmpresaTOCommand(IdEmpresa,_settings.ConnectionString, ref l_ListEmpresaTO);
                    
                    if (l_ListEmpresaTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarEmpresaPorIdEmpresa_" + cachePrefix + IdEmpresa.ToString(), l_ListEmpresaTO);
                    }
                }
            }
            else
            {
                ConsultarEmpresaPorIdEmpresaTO sqlClass = new ConsultarEmpresaPorIdEmpresaTO();
                sqlClass.ConsultarEmpresaPorIdEmpresaTOCommand(IdEmpresa,_settings.ConnectionString, ref l_ListEmpresaTO);
            }


            return Ok(l_ListEmpresaTO);

        }

        /// <summary>
        /// Consulta todas as empresa.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<EmpresaItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarEmpresa()
        {
            List<ConsultarEmpresaTO> l_ListEmpresaTO = new List<ConsultarEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarEmpresaTO> mycache = new Cache<ConsultarEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListEmpresaTO = await mycache.GetListAsync("ConsultarEmpresa_" + cachePrefix );
                if (l_ListEmpresaTO.Count == 0)
                {
                    ConsultarEmpresaTO sqlClass = new ConsultarEmpresaTO();
                    sqlClass.ConsultarEmpresaTOCommand( _settings.ConnectionString, ref l_ListEmpresaTO);

                    if (l_ListEmpresaTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarEmpresa_" + cachePrefix, l_ListEmpresaTO);
                    }
                }
            }
            else
            {
                ConsultarEmpresaTO sqlClass = new ConsultarEmpresaTO();
                sqlClass.ConsultarEmpresaTOCommand( _settings.ConnectionString, ref l_ListEmpresaTO);
            }


            return Ok(l_ListEmpresaTO);

        }
        private bool ruleValidaNomeEmpresaUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_EMPRESANOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["EmpresaNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaCodExternoEmpresa(string codExterno_Empresa, ref string msgRetorno)
        {
            if (codExterno_Empresa == null)
            {
                msgRetorno = _localizer["CodExternoRequerido"];
                return false;
            }

            if ((codExterno_Empresa.Length > 6))
            {
                msgRetorno = _localizer["CodExternoTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaNomeEmpresa(string nome_Empresa, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Empresa ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Empresa.Length < 3) || (nome_Empresa.Length > 60))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }
    }
}