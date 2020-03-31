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
    public class TipoSituacaoTipoAtividadeController : ControllerBase
    {
        private const string cachePrefix = "TIPOSITUACAOTIPOATIVIDADECOMODACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<TipoSituacaoTipoAtividadeController> _localizer;

        public TipoSituacaoTipoAtividadeController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<TipoSituacaoTipoAtividadeController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui uma associação de Tipo de Situação e Tipo de Atividade de Acomodação.
        /// </summary>
        /// <param name="tipoSituacaoTipoAtividadeToSave">
        /// Objeto que representa a associação de tipo de situação e tipo de atividade da Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirTipoSituacaoTipoAtividadeAcomodacao([FromBody]TipoSituacaoTipoAtividadeAcomodacaoItem tipoSituacaoTipoAtividadeToSave)
        {
            string msgRule = "";

            _configuracaoContext.TipoSituacaoTipoAtividadeAcomodacaoItems.Add(tipoSituacaoTipoAtividadeToSave);            

            //Create Integration Event to be published through the Event Bus
            var tipoSituacaoTipoAtividadeAcomodacaoSaveEvent = new TipoSituacaoTAAIncluirIE(tipoSituacaoTipoAtividadeToSave.Id_TipoSituacaoAcomodacao, tipoSituacaoTipoAtividadeToSave.Id_TipoAtividadeAcomodacao);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.IncluirEventAndTipoSituacaoTipoAtividadeAcomodacaoContextChangesAsync(tipoSituacaoTipoAtividadeAcomodacaoSaveEvent);
            }
            catch (Exception e)
            {
               
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaTipoSituacaoTipoAtividadeAcomodacaoPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(tipoSituacaoTipoAtividadeAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(IncluirTipoSituacaoTipoAtividadeAcomodacao), null);
        }

        /// <summary>
        /// Exclui uma associação de tipo de situação e tipo de atividade da acomodação.
        /// </summary>
        /// <param name="id_TipoSituacaoAcomodacao">
        /// Identificador do tipo de situação da acomodação
        /// </param>
        /// <param name="id_TipoAtividadeAcomodacao">
        /// Identificador do tipo de atividade da acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirTipoSituacaoTipoAtividadeAcomodacao(int id_TipoSituacaoAcomodacao,int id_TipoAtividadeAcomodacao)
        {

            if (id_TipoSituacaoAcomodacao < 1|| id_TipoAtividadeAcomodacao<1)
            {
                return BadRequest();
            }

            var tipoSituacaoTipoAtividadeAcomodacaoToDelete = _configuracaoContext.TipoSituacaoTipoAtividadeAcomodacaoItems
                .OfType<TipoSituacaoTipoAtividadeAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_TipoSituacaoAcomodacao == id_TipoSituacaoAcomodacao && c.Id_TipoAtividadeAcomodacao == id_TipoAtividadeAcomodacao);

            if (tipoSituacaoTipoAtividadeAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.TipoSituacaoTipoAtividadeAcomodacaoItems.Remove(tipoSituacaoTipoAtividadeAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var TipoSituacaoTipoAtividadeAcomodacaoExclusaoEvent = new TipoSituacaoTAAExclusaoIE(tipoSituacaoTipoAtividadeAcomodacaoToDelete.Id_TipoSituacaoAcomodacao, tipoSituacaoTipoAtividadeAcomodacaoToDelete.Id_TipoAtividadeAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndTipoSituacaoAcomodacaoContextChangesAsync(TipoSituacaoTipoAtividadeAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(TipoSituacaoTipoAtividadeAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirTipoSituacaoTipoAtividadeAcomodacao), null);
        }

        /// <summary>
        /// Consulta as associações entre todos tipo de situação e tipo de atividade de acomodação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoTipoAtividadeAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoSituacaoTipoAtividadeAcomodacao()
        {
            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = new List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> mycache = new Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacao_" + cachePrefix);
                if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                    sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

                    if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacao_" + cachePrefix, l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoTOCommand(_settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            }


                return Ok(l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

        }
        /// <summary>
        /// Consulta as associações entre um tipo de atividade e seus tipos de situação de acomodação.
        /// </summary>
        /// <param name="idTipoAtividadeAcomodacao">
        /// Identificador do tipo de atividade da acomodação
        /// </param>
        [HttpGet]
        [Route("items/tipoatividade/{idTipoAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoTipoAtividadeAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoAtividade(int idTipoAtividadeAcomodacao)
        {
            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = new List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> mycache = new Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoAtividade_" + cachePrefix + idTipoAtividadeAcomodacao.ToString());
                if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                    sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorAtividadeTOCommand(idTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

                    if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoAtividade_" + cachePrefix + idTipoAtividadeAcomodacao.ToString(), l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorAtividadeTOCommand(idTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            }


                return Ok(l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

        }
        /// <summary>
        /// Consulta as associações entre um tipo de situação e seus tipos de atividade de acomodação.
        /// </summary>
        /// <param name="idTipoSituacaoAcomodacao">
        /// Identificador do tipo de situação da acomodação
        /// </param>
        [HttpGet]
        [Route("items/tiposituacao/{idTipoSituacaoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<TipoSituacaoTipoAtividadeAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoSituacao(int idTipoSituacaoAcomodacao)
        {
            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = new List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> mycache = new Cache<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTipoSituacaoTipoAtividadeAcomodacaoTO = await mycache.GetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoSituacao_" + cachePrefix + idTipoSituacaoAcomodacao.ToString ());
                if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count == 0)
                {
                    ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                    sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorSituacaoTOCommand(idTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

                    if (l_ListTipoSituacaoTipoAtividadeAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorTipoSituacao_" + cachePrefix + idTipoSituacaoAcomodacao.ToString(), l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO sqlClass = new ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO();
                sqlClass.ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorSituacaoTOCommand(idTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            }


                return Ok(l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);

        }

        private bool ruleValidaTipoSituacaoTipoAtividadeAcomodacaoPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_TIPOSITUACAO_TIPOATIVIDADEACOMODACAO"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["TipoSituacaoTipoAtividadeAcomodacaoPK"];
                return true;
            }

            return false;

        }
    }
}