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
    public class TipoAtividadeAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "TIPOATIVIDADEACOMODACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<TipoAtividadeAcomodacaoController> _localizer;

        public TipoAtividadeAcomodacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<TipoAtividadeAcomodacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um Tipo de Atividade de Acomodação.
        /// </summary>
        /// <param name="tipoAtividadeAcomodacaoToSave">
        /// Objeto que representa o tipo de atividade da Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarTipoAtividadeAcomodacao([FromBody]TipoAtividadeAcomodacaoItem tipoAtividadeAcomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeTipoAtividadeAcomodacao(tipoAtividadeAcomodacaoToSave.Nome_TipoAtividadeAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_configuracaoContext.Set<TipoAtividadeAcomodacaoItem>().Any(e => e.Id_TipoAtividadeAcomodacao == tipoAtividadeAcomodacaoToSave.Id_TipoAtividadeAcomodacao))
            {
                _configuracaoContext.TipoAtividadeAcomodacaoItems.Update(tipoAtividadeAcomodacaoToSave);
            }
            else
            {
                _configuracaoContext.TipoAtividadeAcomodacaoItems.Add(tipoAtividadeAcomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var tipoAtividadeAcomodacaoSaveEvent = new TipoAtividadeAcomodacaoSaveIE(tipoAtividadeAcomodacaoToSave.Id_TipoAtividadeAcomodacao, tipoAtividadeAcomodacaoToSave.Nome_TipoAtividadeAcomodacao);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndTipoAtividadeAcomodacaoContextChangesAsync(tipoAtividadeAcomodacaoSaveEvent, tipoAtividadeAcomodacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeTipoAtividadeAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(tipoAtividadeAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarTipoAtividadeAcomodacao), tipoAtividadeAcomodacaoToSave.Id_TipoAtividadeAcomodacao);
        }

        /// <summary>
        /// Exclui um tipo de Atividade de acomodação.
        /// </summary>
        /// <param name="id_TipoAtividadeAcomodacao">
        /// Identificador do tipo de atividade da acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirTipoAtividadeAcomodacao(int id_TipoAtividadeAcomodacao)
        {

            if (id_TipoAtividadeAcomodacao < 1)
            {
                return BadRequest();
            }

            var tipoAtividadeAcomodacaoToDelete = _configuracaoContext.TipoAtividadeAcomodacaoItems
                .OfType<TipoAtividadeAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_TipoAtividadeAcomodacao == id_TipoAtividadeAcomodacao);

            if (tipoAtividadeAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.TipoAtividadeAcomodacaoItems.Remove(tipoAtividadeAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var TipoSituacaoAcomodacaoExclusaoEvent = new TipoSituacaoAcomodacaoExclusaoIE(tipoAtividadeAcomodacaoToDelete.Id_TipoAtividadeAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndTipoSituacaoAcomodacaoContextChangesAsync(TipoSituacaoAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(TipoSituacaoAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirTipoAtividadeAcomodacao), null);
        }

        /// <summary>
        /// TELA ENCAMINHAR LEITO (código: 04) LeitoConsulta o tipo de situação de acomodação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoAtividadeAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoAtividadeAcomodacao()
        {
            List<ConsultarTipoAtividadeAcomodacaoTO> l_ListTipoAtividadeAcomodacaoTO = new List<ConsultarTipoAtividadeAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoAtividadeAcomodacaoTO> mycache = new Cache<ConsultarTipoAtividadeAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoAtividadeAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoAtividadeAcomodacao_" + cachePrefix);
                if (l_ListTipoAtividadeAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoAtividadeAcomodacaoTO();
                    sqlClass.ConsultarTipoAtividadeAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoAtividadeAcomodacaoTO);

                    if (l_ListTipoAtividadeAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoAtividadeAcomodacao_" + cachePrefix, l_ListTipoAtividadeAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoAtividadeAcomodacaoTO();
                sqlClass.ConsultarTipoAtividadeAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoAtividadeAcomodacaoTO);
            }

                return Ok(l_ListTipoAtividadeAcomodacaoTO);

        }

        private bool ruleValidaNomeTipoAtividadeAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_TIPOATIVIDADEACOMODACAONOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["TipoAtividadeAcomodacaoNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomeTipoAtividadeAcomodacao(string nome_TipoAtividadeAcomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_TipoAtividadeAcomodacao ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_TipoAtividadeAcomodacao.Length < 3) || (nome_TipoAtividadeAcomodacao.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;
        }
    }
}