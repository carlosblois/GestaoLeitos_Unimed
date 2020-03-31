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
    public class FluxoAutomaticoAcaoController : ControllerBase
    {

        private const string cachePrefix = "FLUXOAUTOMATICOACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<FluxoAutomaticoAcaoController> _localizer;

        public FluxoAutomaticoAcaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<FluxoAutomaticoAcaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Consulta os fluxos automaticos de uma empresa.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa 
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomatico/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorEmpresa(int IdEmpresa)
        {
            List<ConsultarFluxoAutomaticoTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoTO> mycache = new Cache<ConsultarFluxoAutomaticoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorEmpresa_" + cachePrefix +
                                                            IdEmpresa.ToString());
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoTO sqlClass = new ConsultarFluxoAutomaticoTO();
                    sqlClass.ConsultarFluxoAutomaticoPorEmpresaTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorEmpresa_" + cachePrefix +
                                                            IdEmpresa.ToString(), l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoTO sqlClass = new ConsultarFluxoAutomaticoTO();
                sqlClass.ConsultarFluxoAutomaticoPorEmpresaTOCommand(  IdEmpresa, _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }


        /// <summary>
        /// Consulta os fluxos automaticos de uma acao / atividade / situacao. Especificas com destino Situacao / Tipo Atividade
        /// </summary>
        /// <param name="IdTipoSituacaoAcomodacaoOrigem">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="IdTipoAtividadeAcomodacaoOrigem">
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </param>
        /// <param name="IdTipoAcaoAcomodacaoOrigem">
        /// Identificador do Tipo de Ação de Acomodação
        /// </param>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa 
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomatico/tiposituacao/{IdTipoSituacaoAcomodacaoOrigem}/tipoatividade/{IdTipoAtividadeAcomodacaoOrigem}/tipoacao/{IdTipoAcaoAcomodacaoOrigem}/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorAcaoPorTipoAtividadePorTipoSituacaoPorEmpresa(int IdTipoSituacaoAcomodacaoOrigem,
                                                            int IdTipoAtividadeAcomodacaoOrigem,
                                                            int IdTipoAcaoAcomodacaoOrigem,
                                                            int IdEmpresa)
        {
            List<ConsultarFluxoAutomaticoTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoTO> mycache = new Cache<ConsultarFluxoAutomaticoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorAcaoPorTipoAtividadePorTipoSituacao_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdTipoAtividadeAcomodacaoOrigem.ToString() + "@" +
                                                            IdTipoAcaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdEmpresa.ToString() );
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoTO sqlClass = new ConsultarFluxoAutomaticoTO();
                    sqlClass.ConsultarFluxoAutomaticoTOCommand(IdTipoSituacaoAcomodacaoOrigem, IdTipoAtividadeAcomodacaoOrigem, IdTipoAcaoAcomodacaoOrigem,IdEmpresa,_settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorAcaoPorTipoAtividadePorTipoSituacao_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdTipoAtividadeAcomodacaoOrigem.ToString() + "@" +
                                                            IdTipoAcaoAcomodacaoOrigem.ToString() + "@" +
                                                            IdEmpresa.ToString(), l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoTO sqlClass = new ConsultarFluxoAutomaticoTO();
                sqlClass.ConsultarFluxoAutomaticoTOCommand(IdTipoSituacaoAcomodacaoOrigem, IdTipoAtividadeAcomodacaoOrigem, IdTipoAcaoAcomodacaoOrigem,IdEmpresa,_settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Inclui uma associação de Tipo de Situacao / Tipo Atividade / Ação origem para Tipo de Situacao / Tipo Atividade destino.
        /// </summary>
        /// <param name="FluxoAutomaticoAcaoToSave">
        /// Objeto que representa a Origem e destino do Fluxo automatico de ação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirFluxoAutomaticoAcao([FromBody]FluxoAutomaticoAcaoItem FluxoAutomaticoAcaoToSave)
        {
            string msgRule = "";

            _configuracaoContext.FluxoAutomaticoAcaoItems.Add(FluxoAutomaticoAcaoToSave);

            //Create Integration Event to be published through the Event Bus
            var fluxoAutomaticoAcaoSaveEvent = new FluxoAutomaticoAcaoIncluirIE(FluxoAutomaticoAcaoToSave.Id_TipoSituacaoAcomodacaoOrigem, FluxoAutomaticoAcaoToSave.Id_TipoAtividadeAcomodacaoOrigem, FluxoAutomaticoAcaoToSave.Id_TipoAcaoAcomodacaoOrigem, FluxoAutomaticoAcaoToSave.Id_TipoSituacaoAcomodacaoDestino, FluxoAutomaticoAcaoToSave.Id_TipoAtividadeAcomodacaoDestino, FluxoAutomaticoAcaoToSave.Id_Empresa);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndFluxoAutomaticoAcaoContextChangesAsync(fluxoAutomaticoAcaoSaveEvent);
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
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(fluxoAutomaticoAcaoSaveEvent);


            return CreatedAtAction(nameof(IncluirFluxoAutomaticoAcao), null);
        }

        /// <summary>
        /// Exclui uma associação de Tipo de Situacao / Tipo Atividade / Ação origem para Tipo de Situacao / Tipo Atividade destino.
        /// </summary>
        /// <param name="Id_TipoSituacaoAcomodacaoOrigem">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="Id_TipoAtividadeAcomodacaoOrigem">
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </param>
        /// <param name="Id_TipoAcaoAcomodacaoOrigem">
        /// Identificador do Tipo de Ação de Acomodação
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
        public async Task<IActionResult> ExcluirFluxoAutomaticoAcao(int Id_TipoSituacaoAcomodacaoOrigem,int Id_TipoAtividadeAcomodacaoOrigem, int Id_TipoAcaoAcomodacaoOrigem, int Id_TipoSituacaoAcomodacaoDestino, int Id_TipoAtividadeAcomodacaoDestino,int IdEmpresa)
        {

            if (Id_TipoSituacaoAcomodacaoOrigem < 1 || Id_TipoAtividadeAcomodacaoOrigem < 1 || Id_TipoAcaoAcomodacaoOrigem < 1 || Id_TipoSituacaoAcomodacaoDestino < 1 || Id_TipoAtividadeAcomodacaoDestino < 1 || IdEmpresa < 1)
            {
                return BadRequest();
            }

            var fluxoAutomaticoAcaoToDelete = _configuracaoContext.FluxoAutomaticoAcaoItems
                .OfType<FluxoAutomaticoAcaoItem>()
                .SingleOrDefault(c => c.Id_TipoSituacaoAcomodacaoOrigem == Id_TipoSituacaoAcomodacaoOrigem && c.Id_TipoAtividadeAcomodacaoOrigem == Id_TipoAtividadeAcomodacaoOrigem && c.Id_TipoAcaoAcomodacaoOrigem == Id_TipoAcaoAcomodacaoOrigem && c.Id_TipoSituacaoAcomodacaoDestino == Id_TipoSituacaoAcomodacaoDestino && c.Id_TipoAtividadeAcomodacaoDestino == Id_TipoAtividadeAcomodacaoDestino && c.Id_Empresa == IdEmpresa);

            if (fluxoAutomaticoAcaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.FluxoAutomaticoAcaoItems.Remove(fluxoAutomaticoAcaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var FluxoAutomaticoAcaoExclusaoEvent = new FluxoAutomaticoAcaoExcluirIE(fluxoAutomaticoAcaoToDelete.Id_TipoSituacaoAcomodacaoOrigem, fluxoAutomaticoAcaoToDelete.Id_TipoAtividadeAcomodacaoOrigem, fluxoAutomaticoAcaoToDelete.Id_TipoAcaoAcomodacaoOrigem, fluxoAutomaticoAcaoToDelete.Id_TipoSituacaoAcomodacaoDestino, fluxoAutomaticoAcaoToDelete.Id_TipoAtividadeAcomodacaoDestino, fluxoAutomaticoAcaoToDelete.Id_Empresa);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndFluxoAutomaticoAcaoContextChangesAsync(FluxoAutomaticoAcaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(FluxoAutomaticoAcaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirFluxoAutomaticoAcao), null);
        }

        private bool ruleValidaFluxoAutomaticoPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_FLUXOAUTOMATICOTIPOSITUACAO_TIPOATIVIDADE"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["FluxoAutomaticoTipoSituacao_TipoAtividadePK"];
                return true;
            }

            return false;

        }

    }
}