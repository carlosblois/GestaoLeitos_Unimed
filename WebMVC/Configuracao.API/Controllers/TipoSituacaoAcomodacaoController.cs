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
    public class TipoSituacaoAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "TIPOSITUACAOACOMODACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<TipoSituacaoAcomodacaoController> _localizer;

        public TipoSituacaoAcomodacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<TipoSituacaoAcomodacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um Tipo de Situação de Acomodação.
        /// </summary>
        /// <param name="tipoSituacaoAcomodacaoToSave">
        /// Objeto que representa o tipo de situação da Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarTipoSituacaoAcomodacao([FromBody]TipoSituacaoAcomodacaoItem tipoSituacaoAcomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeTipoSituacaoAcomodacao(tipoSituacaoAcomodacaoToSave.Nome_TipoSituacaoAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_configuracaoContext.Set<TipoSituacaoAcomodacaoItem>().Any(e => e.Id_TipoSituacaoAcomodacao == tipoSituacaoAcomodacaoToSave.Id_TipoSituacaoAcomodacao))
            {
                _configuracaoContext.TipoSituacaoAcomodacaoItems.Update(tipoSituacaoAcomodacaoToSave);
            }
            else
            {
                _configuracaoContext.TipoSituacaoAcomodacaoItems.Add(tipoSituacaoAcomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var tipoSituacacoAcomodacaoSaveEvent = new TipoSituacaoAcomodacaoSaveIE(tipoSituacaoAcomodacaoToSave.Id_TipoSituacaoAcomodacao, tipoSituacaoAcomodacaoToSave.Nome_TipoSituacaoAcomodacao);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndTipoSituacaoAcomodacaoContextChangesAsync(tipoSituacacoAcomodacaoSaveEvent, tipoSituacaoAcomodacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeTipoSituacaoAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(tipoSituacacoAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarTipoSituacaoAcomodacao), tipoSituacaoAcomodacaoToSave.Id_TipoSituacaoAcomodacao);
        }

        /// <summary>
        /// Exclui um tipo de situacção da acomodação.
        /// </summary>
        /// <param name="id_TipoSituacaoAcomodacao">
        /// Identificador do tipo de situação da acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirTipoSituacaoAcomodacao(int id_TipoSituacaoAcomodacao)
        {

            if (id_TipoSituacaoAcomodacao < 1)
            {
                return BadRequest();
            }

            var tipoSituacaoAcomodacaoToDelete = _configuracaoContext.TipoSituacaoAcomodacaoItems
                .OfType<TipoSituacaoAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_TipoSituacaoAcomodacao == id_TipoSituacaoAcomodacao);

            if (tipoSituacaoAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.TipoSituacaoAcomodacaoItems.Remove(tipoSituacaoAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var TipoSituacaoAcomodacaoExclusaoEvent = new TipoSituacaoAcomodacaoExclusaoIE(tipoSituacaoAcomodacaoToDelete.Id_TipoSituacaoAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndTipoSituacaoAcomodacaoContextChangesAsync(TipoSituacaoAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(TipoSituacaoAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirTipoSituacaoAcomodacao), null);
        }

        /// <summary>
        /// Consulta o tipo de situação de acomodação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoSituacaAcomodacao()
        {
            List<ConsultarTipoSituacaoAcomodacaoTO> l_ListTipoSituacaoAcomodacaoTO = new List<ConsultarTipoSituacaoAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoSituacaoAcomodacaoTO> mycache = new Cache<ConsultarTipoSituacaoAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoSituacaoAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoSituacaAcomodacao_" + cachePrefix);
                if (l_ListTipoSituacaoAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoSituacaoAcomodacaoTO sqlClass = new ConsultarTipoSituacaoAcomodacaoTO();
                    sqlClass.ConsultarTipoSituacaoAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoSituacaoAcomodacaoTO);

                    if (l_ListTipoSituacaoAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoSituacaAcomodacao_" + cachePrefix, l_ListTipoSituacaoAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoSituacaoAcomodacaoTO sqlClass = new ConsultarTipoSituacaoAcomodacaoTO();
                sqlClass.ConsultarTipoSituacaoAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoSituacaoAcomodacaoTO);
            }


                return Ok(l_ListTipoSituacaoAcomodacaoTO);

        }

        private bool ruleValidaNomeTipoSituacaoAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_TIPOSITUACAOACOMODACAONOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["TipoSituacaoAcomodacaoNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomeTipoSituacaoAcomodacao(string nome_TipoSituacaoAcomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_TipoSituacaoAcomodacao ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_TipoSituacaoAcomodacao.Length < 3) || (nome_TipoSituacaoAcomodacao.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;
        }
    }
}