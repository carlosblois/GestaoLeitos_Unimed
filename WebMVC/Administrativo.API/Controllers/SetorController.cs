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
    public class SetorController : ControllerBase
    {

        private const string cachePrefix = "SETOR#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<SetorController> _localizer;

        public SetorController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<SetorController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um Setor.
        /// </summary>
        /// <param name="setorToSave">
        /// Objeto que representa o Setor
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarSetor([FromBody]SetorItem setorToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeSetor(setorToSave.nome_Setor, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            if (!ruleValidaCodExternoSetor(setorToSave.CodExterno_Setor, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<SetorItem>().Any(e => e.id_Empresa == setorToSave.id_Empresa && e.id_Setor == setorToSave.id_Setor))
            {
                _administrativoContext.SetorItems.Update(setorToSave);
            }
            else
            {
                _administrativoContext.SetorItems.Add(setorToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var setorSaveEvent = new SetorSaveIE(setorToSave.id_Empresa, setorToSave.nome_Setor, setorToSave.CodExterno_Setor);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndSetorContextChangesAsync(setorSaveEvent, setorToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeSetorUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(setorSaveEvent);


            return CreatedAtAction(nameof(SalvarSetor), setorToSave.id_Setor);
        }

        /// <summary>
        /// Exclui um setor de uma empresa.
        /// </summary>
        /// <param name="id_Empresa">
        /// Identificador da empresa
        /// </param>
        /// <param name="id_Setor">
        /// Identificador do Setor
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirSetor(int id_Empresa, int id_Setor)
        {

            if (id_Empresa < 1 || id_Setor < 1)
            {
                return BadRequest();
            }

            var setorToDelete = _administrativoContext.SetorItems
                .OfType<SetorItem>()
                .SingleOrDefault(c => c.id_Empresa == id_Empresa && c.id_Setor == id_Setor);

            if (setorToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.SetorItems.Remove(setorToDelete);

            //Create Integration Event to be published through the Event Bus
            var setorExclusaoEvent = new SetorExclusaoIE(setorToDelete.id_Empresa, setorToDelete.id_Setor);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndSetorContextChangesAsync(setorExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(setorExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirSetor), null);
        }

        /// <summary>
        /// Consulta o setor de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SetorItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSetorPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarSetorPorIdEmpresaTO> l_ListSetorTO = new List<ConsultarSetorPorIdEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSetorPorIdEmpresaTO> mycache = new Cache<ConsultarSetorPorIdEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSetorTO = await mycache.GetListAsync("ConsultarSetorPorIdEmpresa_" + cachePrefix + idEmpresa.ToString());
                if (l_ListSetorTO.Count == 0)
                {
                    ConsultarSetorPorIdEmpresaTO sqlClass = new ConsultarSetorPorIdEmpresaTO();
                    sqlClass.ConsultarSetorPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListSetorTO);

                    if (l_ListSetorTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSetorPorIdEmpresa_" + cachePrefix + idEmpresa.ToString(), l_ListSetorTO);
                    }
                }
            }
            else
            {
                ConsultarSetorPorIdEmpresaTO sqlClass = new ConsultarSetorPorIdEmpresaTO();
                sqlClass.ConsultarSetorPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListSetorTO);
            }


            return Ok(l_ListSetorTO);
        }

        private bool ruleValidaNomeSetorUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_SETOREMPRESANOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["SetorEmpresaNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaCodExternoSetor(string codExterno_Setor, ref string msgRetorno)
        {
            if (codExterno_Setor == null)
            {
                msgRetorno = _localizer["CodExternoRequerido"];
                return false;
            }

            if ((codExterno_Setor.Length > 4))
            {
                msgRetorno = _localizer["CodExternoTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaNomeSetor(string nome_Setor, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Setor))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Setor.Length < 3) || (nome_Setor.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }

    }


}