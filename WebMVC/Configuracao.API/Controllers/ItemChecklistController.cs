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
    public class ItemChecklistController : ControllerBase
    {

        private const string cachePrefix = "ITEMCHECKLIST#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<ItemChecklistController> _localizer;

        public ItemChecklistController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<ItemChecklistController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um item de checklist.
        /// </summary>
        /// <param name="itemChecklistToSave">
        /// Objeto que representa o item de checklist
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarItemChecklist([FromBody]ItemChecklistItem itemChecklistToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeItemCheckList(itemChecklistToSave.Nome_ItemChecklist, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_configuracaoContext.Set<ItemChecklistItem>().Any(e => e.Id_ItemChecklist == itemChecklistToSave.Id_ItemChecklist))
            {
                _configuracaoContext.ItemChecklistItems.Update(itemChecklistToSave);
            }
            else
            {
                _configuracaoContext.ItemChecklistItems.Add(itemChecklistToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var itemChecklistSaveEvent = new ItemChecklistSaveIE(itemChecklistToSave.Id_ItemChecklist, itemChecklistToSave.Nome_ItemChecklist);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndItemChecklistContextChangesAsync(itemChecklistSaveEvent, itemChecklistToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeItemChecklistUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(itemChecklistSaveEvent);


            return CreatedAtAction(nameof(SalvarItemChecklist), itemChecklistToSave.Id_ItemChecklist);
        }

        /// <summary>
        /// Exclui um item de checklist.
        /// </summary>
        /// <param name="id_ItemChecklist">
        /// Identificador do item de checklist
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirItemChecklist(int id_ItemChecklist)
        {

            if (id_ItemChecklist < 1)
            {
                return BadRequest();
            }

            var itemChecklistToDelete = _configuracaoContext.ItemChecklistItems
                .OfType<ItemChecklistItem>()
                .SingleOrDefault(c => c.Id_ItemChecklist == id_ItemChecklist);

            if (itemChecklistToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.ItemChecklistItems.Remove(itemChecklistToDelete);

            //Create Integration Event to be published through the Event Bus
            var ItemChecklistExclusaoEvent = new ItemChecklistExclusaoIE(itemChecklistToDelete.Id_ItemChecklist);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndChecklistContextChangesAsync(ItemChecklistExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(ItemChecklistExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirItemChecklist), null);
        }

        /// <summary>
        /// Consulta os itens de checklist.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarItemChecklist()
        {
            List<ConsultarItemChecklistTO> l_ListItemChecklistTO = new List<ConsultarItemChecklistTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarItemChecklistTO> mycache = new Cache<ConsultarItemChecklistTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListItemChecklistTO = await mycache.GetListAsync("ConsultarItemChecklist_" + cachePrefix);
                if (l_ListItemChecklistTO.Count == 0)
                {
                    ConsultarItemChecklistTO sqlClass = new ConsultarItemChecklistTO();
                    sqlClass.ConsultarItemChecklistTOCommand(_settings.ConnectionString, ref l_ListItemChecklistTO);

                    if (l_ListItemChecklistTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarItemChecklist_" + cachePrefix, l_ListItemChecklistTO);
                    }
                }
            }
            else
            {
                ConsultarItemChecklistTO sqlClass = new ConsultarItemChecklistTO();
                sqlClass.ConsultarItemChecklistTOCommand(_settings.ConnectionString, ref l_ListItemChecklistTO);
            }


            return Ok(l_ListItemChecklistTO);

        }

        private bool ruleValidaNomeItemChecklistUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_ITEMCHECKLISTNOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["ItemChecklistNomeUnique"];
                return true;
            }

            return false;

        }
        private bool ruleValidaNomeItemCheckList(string nome_ItemChecklist, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_ItemChecklist ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_ItemChecklist.Length < 3) || (nome_ItemChecklist.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }
    }
}