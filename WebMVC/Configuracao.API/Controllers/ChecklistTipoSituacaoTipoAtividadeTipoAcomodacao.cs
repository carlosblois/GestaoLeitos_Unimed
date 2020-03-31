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
using Configuracao.API.Infrastructure;
using Configuracao.API.IntegrationEvents;
using Configuracao.API.IntegrationEvents.Events;
using Configuracao.API.Model;
using Configuracao.API.TO;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;

namespace Configuracao.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoController : ControllerBase
    {

        private const string cachePrefix = "CHECKLISTTIPOSITUACAOTIPOATIVIDADETIPOACOMODACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoController> _localizer;

        public ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Consulta.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador de Empresa
        /// </param>
        [HttpGet]
        [Route("items/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(int IdEmpresa)
        {
            List<ConsultarCTSTATTO> l_ListTO = new List<ConsultarCTSTATTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarCTSTATTO> mycache = new Cache<ConsultarCTSTATTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao_" + cachePrefix + IdEmpresa.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarCTSTATTO sqlClass = new ConsultarCTSTATTO();
                    sqlClass.ConsultarCTSTATTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao_" + cachePrefix + IdEmpresa.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarCTSTATTO sqlClass = new ConsultarCTSTATTO();
                sqlClass.ConsultarCTSTATTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListTO);
            }

            return Ok(l_ListTO);

        }

        /// <summary>
        /// Inclui uma associação de Checklist e Tipo de Situacao / Atividade /Tipo de acomodacao.
        /// </summary>
        /// <param name="checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave">
        /// Objeto que representa a associação de checklist e tipo de situacao/ Atividade /Tipo de acomodacao
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirChecklistTipoSituacaoTipoAtividadeTipoAcomodacao([FromBody]ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave)
        {
            string msgRule = "";

            _configuracaoContext.ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItems.Add(checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave);

            //Create Integration Event to be published through the Event Bus
            var checklistTipoSituacaoTATAcomodacaoSaveEvent = new ChecklistTipoSituacaoTATAIncluirIE(checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_Checklist, 
                checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_TipoSituacaoAcomodacao,
                checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_TipoAtividadeAcomodacao, 
                checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_TipoAcomodacao ,
                checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_Empresa, 
                checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_CheckTSTAT);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(checklistTipoSituacaoTATAcomodacaoSaveEvent, checklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
                
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(checklistTipoSituacaoTATAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(IncluirChecklistTipoSituacaoTipoAtividadeTipoAcomodacao), null);
        }

        /// <summary>
        /// Exclui uma associação de checklist e tipo de situacao/ Atividade /Tipo de acomodacao
        /// </summary>
        /// <param name="Id_CheckTSTAT">
        /// Identificador da associação tipo de situacao/ Atividade /Tipo de acomodacao.
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(int Id_CheckTSTAT)
        {
            if (Id_CheckTSTAT < 1)
            {
                return BadRequest();
            }

            var checklistTipoSituacaoTATAcomodacaoToDelete = _configuracaoContext.ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItems
                .OfType<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_CheckTSTAT == Id_CheckTSTAT);
            
            if (checklistTipoSituacaoTATAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItems.Remove(checklistTipoSituacaoTATAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var ChecklistTipoSituacaoTATAcomodacaoExclusaoEvent = new ChecklistTipoSituacaoTATAExcluirIE(checklistTipoSituacaoTATAcomodacaoToDelete.Id_CheckTSTAT);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(ChecklistTipoSituacaoTATAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(ChecklistTipoSituacaoTATAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirChecklistTipoSituacaoTipoAtividadeTipoAcomodacao), null);
        }

    }
}