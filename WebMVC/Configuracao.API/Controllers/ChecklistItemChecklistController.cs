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
    public class ChecklistItemChecklistController : ControllerBase
    {

        private const string cachePrefix = "CHECKLISTITEMCHECKLIST#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<ChecklistItemChecklistController> _localizer;

        public ChecklistItemChecklistController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<ChecklistItemChecklistController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }
        /// <summary>
        /// Inclui uma associação de Checklist e Item de checklist.
        /// </summary>
        /// <param name="checklistItemChecklistToSave">
        /// Objeto que representa a associação de checklist e item de checklist
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirChecklistItemChecklist([FromBody]ChecklistItemChecklistItem checklistItemChecklistToSave)
        {
            string msgRule = "";

            _configuracaoContext.ChecklistItemChecklistItems.Add(checklistItemChecklistToSave);

            //Create Integration Event to be published through the Event Bus
            var checklistItemChecklistSaveEvent = new ChecklistItemChecklistIncluirIE(checklistItemChecklistToSave.Id_Checklist, checklistItemChecklistToSave.Id_ItemChecklist);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndChecklistItemChecklistContextChangesAsync(checklistItemChecklistSaveEvent);
            }
            catch (Exception e)
            {

                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaChecklistItemChecklistPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(checklistItemChecklistSaveEvent);


            return CreatedAtAction(nameof(IncluirChecklistItemChecklist), null);
        }

        /// <summary>
        /// Exclui uma associação de checklist e item de checklist.
        /// </summary>
        /// <param name="Id_Checklist">
        /// Identificador do checklist.
        /// </param>
        /// <param name="Id_ItemChecklist">
        /// Identificador do item de checklist
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirChecklistItemChecklist(int Id_Checklist, int Id_ItemChecklist)
        {

            if (Id_Checklist < 1 || Id_ItemChecklist < 1)
            {
                return BadRequest();
            }

            var checklistItemChecklistToDelete = _configuracaoContext.ChecklistItemChecklistItems
                .OfType<ChecklistItemChecklistItem>()
                .SingleOrDefault(c => c.Id_Checklist == Id_Checklist && c.Id_ItemChecklist == Id_ItemChecklist);

            if (checklistItemChecklistToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.ChecklistItemChecklistItems.Remove(checklistItemChecklistToDelete);

            //Create Integration Event to be published through the Event Bus
            var ChecklistItemChecklistExclusaoEvent = new ChecklistItemChecklistExclusaoIE(checklistItemChecklistToDelete.Id_Checklist, checklistItemChecklistToDelete.Id_ItemChecklist);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndChecklistItemChecklistContextChangesAsync(ChecklistItemChecklistExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(ChecklistItemChecklistExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirChecklistItemChecklist), null);
        }

        private bool ruleValidaChecklistItemChecklistPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_CHECKLIST_ITENSCHECKLIST"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["ChecklistItensChecklistPK"];
                return true;
            }

            return false;

        }
    }
}