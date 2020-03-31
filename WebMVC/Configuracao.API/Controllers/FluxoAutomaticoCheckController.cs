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
    public class FluxoAutomaticoCheckController : ControllerBase
    {

        private const string cachePrefix = "FLUXOAUTOMATICOCHECK#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<FluxoAutomaticoCheckController> _localizer;

        public FluxoAutomaticoCheckController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<FluxoAutomaticoCheckController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Consulta os fluxos automaticos de checklist.
        /// </summary>
        [HttpGet]
        [Route("items/fluxoautomaticocheck/")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoCheckTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomatico()
        {
            List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoCheckTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoCheckTO> mycache = new Cache<ConsultarFluxoAutomaticoCheckTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomatico_" + cachePrefix);
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                    sqlClass.ConsultarFluxoAutomaticoCheckTOCommand( _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomatico_" + cachePrefix , l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                sqlClass.ConsultarFluxoAutomaticoCheckTOCommand( _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Consulta os fluxos automaticos de checklist por situacao.
        /// </summary>
        /// <param name="IdTipoSituacaoAcomodacao">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomaticocheck/tiposituacao/{IdTipoSituacaoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoCheckTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorTipoSituacao(int IdTipoSituacaoAcomodacao)
        {
            List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoCheckTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoCheckTO> mycache = new Cache<ConsultarFluxoAutomaticoCheckTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorTipoSituacao_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacao.ToString() );
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                    sqlClass.ConsultarFluxoAutomaticoCheckPorSituacaoTOCommand(IdTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorTipoSituacao_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacao.ToString() , l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                sqlClass.ConsultarFluxoAutomaticoCheckPorSituacaoTOCommand(IdTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Consulta os fluxos automaticos de checklist por situacao.
        /// </summary>
        /// <param name="IdTipoSituacaoAcomodacao">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="IdCheckList">
        /// Identificador do Checklist
        /// </param>
        [HttpGet]
        [Route("items/fluxoautomaticocheck/tiposituacao/{IdTipoSituacaoAcomodacao}/checklist/{IdCheckList}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarFluxoAutomaticoCheckTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarFluxoAutomaticoPorTipoSituacaoPorIdChecklist(int IdTipoSituacaoAcomodacao, int IdCheckList)
        {
            List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO = new List<ConsultarFluxoAutomaticoCheckTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarFluxoAutomaticoCheckTO> mycache = new Cache<ConsultarFluxoAutomaticoCheckTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListFluxoTO = await mycache.GetListAsync("ConsultarFluxoAutomaticoPorTipoSituacaoPorIdChecklist_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacao.ToString() + "@" +
                                                            IdCheckList.ToString());
                if (l_ListFluxoTO.Count == 0)
                {
                    ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                    sqlClass.ConsultarFluxoAutomaticoCheckPorSituacaoIdChecklistTOCommand(IdTipoSituacaoAcomodacao, IdCheckList, _settings.ConnectionString, ref l_ListFluxoTO);

                    if (l_ListFluxoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarFluxoAutomaticoPorTipoSituacaoPorIdChecklist_" + cachePrefix +
                                                            IdTipoSituacaoAcomodacao.ToString() + "@" +
                                                            IdCheckList.ToString(), l_ListFluxoTO);
                    }
                }
            }
            else
            {
                ConsultarFluxoAutomaticoCheckTO sqlClass = new ConsultarFluxoAutomaticoCheckTO();
                sqlClass.ConsultarFluxoAutomaticoCheckPorSituacaoIdChecklistTOCommand(IdTipoSituacaoAcomodacao, IdCheckList, _settings.ConnectionString, ref l_ListFluxoTO);
            }


            return Ok(l_ListFluxoTO);

        }

        /// <summary>
        /// Inclui uma associação de Item de checklist com  TipoSituacao / Tipo Atividade destino.
        /// </summary>
        /// <param name="FluxoAutomaticoCheckToSave">
        /// Objeto que representa o Fluxo automatico de check
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirFluxoAutomaticoCheck([FromBody]FluxoAutomaticoCheckItem FluxoAutomaticoCheckToSave)
        {
            string msgRule = "";

            _configuracaoContext.FluxoAutomaticoCheckItems.Add(FluxoAutomaticoCheckToSave);

            //Create Integration Event to be published through the Event Bus
            var fluxoAutomaticoCheckSaveEvent = new FluxoAutomaticoCheckIncluirIE(
                FluxoAutomaticoCheckToSave.Id_Checklist,
                FluxoAutomaticoCheckToSave.Id_TipoSituacaoAcomodacao,
                FluxoAutomaticoCheckToSave.Id_ItemChecklist,
                FluxoAutomaticoCheckToSave.Id_TipoAtividadeAcomodacao, 
                FluxoAutomaticoCheckToSave.Cod_Resposta,
                FluxoAutomaticoCheckToSave.Cod_PermiteTotal);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndFluxoAutomaticoCheckContextChangesAsync(fluxoAutomaticoCheckSaveEvent);
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
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(fluxoAutomaticoCheckSaveEvent);


            return CreatedAtAction(nameof(IncluirFluxoAutomaticoCheck), null);
        }

        /// <summary>
        /// Exclui uma associação de item de checklist com Tipo de Situacao / Tipo Atividade.
        /// </summary>
        /// <param name="Id_TipoSituacaoAcomodacao">
        /// Identificador do Tipo de Situação de Acomodação
        /// </param>
        /// <param name="Id_TipoAtividadeAcomodacao">
        /// Identificador do Tipo de Atividade de Acomodação
        /// </param>
        /// <param name="Id_Checklist">
        /// Identificador do Checklist
        /// </param>
        /// <param name="Id_ItemChecklist">
        /// Identificador do item de checklist
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirFluxoAutomaticoCheck(int Id_Checklist, int Id_TipoSituacaoAcomodacao, int Id_ItemChecklist, int Id_TipoAtividadeAcomodacao)
        {

            if (Id_Checklist < 1 || Id_TipoSituacaoAcomodacao < 1 || Id_ItemChecklist < 1 || Id_TipoAtividadeAcomodacao < 1)
            {
                return BadRequest();
            }

            var fluxoAutomaticoCheckToDelete = _configuracaoContext.FluxoAutomaticoCheckItems
                .OfType<FluxoAutomaticoCheckItem>()
                .SingleOrDefault(c => c.Id_Checklist == Id_Checklist 
                                && c.Id_TipoSituacaoAcomodacao == Id_TipoSituacaoAcomodacao 
                                && c.Id_TipoAtividadeAcomodacao == Id_TipoAtividadeAcomodacao 
                                && c.Id_ItemChecklist == Id_ItemChecklist);

            if (fluxoAutomaticoCheckToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.FluxoAutomaticoCheckItems.Remove(fluxoAutomaticoCheckToDelete);

            //Create Integration Event to be published through the Event Bus
            var FluxoAutomaticoCheckExclusaoEvent = new FluxoAutomaticoCheckExcluirIE(
                fluxoAutomaticoCheckToDelete.Id_Checklist,
                fluxoAutomaticoCheckToDelete.Id_TipoSituacaoAcomodacao,
                fluxoAutomaticoCheckToDelete.Id_ItemChecklist,
                fluxoAutomaticoCheckToDelete.Id_TipoAtividadeAcomodacao);


            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndFluxoAutomaticoCheckContextChangesAsync(FluxoAutomaticoCheckExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(FluxoAutomaticoCheckExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirFluxoAutomaticoCheck), null);
        }

        private bool ruleValidaFluxoAutomaticoPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_FLUXOAUTOMATICOCHECK"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["FluxoAutomaticoCheckPK"];
                return true;
            }

            return false;

        }

    }
}