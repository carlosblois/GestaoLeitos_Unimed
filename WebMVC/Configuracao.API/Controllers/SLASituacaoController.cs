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
    public class SLASituacaoController : ControllerBase
    {
        private const string cachePrefix = "SLASITUACAO#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<SLASituacaoController> _localizer;

        public SLASituacaoController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<SLASituacaoController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um SLA de Tipo de Situação de uma empresa.
        /// </summary>
        /// <param name="slaSituacaoToSave">
        /// Objeto que representa o sla de tipo de situação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarSLASituacao([FromBody]SLASituacaoItem slaSituacaoToSave)
        {
            string msgRule = "";

            if (_configuracaoContext.Set<SLASituacaoItem>().Any(e => e.Id_SLA == slaSituacaoToSave.Id_SLA))
            {
                _configuracaoContext.SLASituacaoItems.Update(slaSituacaoToSave);
            }
            else
            {
                _configuracaoContext.SLASituacaoItems.Add(slaSituacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var slaSituacaoSaveEvent = new SLASituacaoSaveIE(slaSituacaoToSave.Id_SLA, slaSituacaoToSave.Id_TipoSituacaoAcomodacao, slaSituacaoToSave.Id_Empresa, slaSituacaoToSave.Tempo_Minutos, slaSituacaoToSave.Versao_SLA);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndSLASituacaoContextChangesAsync(slaSituacaoSaveEvent, slaSituacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaVersaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(slaSituacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarSLASituacao), slaSituacaoToSave.Id_SLA);
        }

        /// <summary>
        /// Exclui um SLA de tipo de situação.
        /// </summary>
        /// <param name="id_SLA">
        /// Identificador do SLA
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirSLASituacao(int id_SLA)
        {

            if (id_SLA < 1)
            {
                return BadRequest();
            }

            var slaSituacaoToDelete = _configuracaoContext.SLASituacaoItems
                .OfType<SLASituacaoItem>()
                .SingleOrDefault(c => c.Id_SLA == id_SLA);

            if (slaSituacaoToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.SLASituacaoItems.Remove(slaSituacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var SLASituacaoExclusaoEvent = new SLASituacaoExclusaoIE(slaSituacaoToDelete.Id_SLA);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndSLASituacaoContextChangesAsync(SLASituacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(SLASituacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirSLASituacao), null);
        }

        /// <summary>
        /// Consulta os SLAs de Situação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLASituacao()
        {
            List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO = new List<ConsultarSLASituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLASituacaoTO> mycache = new Cache<ConsultarSLASituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLASituacaoTO = await mycache.GetListAsync("ConsultarSLASituacao_" + cachePrefix);
                if (l_ListSLASituacaoTO.Count == 0)
                {
                    ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                    sqlClass.ConsultarSLASituacaoTOCommand(_settings.ConnectionString, ref l_ListSLASituacaoTO);

                    if (l_ListSLASituacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLASituacao_" + cachePrefix, l_ListSLASituacaoTO);
                    }
                }
            }
            else
            {
                ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                sqlClass.ConsultarSLASituacaoTOCommand(_settings.ConnectionString, ref l_ListSLASituacaoTO);
            }


                return Ok(l_ListSLASituacaoTO);

        }
        /// <summary>
        /// Consulta os SLAs de Situação de uma empresa de um tipo de Situacao em um tipo de acomodacao.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAcomodacao">
        /// Identificador de Tipo de acomodacao
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}/tipoacomodacao/{idTipoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLASituacaoPorIdEmpresaIdTipoSituacaoTipoAcomodacao(int idEmpresa, int idTipoSituacao,int idTipoAcomodacao)
        {
            List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO = new List<ConsultarSLASituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLASituacaoTO> mycache = new Cache<ConsultarSLASituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLASituacaoTO = await mycache.GetListAsync("ConsultarSLASituacaoPorIdEmpresaIdTipoSituacaoTipoAcomodacao_" + cachePrefix +
                                                                    idEmpresa.ToString() + "@" +
                                                                    idTipoSituacao.ToString() + "@" +
                                                                    idTipoAcomodacao.ToString());
                if (l_ListSLASituacaoTO.Count == 0)
                {
                    ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                    sqlClass.ConsultarSLASituacaoPorIdTipoSituacaoIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAcomodacao,_settings.ConnectionString, ref l_ListSLASituacaoTO);

                    if (l_ListSLASituacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLASituacaoPorIdEmpresaIdTipoSituacaoTipoAcomodacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idTipoSituacao.ToString() + "@" +
                                                    idTipoAcomodacao.ToString(), l_ListSLASituacaoTO);
                    }
                }
            }
            else
            {
                ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                sqlClass.ConsultarSLASituacaoPorIdTipoSituacaoTOCommand(idEmpresa, idTipoSituacao, _settings.ConnectionString, ref l_ListSLASituacaoTO);
            }


            return Ok(l_ListSLASituacaoTO);

        }


        /// <summary>
        /// Consulta os SLAs de Situação de uma empresa de um tipo de Situacao.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLASituacaoPorIdEmpresaIdTipoSituacao(int idEmpresa, int idTipoSituacao)
        {
            List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO = new List<ConsultarSLASituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLASituacaoTO> mycache = new Cache<ConsultarSLASituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLASituacaoTO = await mycache.GetListAsync("ConsultarSLASituacaoPorIdEmpresaIdTipoSituacao_" + cachePrefix + 
                                                                    idEmpresa.ToString() + "@" + 
                                                                    idTipoSituacao.ToString());
                if (l_ListSLASituacaoTO.Count == 0)
                {
                    ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                    sqlClass.ConsultarSLASituacaoPorIdTipoSituacaoTOCommand(idEmpresa, idTipoSituacao, _settings.ConnectionString, ref l_ListSLASituacaoTO);

                    if (l_ListSLASituacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLASituacaoPorIdEmpresaIdTipoSituacao_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idTipoSituacao.ToString(), l_ListSLASituacaoTO);
                    }
                }
            }
            else
            {
                ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                sqlClass.ConsultarSLASituacaoPorIdTipoSituacaoTOCommand(idEmpresa, idTipoSituacao, _settings.ConnectionString, ref l_ListSLASituacaoTO);
            }


                return Ok(l_ListSLASituacaoTO);

        }

        /// <summary>
        /// Consulta os SLAs de Situação de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLASituacaoPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO = new List<ConsultarSLASituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLASituacaoTO> mycache = new Cache<ConsultarSLASituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLASituacaoTO = await mycache.GetListAsync("ConsultarSLASituacaoPorIdEmpresa_" + cachePrefix + idEmpresa.ToString());
                if (l_ListSLASituacaoTO.Count == 0)
                {
                    ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                    sqlClass.ConsultarSLASituacaoPorIdEmpresaTOCommand(idEmpresa,_settings.ConnectionString, ref l_ListSLASituacaoTO);

                    if (l_ListSLASituacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLASituacaoPorIdEmpresa_" + cachePrefix + idEmpresa.ToString(), l_ListSLASituacaoTO);
                    }
                }
            }
            else
            {
                ConsultarSLASituacaoTO sqlClass = new ConsultarSLASituacaoTO();
                sqlClass.ConsultarSLASituacaoPorIdEmpresaTOCommand(idEmpresa,_settings.ConnectionString, ref l_ListSLASituacaoTO);
            }


                return Ok(l_ListSLASituacaoTO);

        }
        private bool ruleValidaVersaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_SLASITUACAOVERSAO"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["SLASituacaoVersaoUnique"];
                return true;
            }

            return false;

        }

    }
}