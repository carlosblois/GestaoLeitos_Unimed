using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Configuracao.API.Infrastructure;
using Configuracao.API.IntegrationEvents;
using Configuracao.API.IntegrationEvents.Events;
using Configuracao.API.Model;
using Configuracao.API.TO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Configuracao.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FluxoAutomaticoSituacaoController : ControllerBase
    {

        private const string cachePrefix = "FLUXOAUTOMATICOSITUACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<FluxoAutomaticoSituacaoController> _localizer;

        public FluxoAutomaticoSituacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<FluxoAutomaticoSituacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Consulta os fluxos automaticos de situacao de uma empresa.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa 
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomaticosituacao/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoSitTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorEmpresa(int IdEmpresa)
        {
            List<ConsultarFluxoAutomaticoSitTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoSitTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoSitTO> mycache = new Cache<ConsultarFluxoAutomaticoSitTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorEmpresa_" + cachePrefix +
                                                            IdEmpresa.ToString());
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoSitTO sqlClass = new ConsultarFluxoAutomaticoSitTO();
                    sqlClass.ConsultarFluxoAutomaticoSitPorEmpresaTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorEmpresa_" + cachePrefix +
                                                            IdEmpresa.ToString(), l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoSitTO sqlClass = new ConsultarFluxoAutomaticoSitTO();
                sqlClass.ConsultarFluxoAutomaticoSitPorEmpresaTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Consulta os fluxos automaticos de situacao por situacao e empresa.
        /// </summary>
        /// <param name="IdTipoSituacaoAcomodacaoOrigem">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa 
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomaticosituacao/tiposituacao/{IdTipoSituacaoAcomodacaoOrigem}/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoSitTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorTipoSituacaoPorEmpresa(int IdTipoSituacaoAcomodacaoOrigem,int IdEmpresa)
        {
            List<ConsultarFluxoAutomaticoSitTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoSitTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoSitTO> mycache = new Cache<ConsultarFluxoAutomaticoSitTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorTipoSituacaoPorEmpresa_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdEmpresa.ToString());
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoSitTO sqlClass = new ConsultarFluxoAutomaticoSitTO();
                    sqlClass.ConsultarFluxoAutomaticoSitTOCommand(IdTipoSituacaoAcomodacaoOrigem,   IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorTipoSituacaoPorEmpresa_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdEmpresa.ToString(), l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoSitTO sqlClass = new ConsultarFluxoAutomaticoSitTO();
                sqlClass.ConsultarFluxoAutomaticoSitTOCommand(IdTipoSituacaoAcomodacaoOrigem,   IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Inclui uma associação de Tipo de Situacao origem para Tipo de Situacao / Tipo Atividade destino.
        /// </summary>
        /// <param name="FluxoAutomaticoSituacaoToSave">
        /// Objeto que representa a Origem e destino do Fluxo automatico de situacao
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirFluxoAutomaticoSituacao([FromBody]FluxoAutomaticoSituacaoItem FluxoAutomaticoSituacaoToSave)
        {
            string msgRule = "";

            _configuracaoContext.FluxoAutomaticoSituacaoItems.Add(FluxoAutomaticoSituacaoToSave);

            //Create Integration Event to be published through the Event Bus
            var fluxoAutomaticoSituacaoSaveEvent = new FluxoAutomaticoSituacaoIncluirIE(FluxoAutomaticoSituacaoToSave.Id_TipoSituacaoAcomodacaoOrigem,  FluxoAutomaticoSituacaoToSave.Id_TipoSituacaoAcomodacaoDestino, FluxoAutomaticoSituacaoToSave.Id_TipoAtividadeAcomodacaoDestino, FluxoAutomaticoSituacaoToSave.Id_Empresa);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndFluxoAutomaticoSituacaoContextChangesAsync(fluxoAutomaticoSituacaoSaveEvent);
            }
            catch (Exception e)
            {

                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaFluxoAutomaticoPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(fluxoAutomaticoSituacaoSaveEvent);


            return CreatedAtAction(nameof(IncluirFluxoAutomaticoSituacao), null);
        }

        /// <summary>
        /// Exclui uma associação de Tipo de Situacao / Tipo Atividade / Ação origem para Tipo de Situacao / Tipo Atividade destino.
        /// </summary>
        /// <param name="Id_TipoSituacaoAcomodacaoOrigem">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="Id_TipoSituacaoAcomodacaoDestino">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="Id_TipoAtividadeAcomodacaoDestino">
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </param>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa 
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirFluxoAutomaticoSituacao(int Id_TipoSituacaoAcomodacaoOrigem, int Id_TipoSituacaoAcomodacaoDestino, int Id_TipoAtividadeAcomodacaoDestino, int IdEmpresa)
        {

            if (Id_TipoSituacaoAcomodacaoOrigem < 1 || Id_TipoSituacaoAcomodacaoDestino < 1 || Id_TipoAtividadeAcomodacaoDestino < 1 || IdEmpresa < 1)
            {
                return BadRequest();
            }

            var fluxoAutomaticoAcaoToDelete = _configuracaoContext.FluxoAutomaticoSituacaoItems
                .OfType<FluxoAutomaticoSituacaoItem>()
                .SingleOrDefault(c => c.Id_TipoSituacaoAcomodacaoOrigem == Id_TipoSituacaoAcomodacaoOrigem  && c.Id_TipoSituacaoAcomodacaoDestino == Id_TipoSituacaoAcomodacaoDestino && c.Id_TipoAtividadeAcomodacaoDestino == Id_TipoAtividadeAcomodacaoDestino && c.Id_Empresa == IdEmpresa);

            if (fluxoAutomaticoAcaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.FluxoAutomaticoSituacaoItems.Remove(fluxoAutomaticoAcaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var FluxoAutomaticoSituacaoExclusaoEvent = new FluxoAutomaticoSituacaoExcluirIE(fluxoAutomaticoAcaoToDelete.Id_TipoSituacaoAcomodacaoOrigem,   fluxoAutomaticoAcaoToDelete.Id_TipoSituacaoAcomodacaoDestino, fluxoAutomaticoAcaoToDelete.Id_TipoAtividadeAcomodacaoDestino, fluxoAutomaticoAcaoToDelete.Id_Empresa);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndFluxoAutomaticoSituacaoContextChangesAsync(FluxoAutomaticoSituacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(FluxoAutomaticoSituacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirFluxoAutomaticoSituacao), null);
        }

        private bool ruleValidaFluxoAutomaticoPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_FLUXOAUTOMATICOTIPOSITUACAOTRANSICAO"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["FluxoAutomaticoTipoSituacaoTransicaoPK"];
                return true;
            }

            return false;

        }

    }
}