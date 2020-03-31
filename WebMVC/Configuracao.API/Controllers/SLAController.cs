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
    public class SLAController : ControllerBase
    {

        private const string cachePrefix = "SLA#";
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _configuracaoIntegrationEventService;
        private readonly IStringLocalizer<SLAController> _localizer;

        public SLAController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService configuracaoIntegrationEventService, IStringLocalizer<SLAController> localizer)
        {
            _configuracaoContext = context ?? throw new ArgumentNullException(nameof(context));
            _configuracaoIntegrationEventService = configuracaoIntegrationEventService ?? throw new ArgumentNullException(nameof(configuracaoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza um SLA de uma ação / Atividade / Situação de uma empresa.
        /// </summary>
        /// <param name="slaToSave">
        /// Objeto que representa o sla
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarSLA([FromBody]SLAItem slaToSave)
        {
            string msgRule = "";

            if (_configuracaoContext.Set<SLAItem>().Any(e => e.Id_SLA == slaToSave.Id_SLA))
            {
                _configuracaoContext.SLAItems.Update(slaToSave);
            }
            else
            {
                _configuracaoContext.SLAItems.Add(slaToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var slaSaveEvent = new SLASaveIE(slaToSave.Id_SLA, slaToSave.Id_TipoSituacaoAcomodacao, slaToSave.Id_TipoAtividadeAcomodacao, slaToSave.Id_TipoAcaoAcomodacao,slaToSave.Id_Empresa, slaToSave.Tempo_Minutos, slaToSave.Versao_SLA);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _configuracaoIntegrationEventService.SaveEventAndSLAContextChangesAsync(slaSaveEvent, slaToSave);
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
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(slaSaveEvent);


            return CreatedAtAction(nameof(SalvarSLA), slaToSave.Id_SLA);
        }

        /// <summary>
        /// Exclui um SLA.
        /// </summary>
        /// <param name="id_SLA">
        /// Identificador do SLA
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirSLA(int id_SLA)
        {

            if (id_SLA < 1)
            {
                return BadRequest();
            }

            var slaToDelete = _configuracaoContext.SLAItems
                .OfType<SLAItem>()
                .SingleOrDefault(c => c.Id_SLA == id_SLA);

            if (slaToDelete is null)
            {
                return BadRequest();
            }

            _configuracaoContext.SLAItems.Remove(slaToDelete);

            //Create Integration Event to be published through the Event Bus
            var SLAExclusaoEvent = new SLAExclusaoIE(slaToDelete.Id_SLA);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _configuracaoIntegrationEventService.DeleteEventAndSLAContextChangesAsync(SLAExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _configuracaoIntegrationEventService.PublishThroughEventBusAsync(SLAExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirSLA), null);
        }

        /// <summary>
        /// Consulta o SLA de Atividade por / Atividade / Situação de uma empresa e tipo de acomodacao (ultima versao).
        /// Essa consulta é uma consolidação dos tempos de SLA das ações de uma atividade
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAtividade">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAcomodacao">
        /// Identificador de Tipo de acomodacao
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}/tipoatividade/{idTipoAtividade}/tipoacomodacao/{idTipoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacao(int idEmpresa, int idTipoSituacao, int idTipoAtividade, int idTipoAcomodacao)
        {
            List<ConsultarSLAAtividadeTO> l_ListSLATO = new List<ConsultarSLAAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLAAtividadeTO> mycache = new Cache<ConsultarSLAAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacao_" + cachePrefix +
                                                        idEmpresa.ToString() + "@" +
                                                        idTipoSituacao.ToString() + "@" +
                                                        idTipoAtividade.ToString() + "@" +
                                                        idTipoAcomodacao.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLAAtividadeTO sqlClass = new ConsultarSLAAtividadeTO();
                    sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idTipoSituacao.ToString() + "@" +
                                                     idTipoAtividade.ToString() + "@" +
                                                     idTipoAcomodacao.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLAAtividadeTO sqlClass = new ConsultarSLAAtividadeTO();
                sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);
            }


                return Ok(l_ListSLATO);

        }

        /// <summary>
        /// Consulta o SLA de Ação / Atividade / Situação por tipo de acaomodacao de uma empresa (ultima versao).
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAtividade">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAcao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAcomodacao">
        /// Identificador de Tipo de acomodacao
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}/tipoatividade/{idTipoAtividade}/tipoacao/{idTipoAcao}/tipoacomodacao/{idTipoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao(int idEmpresa, int idTipoSituacao, int idTipoAtividade, int idTipoAcao, int idTipoAcomodacao)
        {
            List<ConsultarSLATO> l_ListSLATO = new List<ConsultarSLATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLATO> mycache = new Cache<ConsultarSLATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao_" + cachePrefix +
                                                        idEmpresa.ToString() + "@" +
                                                        idTipoSituacao.ToString() + "@" +
                                                        idTipoAtividade.ToString() + "@" +
                                                        idTipoAcao.ToString() + "@" +
                                                        idTipoAcomodacao.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLATO sqlClass = new ConsultarSLATO();
                    sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcao,idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idTipoSituacao.ToString() + "@" +
                                                    idTipoAtividade.ToString() + "@" +
                                                    idTipoAcao.ToString() + "@" +
                                                    idTipoAcomodacao.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLATO sqlClass = new ConsultarSLATO();
                sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcao, idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);
            }


            return Ok(l_ListSLATO);

        }

        /// <summary>
        /// Listar os SLA de Ação / Atividade / Situação por tipo de acaomodacao de uma empresa (ultima versao).
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAtividade">
        /// Identificador de Tipo de atividade
        /// </param>
        /// <param name="idTipoAcao">
        /// Identificador de Tipo de ação
        /// </param>
        /// <param name="idTipoAcomodacao">
        /// Identificador de Tipo de acomodacao
        /// </param>
        [HttpGet]
        [Route("items/listar/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}/tipoatividade/{idTipoAtividade}/tipoacao/{idTipoAcao}/tipoacomodacao/{idTipoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao(int idEmpresa, int idTipoSituacao, int idTipoAtividade, int idTipoAcao, int idTipoAcomodacao)
        {
            List<ConsultarSLATO> l_ListSLATO = new List<ConsultarSLATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLATO> mycache = new Cache<ConsultarSLATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao_" + cachePrefix +
                                                        idEmpresa.ToString() + "@" +
                                                        idTipoSituacao.ToString() + "@" +
                                                        idTipoAtividade.ToString() + "@" +
                                                        idTipoAcao.ToString() + "@" +
                                                        idTipoAcomodacao.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLATO sqlClass = new ConsultarSLATO();
                    sqlClass.ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcao, idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idTipoSituacao.ToString() + "@" +
                                                    idTipoAtividade.ToString() + "@" +
                                                    idTipoAcao.ToString() + "@" +
                                                    idTipoAcomodacao.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLATO sqlClass = new ConsultarSLATO();
                sqlClass.ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcao, idTipoAcomodacao, _settings.ConnectionString, ref l_ListSLATO);
            }


            return Ok(l_ListSLATO);

        }

        /// <summary>
        /// Consulta SLA por Id do SLA.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idSla">
        /// Identificador do Sla
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/id/{idSla}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLAPorId(int idEmpresa, int idSla)
        {
            List<ConsultarSLATO> l_ListSLATO = new List<ConsultarSLATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLATO> mycache = new Cache<ConsultarSLATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ConsultarSLAPorId_" + cachePrefix +
                                                        idEmpresa.ToString() + "@" +
                                                        idSla.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLATO sqlClass = new ConsultarSLATO();
                    sqlClass.ConsultarSLAPorIdTOCommand(idEmpresa, idSla, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLAPorId_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idSla.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLATO sqlClass = new ConsultarSLATO();
                sqlClass.ConsultarSLAPorIdTOCommand(idEmpresa, idSla, _settings.ConnectionString, ref l_ListSLATO);
            }


            return Ok(l_ListSLATO);

        }


        /// <summary>
        /// Consulta o SLA de Ação / Atividade / Situação de uma empresa (ultima versao).
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoSituacao">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAtividade">
        /// Identificador de Tipo de situacao
        /// </param>
        /// <param name="idTipoAcao">
        /// Identificador de Tipo de situacao
        /// </param>
        [HttpGet]      
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacao}/tipoatividade/{idTipoAtividade}/tipoacao/{idTipoAcao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLASituacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcao(int idEmpresa, int idTipoSituacao,int idTipoAtividade,int idTipoAcao)
        {
            List<ConsultarSLATO> l_ListSLATO = new List<ConsultarSLATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLATO> mycache = new Cache<ConsultarSLATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcao_" + cachePrefix + 
                                                        idEmpresa.ToString() + "@" + 
                                                        idTipoSituacao.ToString() + "@" + 
                                                        idTipoAtividade.ToString() + "@" + 
                                                        idTipoAcao.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLATO sqlClass = new ConsultarSLATO();
                    sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade,  idTipoAcao, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcao_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idTipoSituacao.ToString() + "@" + 
                                                    idTipoAtividade.ToString() + "@" + 
                                                    idTipoAcao.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLATO sqlClass = new ConsultarSLATO();
                sqlClass.ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoTOCommand(idEmpresa, idTipoSituacao, idTipoAtividade, idTipoAcao, _settings.ConnectionString, ref l_ListSLATO);
            }


                return Ok(l_ListSLATO);

        }

        /// <summary>
        /// Consulta os SLAs de Ação / Atividade / Situação de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SLAItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSLAPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarSLATO> l_ListSLATO = new List<ConsultarSLATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSLATO> mycache = new Cache<ConsultarSLATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSLATO = await mycache.GetListAsync("ConsultarSLAPorIdEmpresa_" + cachePrefix + idEmpresa.ToString());
                if (l_ListSLATO.Count == 0)
                {
                    ConsultarSLATO sqlClass = new ConsultarSLATO();
                    sqlClass.ConsultarSLAPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListSLATO);

                    if (l_ListSLATO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSLAPorIdEmpresa_" + cachePrefix + idEmpresa.ToString(), l_ListSLATO);
                    }
                }
            }
            else
            {
                ConsultarSLATO sqlClass = new ConsultarSLATO();
                sqlClass.ConsultarSLAPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListSLATO);
            }


            return Ok(l_ListSLATO);

        }
        private bool ruleValidaVersaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_SLAVERSAO"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["SLAVersaoUnique"];
                return true;
            }

            return false;

        }
    }
}