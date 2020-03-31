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
    public class CheckListController : ControllerBase
    {
        private const string cachePrefix = "CHECKLIST#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<CheckListController> _localizer;

        public CheckListController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<CheckListController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um checklist.
        /// </summary>
        /// <param name="checklistToSave">
        /// Objeto que representa o checklist
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarChecklist([FromBody]ChecklistItem checklistToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeCheckList(checklistToSave.Nome_Checklist, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_configuracaoContext.Set<ChecklistItem>().Any(e => e.Id_Checklist == checklistToSave.Id_Checklist ))
            {
                _configuracaoContext.ChecklistItems.Update(checklistToSave);
            }
            else
            {
                _configuracaoContext.ChecklistItems.Add(checklistToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var checklistSaveEvent = new ChecklistSaveIE(checklistToSave.Id_Checklist, checklistToSave.Nome_Checklist);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndChecklistContextChangesAsync(checklistSaveEvent, checklistToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeChecklistUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(checklistSaveEvent);


            return CreatedAtAction(nameof(SalvarChecklist), checklistToSave.Id_Checklist);
        }
        /// <summary>
        /// Exclui um checklist.
        /// </summary>
        /// <param name="id_Checklist">
        /// Identificador do checklist
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirChecklist(int id_Checklist)
        {

            if (id_Checklist < 1)
            {
                return BadRequest();
            }

            var checklistToDelete = _configuracaoContext.ChecklistItems
                .OfType<ChecklistItem>()
                .SingleOrDefault(c => c.Id_Checklist == id_Checklist);

            if (checklistToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.ChecklistItems.Remove(checklistToDelete);

            //Create Integration Event to be published through the Event Bus
            var ChecklistExclusaoEvent = new ChecklistExclusaoIE(checklistToDelete.Id_Checklist);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndChecklistContextChangesAsync(ChecklistExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(ChecklistExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirChecklist), null);
        }

        /// <summary>
        /// Consulta os checklist.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarChecklist()
        {
            List<ConsultarChecklistTO> l_ListChecklistTO = new List<ConsultarChecklistTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarChecklistTO> mycache = new Cache<ConsultarChecklistTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListChecklistTO = await mycache.GetListAsync("ConsultarChecklist_" + cachePrefix);
                if (l_ListChecklistTO.Count == 0)
                {
                    ConsultarChecklistTO sqlClass = new ConsultarChecklistTO();
                    sqlClass.ConsultarChecklistTOCommand(_settings.ConnectionString, ref l_ListChecklistTO);

                    if (l_ListChecklistTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarChecklist_" + cachePrefix, l_ListChecklistTO);
                    }
                }
            }
            else
            {
                ConsultarChecklistTO sqlClass = new ConsultarChecklistTO();
                sqlClass.ConsultarChecklistTOCommand(_settings.ConnectionString, ref l_ListChecklistTO);
            }


            return Ok(l_ListChecklistTO);

        }

        /// <summary>
        /// Consulta os checklist e seus items.
        /// </summary>
        /// <param name="IdChecklist">
        /// Identificador do checklist
        /// </param>
        [HttpGet]
        [Route("items/checklist/{IdChecklist}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarChecklistDetalhe(int IdChecklist)
        {
            List<ConsultarChecklistDetalheTO> l_ListChecklistTO = new List<ConsultarChecklistDetalheTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarChecklistDetalheTO> mycache = new Cache<ConsultarChecklistDetalheTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListChecklistTO = await mycache.GetListAsync("ConsultarChecklistDetalhe_" + cachePrefix+ IdChecklist.ToString());
                if (l_ListChecklistTO.Count == 0)
                {
                    ConsultarChecklistDetalheTO sqlClass = new ConsultarChecklistDetalheTO();
                    sqlClass.ConsultarChecklistDetalheTOCommand(IdChecklist,_settings.ConnectionString, ref l_ListChecklistTO);

                    if (l_ListChecklistTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarChecklistDetalhe_" + cachePrefix + IdChecklist.ToString(), l_ListChecklistTO);
                    }
                }
            }
            else
            {
                ConsultarChecklistDetalheTO sqlClass = new ConsultarChecklistDetalheTO();
                sqlClass.ConsultarChecklistDetalheTOCommand(IdChecklist,_settings.ConnectionString, ref l_ListChecklistTO);
            }


            return Ok(l_ListChecklistTO);

        }

        /// <summary>
        /// Consulta o checklist e seus items de uma determinada situação / atividade / tipo acomodação de uma empresa.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        /// <param name="IdTipoAcomodacao">
        /// Identificador do Tipo de Acomodação
        /// </param>
        /// <param name="IdTipoAtividade">
        /// Identificador do Tipo de Atividade
        /// </param>
        /// <param name="IdTipoSituacao">
        /// Identificador do Tipo de Situação
        /// </param>
        [HttpGet]
        [Route("items/empresa/{IdEmpresa}/tipoacomodacao/{IdTipoAcomodacao}/tipoatividade/{IdTipoAtividade}/tiposituacao/{IdTipoSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacao(int IdEmpresa,int IdTipoAcomodacao, int IdTipoAtividade, int IdTipoSituacao)
        {
            List<ConsultarChecklistDetalheTO> l_ListChecklistTO = new List<ConsultarChecklistDetalheTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarChecklistDetalheTO> mycache = new Cache<ConsultarChecklistDetalheTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListChecklistTO = await mycache.GetListAsync("ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacao_" + cachePrefix +
                                                                                IdEmpresa.ToString() + "@" +
                                                                                IdTipoAcomodacao.ToString() + "@" +
                                                                                IdTipoAtividade.ToString() + "@" +
                                                                                IdTipoSituacao.ToString());
                if (l_ListChecklistTO.Count == 0)
                {
                    ConsultarChecklistDetalheTO sqlClass = new ConsultarChecklistDetalheTO();
                    sqlClass.ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacaoTOCommand(IdEmpresa,IdTipoAcomodacao,IdTipoAtividade,IdTipoSituacao, _settings.ConnectionString, ref l_ListChecklistTO);

                    if (l_ListChecklistTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacao_" + cachePrefix +
                                                                                    IdEmpresa.ToString() + "@" +
                                                                                    IdTipoAcomodacao.ToString() + "@" +
                                                                                    IdTipoAtividade.ToString() + "@" +
                                                                                    IdTipoSituacao.ToString(), l_ListChecklistTO);
                    }
                }
            }
            else
            {
                ConsultarChecklistDetalheTO sqlClass = new ConsultarChecklistDetalheTO();
                sqlClass.ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacaoTOCommand(IdEmpresa, IdTipoAcomodacao, IdTipoAtividade, IdTipoSituacao, _settings.ConnectionString, ref l_ListChecklistTO);
            }


            return Ok(l_ListChecklistTO);

        }

        private bool ruleValidaNomeChecklistUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_CHECKLISTNOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["ChecklistNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomeCheckList(string nome_Checklist, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Checklist))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Checklist.Length < 3) || (nome_Checklist.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }
    }
}