using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Administrativo.API.Infrastructure;
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
    public class TipoAcomodacaoController : ControllerBase
    {

        private const string cachePrefix = "TIPOACOMODACAO#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<TipoAcomodacaoController> _localizer;

        public TipoAcomodacaoController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<TipoAcomodacaoController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um Tipo de Acomodação.
        /// </summary>
        /// <param name="tipoAcomodacaoToSave">
        /// Objeto que representa o Tipo de Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarTipoAcomodacao([FromBody]TipoAcomodacaoItem tipoAcomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeTipoAcomodacao(tipoAcomodacaoToSave.Nome_TipoAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            if (!ruleValidaCodExternoTipoAcomodacao(tipoAcomodacaoToSave.CodExterno_TipoAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<TipoAcomodacaoItem>().Any(e => e.Id_Empresa == tipoAcomodacaoToSave.Id_Empresa && e.Id_TipoAcomodacao == tipoAcomodacaoToSave.Id_TipoAcomodacao))
            {
                _administrativoContext.TipoAcomodacaoItems.Update(tipoAcomodacaoToSave);
            }
            else
            {
                _administrativoContext.TipoAcomodacaoItems.Add(tipoAcomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var tipoAcomodacaoSaveEvent = new TipoAcomodacaoSaveIE(tipoAcomodacaoToSave.Id_Empresa , tipoAcomodacaoToSave.Id_TipoAcomodacao, tipoAcomodacaoToSave.Nome_TipoAcomodacao, tipoAcomodacaoToSave.CodExterno_TipoAcomodacao, tipoAcomodacaoToSave.Id_CaracteristicaAcomodacao);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndTipoAcomodacaoContextChangesAsync(tipoAcomodacaoSaveEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeTipoAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(tipoAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarTipoAcomodacao), null);
        }

        /// <summary>
        /// Consulta o tipo de acomodação de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoAcomodacaoPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarTipoAcomodacaoPorIdEmpresaTO> l_ListTipoAcomodacaoTO = new List<ConsultarTipoAcomodacaoPorIdEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoAcomodacaoPorIdEmpresaTO> mycache = new Cache<ConsultarTipoAcomodacaoPorIdEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoAcomodacaoPorIdEmpresa_" + cachePrefix + idEmpresa.ToString());
                if (l_ListTipoAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoAcomodacaoPorIdEmpresaTO sqlClass = new ConsultarTipoAcomodacaoPorIdEmpresaTO();
                    sqlClass.ConsultarTipoAcomodacaoPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListTipoAcomodacaoTO);

                    if (l_ListTipoAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoAcomodacaoPorIdEmpresa_" + cachePrefix + idEmpresa.ToString(), l_ListTipoAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoAcomodacaoPorIdEmpresaTO sqlClass = new ConsultarTipoAcomodacaoPorIdEmpresaTO();
                sqlClass.ConsultarTipoAcomodacaoPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListTipoAcomodacaoTO);
            }


            return Ok(l_ListTipoAcomodacaoTO);

        }


        /// <summary>
        /// Consulta o tipo de acomodação de uma acomodacao.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="idAcomodacao">
        /// Identificador de Acomodacao.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/acomodacao/{idAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoAcomodacaoPorIdAcomodacao(int idEmpresa, int idAcomodacao)
        {
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> l_ListTipoAcomodacaoTO = new List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> mycache = new Cache<ConsultarTipoAcomodacaoPorIdAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoAcomodacaoPorIdAcomodacao_" + cachePrefix + idEmpresa.ToString());
                if (l_ListTipoAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoAcomodacaoPorIdAcomodacaoTO sqlClass = new ConsultarTipoAcomodacaoPorIdAcomodacaoTO();
                    sqlClass.ConsultarTipoAcomodacaoPorIdAcomodacaoTOCommand(idEmpresa, idAcomodacao,_settings.ConnectionString, ref l_ListTipoAcomodacaoTO);

                    if (l_ListTipoAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoAcomodacaoPorIdAcomodacao_" + cachePrefix + idEmpresa.ToString(), l_ListTipoAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoAcomodacaoPorIdAcomodacaoTO sqlClass = new ConsultarTipoAcomodacaoPorIdAcomodacaoTO();
                sqlClass.ConsultarTipoAcomodacaoPorIdAcomodacaoTOCommand(idEmpresa, idAcomodacao, _settings.ConnectionString, ref l_ListTipoAcomodacaoTO);
            }


            return Ok(l_ListTipoAcomodacaoTO);

        }

        /// <summary>
        /// Exclui um tipo de acomodação de uma empresa.
        /// </summary>
        /// <param name="id_Empresa">
        /// Identificador da empresa
        /// </param>
        /// <param name="id_TipoAcomodacao">
        /// Identificador do tipo de acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirTipoAcomodacao(int id_Empresa, int id_TipoAcomodacao)
        {

            if (id_Empresa < 1 || id_TipoAcomodacao<1)
            {
                return BadRequest();
            }

            var tipoAcomodacaoToDelete = _administrativoContext.TipoAcomodacaoItems
                .OfType<TipoAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Empresa == id_Empresa && c.Id_TipoAcomodacao == id_TipoAcomodacao);

            if (tipoAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.TipoAcomodacaoItems.Remove(tipoAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var TipoAcomodacaoExclusaoEvent = new TipoAcomodacaoExclusaoIE(tipoAcomodacaoToDelete.Id_Empresa, tipoAcomodacaoToDelete.Id_TipoAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndTipoAcomodacaoContextChangesAsync(TipoAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(TipoAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirTipoAcomodacao), null);
        }

        private bool ruleValidaNomeTipoAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_TIPOACOMODACAOEMPRESANOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["TipoAcomodacaoEmpresaNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaCodExternoTipoAcomodacao(string codExterno_TipoAcomodacao, ref string msgRetorno)
        {
            if (codExterno_TipoAcomodacao == null)
            {
                msgRetorno = _localizer["CodExternoRequerido"];
                return false;
            }

            if ((codExterno_TipoAcomodacao.Length > 2))
            {
                msgRetorno = _localizer["CodExternoTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaNomeTipoAcomodacao(string nome_TipoAcomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_TipoAcomodacao ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_TipoAcomodacao.Length < 3) || (nome_TipoAcomodacao.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }

    }


}