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
    public class TipoAcaoAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "TIPOACAOACOMODACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<TipoAcaoAcomodacaoController> _localizer;

        public TipoAcaoAcomodacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<TipoAcaoAcomodacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um Tipo de Ação de Acomodação.
        /// </summary>
        /// <param name="tipoAcaoAcomodacaoToSave">
        /// Objeto que representa o tipo de ação da Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarTipoAcaoAcomodacao([FromBody]TipoAcaoAcomodacaoItem tipoAcaoAcomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeTipoAcaoAcomodacao(tipoAcaoAcomodacaoToSave.Nome_TipoAcaoAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            if (!ruleValidaStatusTipoAcaoAcomodacao(tipoAcaoAcomodacaoToSave.Nome_Status, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_configuracaoContext.Set<TipoAcaoAcomodacaoItem>().Any(e => e.Id_TipoAcaoAcomodacao == tipoAcaoAcomodacaoToSave.Id_TipoAcaoAcomodacao))
            {
                _configuracaoContext.TipoAcaoAcomodacaoItems.Update(tipoAcaoAcomodacaoToSave);
            }
            else
            {
                _configuracaoContext.TipoAcaoAcomodacaoItems.Add(tipoAcaoAcomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var tipoAcaoAcomodacaoSaveEvent = new TipoAcaoAcomodacaoSaveIE(tipoAcaoAcomodacaoToSave.Id_TipoAcaoAcomodacao, tipoAcaoAcomodacaoToSave.Nome_TipoAcaoAcomodacao, tipoAcaoAcomodacaoToSave.Nome_Status);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndTipoAcaoAcomodacaoContextChangesAsync(tipoAcaoAcomodacaoSaveEvent, tipoAcaoAcomodacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeTipoAcaoAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(tipoAcaoAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarTipoAcaoAcomodacao), tipoAcaoAcomodacaoToSave.Id_TipoAcaoAcomodacao);
        }

        /// <summary>
        /// Exclui um tipo de Ação de acomodação.
        /// </summary>
        /// <param name="id_TipoAcaoAcomodacao">
        /// Identificador do tipo de ação da acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirTipoAcaoAcomodacao(int id_TipoAcaoAcomodacao)
        {

            if (id_TipoAcaoAcomodacao < 1)
            {
                return BadRequest();
            }

            var tipoAcaoAcomodacaoToDelete = _configuracaoContext.TipoAcaoAcomodacaoItems
                .OfType<TipoAcaoAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_TipoAcaoAcomodacao == id_TipoAcaoAcomodacao);

            if (tipoAcaoAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.TipoAcaoAcomodacaoItems.Remove(tipoAcaoAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var TipoAcaoAcomodacaoExclusaoEvent = new TipoSituacaoAcomodacaoExclusaoIE(tipoAcaoAcomodacaoToDelete.Id_TipoAcaoAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndTipoAcaoAcomodacaoContextChangesAsync(TipoAcaoAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(TipoAcaoAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirTipoAcaoAcomodacao), null);
        }

        /// <summary>
        /// Consulta o tipo de ação de acomodação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoAcaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoAcaoAcomodacao()
        {
            List<ConsultarTipoAcaoAcomodacaoTO> l_ListTipoAcaoAcomodacaoTO = new List<ConsultarTipoAcaoAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoAcaoAcomodacaoTO> mycache = new Cache<ConsultarTipoAcaoAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoAcaoAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoAcaoAcomodacao_" + cachePrefix);
                if (l_ListTipoAcaoAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoAcaoAcomodacaoTO sqlClass = new ConsultarTipoAcaoAcomodacaoTO();
                    sqlClass.ConsultarTipoAcaoAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoAcaoAcomodacaoTO);

                    if (l_ListTipoAcaoAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoAcaoAcomodacao_" + cachePrefix, l_ListTipoAcaoAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoAcaoAcomodacaoTO sqlClass = new ConsultarTipoAcaoAcomodacaoTO();
                sqlClass.ConsultarTipoAcaoAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoAcaoAcomodacaoTO);
            }

                return Ok(l_ListTipoAcaoAcomodacaoTO);

        }

        private bool ruleValidaNomeTipoAcaoAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_TIPOACAOACOMODACAONOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["TipoAcaoAcomodacaoNomeUnique"];
                return true;
            }

            return false;

        }
        private bool ruleValidaStatusTipoAcaoAcomodacao(string status_TipoAcaoAcomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(status_TipoAcaoAcomodacao))
            {
                msgRetorno = _localizer["StatusRequerido"];
                return false;
            }

            if ((status_TipoAcaoAcomodacao.Length < 3) || (status_TipoAcaoAcomodacao.Length > 50))
            {
                msgRetorno = _localizer["StatusTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaNomeTipoAcaoAcomodacao(string nome_TipoAcaoAcomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_TipoAcaoAcomodacao))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_TipoAcaoAcomodacao.Length < 3) || (nome_TipoAcaoAcomodacao.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;
        }


    }
}