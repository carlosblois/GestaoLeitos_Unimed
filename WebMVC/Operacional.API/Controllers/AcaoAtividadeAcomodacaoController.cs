using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Operacional.API.Infrastructure;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Operacional.API.IntegrationEvents;

using Operacional.API.Model;
using Microsoft.Extensions.Localization;
using Operacional.API.TO;
using Microsoft.AspNetCore.Authorization;
using Operacional.API.IntegrationEvents.Events;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using static Operacional.API.Enum.ExpoEnum;
using static Operacional.API.Utilitarios.Util;
using EventBus.Events;
using Configuracao.API.TO;
using Administrativo.API.TO;
using Operacional.API.Utilitarios;

namespace Operacional.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AcaoAtividadeAcomodacaoController : ControllerBase
    {
      

        private const string cachePrefix = "ACAO#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<AcaoAtividadeAcomodacaoController> _localizer;

        public AcaoAtividadeAcomodacaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<AcaoAtividadeAcomodacaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Efetua o CANCELAMENTO DO ACEITE de uma atividade 
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        [HttpPost]
        [Route("items/cancelaraceitar/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelarAceitar(int idEmpresa, int idUsuario, int idAtividadeAcomodacao)
        {
            if ((idAtividadeAcomodacao < 1) || (idEmpresa < 1) || (idUsuario < 1))
            {
                return BadRequest();
            }

            //VALIDA SE A ACAO ATUAL É ACEITAR
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.Id_TipoAcaoAcomodacao == (int)TipoAcao.ACEITAR && c.dt_FimAcaoAtividade == null);

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;
            string AdministrativoURL = this._settings.AdministrativoURL ;
            string tokenURL = this._settings.TokenURL;

            int idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            int idTipoAtividadeAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao;
            int idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;
            //_operacionalContext.AcaoItems.Update(acaoAtividadeAcomodacaoToUpdate); Remover acao ACEITAR, conforme card: https://trello.com/c/IeljVS4P/104-gest%C3%A3o-de-leitos-app-cancelamento-de-aceite
            _operacionalContext.AcaoItems.Remove(acaoAtividadeAcomodacaoToUpdate);

            AcaoItem acaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idAtividadeAcomodacao, (int)TipoAcao.SOLICITAR, DateTime.Now, null, idUsuario.ToString(), idTipoAcomodacao);
            _operacionalContext.AcaoItems.Add(acaoToSave);

            //Create Integration Event to be published through the Event Bus
            var finalizaAceitarEvent = new FinalizaAcaoAcomodacaoIE("ACEITAR",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);


            List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = new List<ConsultarAcessoAtividadeEmpresaPerfilTO>();
            Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao, atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao);


            //Create Integration Event to be published through the Event Bus
            var geraSolicitarEvent = new GeraAcaoAcomodacaoSolicitarIE("SOLICITAR",
                                                                        acaoToSave.Id_AcaoAtividadeAcomodacao,
                                                                        acaoToSave.Id_AtividadeAcomodacao,
                                                                        acaoToSave.Id_TipoAcaoAcomodacao,
                                                                        acaoToSave.dt_InicioAcaoAtividade,
                                                                        acaoToSave.dt_FimAcaoAtividade,
                                                                        acaoToSave.Id_SLA,
                                                                        acaoToSave.Id_UsuarioExecutor,
                                                                        Perfis);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndCancelaAceiteAcaoAsync(geraSolicitarEvent, finalizaAceitarEvent, acaoToSave);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaAceitarEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(geraSolicitarEvent);

            return CreatedAtAction(nameof(CancelarAceitar), "OK");
        }


        /// <summary>
        /// Efetua o CANCELAMENTO GENERICO independente da condição da atividade 
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        [HttpPost]
        [Route("items/cancelargenerico/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelarGenerico(int idEmpresa, int idUsuario, int idAtividadeAcomodacao)
        {
            int idTipoSituacaoAcomodacao = 0;
            int idSituacaoAcomodacao = 0;
            int idAcomodacao = 0;

            if ((idAtividadeAcomodacao < 1) || (idEmpresa < 1) || (idUsuario < 1))
            {
                return BadRequest();
            }

            //BUSCA A ACAO ATUAL
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.dt_FimAcaoAtividade == null);

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }
            

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;

            //CONSULTA AS CHAVES
            var situacaoAcomodacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            if (situacaoAcomodacaoAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            idAcomodacao = situacaoAcomodacaoAcomodacaoToConsultar.Id_Acomodacao;

            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;
            acaoAtividadeAcomodacaoToUpdate.Id_UsuarioExecutor = idUsuario.ToString();
            _operacionalContext.AcaoItems.Update(acaoAtividadeAcomodacaoToUpdate);

            atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao = DateTime.Now;
            _operacionalContext.AtividadeItems.Update(atividadeAcomodacaoToConsultar);

            if ((idTipoSituacaoAcomodacao == (int)TipoSituacao.ALTAMEDICA) && situacaoAcomodacaoAcomodacaoToConsultar.Alta_Administrativa=="S")
            {
                //TEM ATIVIDADES
                //LISTA DE ATIVIDADES EXISTENTES
                IEnumerable<AtividadeItem> lstquery = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
               .Where(c => (c.Id_SituacaoAcomodacao == idSituacaoAcomodacao) &&
                                (c.Id_AtividadeAcomodacao != idAtividadeAcomodacao) &&
                                 (c.dt_FimAtividadeAcomodacao) == null
                                 );

                List<AtividadeItem> atividadeAcomodacaoSit = lstquery.ToList();

                //PACIENTE ATUAL
                IEnumerable<PacienteAcomodacaoItem> lstPquery = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
               .Where(c => (c.Id_Acomodacao == idAcomodacao) && (c.Dt_Saida) == null);
                List<PacienteAcomodacaoItem> PacienteAcAtual = lstPquery.ToList();

                List<PacienteItem> PacienteAtual = new List<PacienteItem>();
                if (PacienteAcAtual.Count > 0)
                {
                    //PACIENTE ATUAL
                    IEnumerable<PacienteItem> lstPacquery = _operacionalContext.PacienteItems
                    .OfType<PacienteItem>()
                   .Where(c => (c.Id_Paciente == PacienteAcAtual[0].Id_Paciente));
                    PacienteAtual = lstPacquery.ToList();
                }

                if (PacienteAcAtual.Count > 0 && atividadeAcomodacaoSit.Count == 0 && PacienteAtual[0].PendenciaFinanceira == "N")
                {
                    // RETIRA O PACIENTE DO LEITO
                    foreach (PacienteAcomodacaoItem item in PacienteAcAtual)
                    {
                        item.Dt_Saida = DateTime.Now;
                        _operacionalContext.PacienteAcomodacaoItems.Update(item);
                    }

                    //MUDA A SITUACAO
                    var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                        .OfType<SituacaoItem>()
                        .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao
                                                && c.dt_FimSituacaoAcomodacao == null);

                    situacaoAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.VAGO;


                    _operacionalContext.SituacaoItems.Update(situacaoAcomodacaoToConsultar);
                }
            }

            //Create Integration Event to be published through the Event Bus
            var finalizaEvent = new FinalizaAcaoAcomodacaoIE("CANCELAMENTO",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);
            var finalizaAtEvent = new FinalizaAtividadeAcomodacaoIE("CANCELAMENTO",
                                                                                    atividadeAcomodacaoToConsultar.Id_AtividadeAcomodacao,
                                                                                    atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndCancelaAtividadeAsync(finalizaAtEvent,finalizaEvent);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaEvent);


            return CreatedAtAction(nameof(CancelarGenerico), "OK");
        }


        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Efetua o ACEITE de uma atividade (O Aceite sempre vem de uma atividade no Status SOLICITADO.)
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        [HttpPost]
        [Route("items/aceitar/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GeraAceitar(int idEmpresa, int idUsuario, int idAtividadeAcomodacao )
        {
            if ((idAtividadeAcomodacao < 1) || (idEmpresa<1) || (idUsuario < 1) )
            {
                return BadRequest();
            }

            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();

            //VALIDA SE A ACAO ATUAL É SOLICITAR
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.Id_TipoAcaoAcomodacao == (int)TipoAcao.SOLICITAR && c.dt_FimAcaoAtividade == null);

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;
            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            int idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            int idTipoAtividadeAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao;
            int idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;
            _operacionalContext.AcaoItems.Update(acaoAtividadeAcomodacaoToUpdate);
            AcaoItem acaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idAtividadeAcomodacao, (int)TipoAcao.ACEITAR, DateTime.Now, null,  idUsuario.ToString(), idTipoAcomodacao);
            _operacionalContext.AcaoItems.Add(acaoToSave);

            //**************************************************************************************

            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();

            List<AtividadeItem> lstAt = new List<AtividadeItem>();

            //TRATA OS FLUXOS AUTOMATICOS
            //VERIFICA JA ENCAMINHADOS E ATIVIDADES JA ABERTAS
            List<ConsultarFluxoAutomaticoTO> lstFluxo = await ConsultaValidaFluxoAutomatico(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, (int)TipoAcao.ACEITAR, idEmpresa, lstAt, atividadeAcomodacaoExistentes);
            List<TipoAtividade> lstTipoFluxo = new List<TipoAtividade>();
            if (lstFluxo != null || lstFluxo.Count > 0)
            {
                foreach (ConsultarFluxoAutomaticoTO Atividade in lstFluxo)
                {
                    lstTipoFluxo.Add((TipoAtividade)Atividade.Id_TipoAtividadeAcomodacaoDestino);
                }
            }
            //GRAVA FLUXOS VALIDOS
            if (lstTipoFluxo.Count > 0)
            {
                EncaminharOut fluxoToSave = null;

                fluxoToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstTipoFluxo, idTipoAcomodacao, idAcomodacao);

                foreach (AtividadeItem Atividade in fluxoToSave.lstItem)
                {
                    _operacionalContext.AtividadeItems.Add(Atividade);
                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);
                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, idAcomodacao));
                }

            }
            //**************************************************************************************

            //Create Integration Event to be published through the Event Bus
            var finalizaSolicitarEvent = new FinalizaAcaoAcomodacaoIE("SOLICITAR",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);

            //Create Integration Event to be published through the Event Bus
            var geraAceiteEvent = new GeraAcaoAcomodacaoIE("ACEITAR",
                                                                        acaoToSave.Id_AcaoAtividadeAcomodacao , 
                                                                        acaoToSave.Id_AtividadeAcomodacao, 
                                                                        acaoToSave.Id_TipoAcaoAcomodacao, 
                                                                        acaoToSave.dt_InicioAcaoAtividade, 
                                                                        acaoToSave.dt_FimAcaoAtividade, 
                                                                        acaoToSave.Id_SLA, 
                                                                        acaoToSave.Id_UsuarioExecutor);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndGeraAcaoAsync(geraAceiteEvent, finalizaSolicitarEvent, acaoToSave, lstAtvEvento);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSolicitarEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(geraAceiteEvent);

            return CreatedAtAction(nameof(GeraAceitar), "OK");
        }


        /// <summary>
        /// Efetua o Inciar de uma atividade (O iniciar sempre vem de uma atividade no Status ACEITE.)
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        [HttpPost]
        [Route("items/iniciar/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GeraIniciar(int idEmpresa, int idUsuario, int idAtividadeAcomodacao )
        {
            if ((idAtividadeAcomodacao < 1) || (idEmpresa < 1) || (idUsuario < 1) )
            {
                return BadRequest();
            }

            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();

            //VALIDA SE A ACAO ATUAL É ACEITAR
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.Id_TipoAcaoAcomodacao == (int)TipoAcao.ACEITAR && c.dt_FimAcaoAtividade == null);

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;
            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            int idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            int idTipoAtividadeAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao;
            int idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                    .OfType<SituacaoItem>()
                    .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;
            _operacionalContext.AcaoItems.Update (acaoAtividadeAcomodacaoToUpdate);
            AcaoItem acaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idAtividadeAcomodacao, (int)TipoAcao.INICIAR , DateTime.Now, null, idUsuario.ToString(), idTipoAcomodacao);
            _operacionalContext.AcaoItems.Add(acaoToSave);


            //**************************************************************************************

            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();

            List<AtividadeItem> lstAt = new List<AtividadeItem>();

            //TRATA OS FLUXOS AUTOMATICOS
            //VERIFICA JA ENCAMINHADOS E ATIVIDADES JA ABERTAS
            List<ConsultarFluxoAutomaticoTO> lstFluxo = await ConsultaValidaFluxoAutomatico(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, (int)TipoAcao.INICIAR, idEmpresa, lstAt, atividadeAcomodacaoExistentes);
            List<TipoAtividade> lstTipoFluxo = new List<TipoAtividade>();
            if (lstFluxo != null || lstFluxo.Count > 0)
            {
                foreach (ConsultarFluxoAutomaticoTO Atividade in lstFluxo)
                {
                    lstTipoFluxo.Add((TipoAtividade)Atividade.Id_TipoAtividadeAcomodacaoDestino);
                }
            }
            //GRAVA FLUXOS VALIDOS
            if (lstTipoFluxo.Count > 0)
            {
                EncaminharOut fluxoToSave = null;

                fluxoToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstTipoFluxo, idTipoAcomodacao, idAcomodacao);

                foreach (AtividadeItem Atividade in fluxoToSave.lstItem)
                {
                    _operacionalContext.AtividadeItems.Add(Atividade);
                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);
                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, idAcomodacao));
                }

            }
            //**************************************************************************************


            //Create Integration Event to be published through the Event Bus
            var finalizaAceitarEvent = new FinalizaAcaoAcomodacaoIE("ACEITAR",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);

            //Create Integration Event to be published through the Event Bus
            var geraInicioEvent = new GeraAcaoAcomodacaoIE("INICIAR",
                                                                        acaoToSave.Id_AcaoAtividadeAcomodacao,
                                                                        acaoToSave.Id_AtividadeAcomodacao,
                                                                        acaoToSave.Id_TipoAcaoAcomodacao,
                                                                        acaoToSave.dt_InicioAcaoAtividade,
                                                                        acaoToSave.dt_FimAcaoAtividade,
                                                                        acaoToSave.Id_SLA,
                                                                        acaoToSave.Id_UsuarioExecutor);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndGeraAcaoAsync(geraInicioEvent, finalizaAceitarEvent, acaoToSave, lstAtvEvento);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(geraInicioEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaAceitarEvent);



            return CreatedAtAction(nameof(GeraIniciar), "OK");
        }


        /// <summary>
        /// Efetua o Finalizar Totalmente de uma atividade (O Finalizar Totalmente sempre vem de uma atividade no Status INICIAR.)
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        /// <param name="lstRespostas">
        /// Lista de respostas referente ao questionário
        /// </param>
        /// <param name="lstAtividadeEncaminhamento">
        /// Lista de encaminhamentos de atividades
        /// </param>
        [HttpPost]
        [Route("items/finalizartotalmente/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GeraFinalizarTotalmente(int idEmpresa, int idUsuario, int idAtividadeAcomodacao, [FromBody] List<RespostasChecklistItem> lstRespostas, List<TipoAtividade> lstAtividadeEncaminhamento)
        {
          

            if ((idAtividadeAcomodacao < 1) || (idEmpresa < 1) || (idUsuario < 1))
            {
                return BadRequest();
            }

           

            //VALIDA SE A ACAO ATUAL É INICIAR
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => (c.Id_AtividadeAcomodacao == idAtividadeAcomodacao) && (c.dt_FimAcaoAtividade == null) && ((c.Id_TipoAcaoAcomodacao == (int)TipoAcao.FINALIZAR_PARCIALMENTE) || (c.Id_TipoAcaoAcomodacao == (int)TipoAcao.INICIAR)));

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.dt_FimAtividadeAcomodacao == null);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;
            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            int idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;
            int idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            int idTipoAtividadeAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao;

            var situacaoAcomodacaoToView = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToView.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O CHECKLIST DA SITUACAO
            bool retorno = await ValidaRespostasChecklist(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao, lstRespostas);
            if (!retorno)
            {
                string msgStatus = _localizer["VALIDA_RESPOSTAS"];
                return BadRequest(msgStatus);
            }

            //FINALIZA A ACAO
            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;

            //GERA A ACAO FINALIZAR
            AcaoItem acaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idAtividadeAcomodacao, (int)TipoAcao.FINALIZAR_TOTALMENTE, DateTime.Now, DateTime.Now, idUsuario.ToString(), idTipoAcomodacao);
            _operacionalContext.AcaoItems.Add(acaoToSave);

            //BUSCA OS FLUXOS CHECK
            List<ConsultarFluxoAutomaticoCheckTO> fluxoCheckToView = await ConsultaFluxoCheckAsync(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao);

            //REGISTRA AS RESPOSTAS
            List<IntegrationEvent> lstEvt = new List<IntegrationEvent>();
            List<TipoAtividade> lstEncaminharCheck = new List<TipoAtividade>();
            List<LogCheck> listCheckLog = new List<LogCheck>();
            foreach (RespostasChecklistItem Resposta in lstRespostas)
            {
                string ver;
                if (Resposta.Valor == "S")
                    ver = "V";
                else
                    ver = "F";


                List<ConsultarFluxoAutomaticoCheckTO> result = fluxoCheckToView.FindAll(item => item.Id_Checklist == Resposta.Id_Checklist && item.Id_ItemChecklist == Resposta.Id_ItemChecklist && item.Cod_Resposta == ver);
                if (result != null && result.Count > 0)
                {
                    foreach (ConsultarFluxoAutomaticoCheckTO itloc in result)
                    {
                        lstEncaminharCheck.Add((TipoAtividade)itloc.Id_TipoAtividadeAcomodacao);
                        LogCheck itLog = new LogCheck(Resposta.Id_ItemChecklist, (TipoAtividade)itloc.Id_TipoAtividadeAcomodacao);
                        listCheckLog.Add(itLog);
                    }
                }
                lstEvt.Add(new RespostaChecklistSaveIE(Resposta.Id_Checklist, Resposta.Id_ItemChecklist, Resposta.Valor, Resposta.Id_AtividadeAcomodacao, 0, Resposta.Id_CheckTSTAT));
                //_operacionalContext.RespostasChecklistItems.Add(Resposta);
            }

            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();

            EncaminharOut encaminharToSave = new EncaminharOut();

            //VALIDA COLISAO ENCAMINHAMENTO COM ATIVIDADES EXISTENTES E CHECKLOG
            LogAtividade lstEncaminhar = new LogAtividade();
            lstEncaminhar = await ConsultaValidaEncaminhamento(lstAtividadeEncaminhamento, atividadeAcomodacaoExistentes, lstEncaminharCheck, listCheckLog);
            ////////////////////////////////////////////////////////////////////
            if (lstEncaminhar.LstAt.Count() > 0)
            {
                //ENCAMINHAR
                encaminharToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstEncaminhar.LstAt, idTipoAcomodacao, idAcomodacao);

                foreach (AtividadeItem Atividade in encaminharToSave.lstItem)
                {

                    var result = listCheckLog.Find(item => (int)item.TipoAtv == Atividade.Id_TipoAtividadeAcomodacao);
                    if (result != null)
                    {
                        var res = lstRespostas.Find(itemR => itemR.Id_ItemChecklist == result.Log);
                        if (result != null)
                        {
                            List<CheckRespostaAtividadeItem> lstCheck = new List<CheckRespostaAtividadeItem>();
                            CheckRespostaAtividadeItem checkLog = new CheckRespostaAtividadeItem();
                            lstCheck.Add(checkLog);
                            Atividade.CheckRespostaAtividadeItems = lstCheck;
                            if (res != null)
                            {
                                res.CheckRespostaAtividadeItems = lstCheck;
                                _operacionalContext.RespostasChecklistItems.Add(res);
                                lstRespostas.Remove(res);
                            }
                        }
                    }
                    _operacionalContext.AtividadeItems.Add(Atividade);
                }

                foreach (RespostasChecklistItem rp in lstRespostas)
                {
                    _operacionalContext.RespostasChecklistItems.Add(rp);
                    //lstRespostas.Remove(rp);
                }

            }

            //FINALIZA ATIVIDADE
            atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao = DateTime.Now;
            _operacionalContext.AtividadeItems.Update(atividadeAcomodacaoToConsultar);        

            //TRATA OS FLUXOS AUTOMATICOS
            //VERIFICA JA ENCAMINHADOS E ATIVIDADES JA ABERTAS
            List<ConsultarFluxoAutomaticoTO> lstFluxo =  await ConsultaValidaFluxoAutomatico(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao,(int) TipoAcao.FINALIZAR_TOTALMENTE,idEmpresa, encaminharToSave.lstItem, atividadeAcomodacaoExistentes);
            List<TipoAtividade> lstTipoFluxo = new List<TipoAtividade>();
            if (lstFluxo != null || lstFluxo.Count>0)
            {
                foreach (ConsultarFluxoAutomaticoTO Atividade in lstFluxo)
                {
                    lstTipoFluxo.Add((TipoAtividade)Atividade.Id_TipoAtividadeAcomodacaoDestino);
                }
            }
            //GRAVA FLUXOS VALIDOS
            if (lstTipoFluxo.Count > 0)            
            {
                EncaminharOut fluxoToSave = null;

                fluxoToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstTipoFluxo, idTipoAcomodacao, idAcomodacao);

                foreach (AtividadeItem Atividade in fluxoToSave.lstItem)
                {                    
                    _operacionalContext.AtividadeItems.Add(Atividade);
                    encaminharToSave.lstItem.Add(Atividade);
                }

                foreach (IntegrationEvent evt in fluxoToSave.lstEvt)
                {//Adicinonando na mesma lista de eventos encaminhados
                    encaminharToSave.lstEvt.Add(evt);
                }
            }

            if (lstEncaminhar.LstAt == null)
            {
                lstEncaminhar.LstAt = new List<TipoAtividade>();
            }

            // COMENTADO APÓS SOLICITAÇÃO VIA CARD 
            // https://trello.com/c/DLbMSg5o/95-gest%C3%A3o-de-leitos-finaliza%C3%A7%C3%A3o-atividade
            if (situacaoAcomodacaoToView.Alta_Administrativa == "S")
            {
                if ((idTipoSituacaoAcomodacao == (int)TipoSituacao.ALTAMEDICA) && (lstTipoFluxo.Count == 0) && (lstEncaminhar.LstAt.Count == 0))
                {
                    //TEM ATIVIDADES
                    //LISTA DE ATIVIDADES EXISTENTES
                    IEnumerable<AtividadeItem> lstquery = _operacionalContext.AtividadeItems
                    .OfType<AtividadeItem>()
                   .Where(c => (c.Id_SituacaoAcomodacao == idSituacaoAcomodacao) &&
                                    (c.Id_AtividadeAcomodacao != idAtividadeAcomodacao) &&
                                     (c.dt_FimAtividadeAcomodacao) == null
                                     );

                    List<AtividadeItem> atividadeAcomodacaoSit = lstquery.ToList();

                    //PACIENTE ATUAL
                    IEnumerable<PacienteAcomodacaoItem> lstPquery = _operacionalContext.PacienteAcomodacaoItems
                    .OfType<PacienteAcomodacaoItem>()
                   .Where(c => (c.Id_Acomodacao == idAcomodacao) && (c.Dt_Saida) == null);
                    List<PacienteAcomodacaoItem> PacienteAcAtual = lstPquery.ToList();

                    List<PacienteItem> PacienteAtual = new List<PacienteItem>();
                    if (PacienteAcAtual.Count > 0)
                    {
                        //PACIENTE ATUAL
                        IEnumerable<PacienteItem> lstPacquery = _operacionalContext.PacienteItems
                        .OfType<PacienteItem>()
                       .Where(c => (c.Id_Paciente == PacienteAcAtual[0].Id_Paciente));
                        PacienteAtual = lstPacquery.ToList();
                    }

                    if (PacienteAcAtual.Count > 0 && atividadeAcomodacaoSit.Count == 0 && PacienteAtual[0].PendenciaFinanceira == "N")
                    {
                        // RETIRA O PACIENTE DO LEITO
                        foreach (PacienteAcomodacaoItem item in PacienteAcAtual)
                        {
                            item.Dt_Saida = DateTime.Now;
                            _operacionalContext.PacienteAcomodacaoItems.Update(item);
                        }

                        //MUDA A SITUACAO
                        var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                            .OfType<SituacaoItem>()
                            .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao
                                                    && c.dt_FimSituacaoAcomodacao == null);

                        situacaoAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.VAGO;


                        _operacionalContext.SituacaoItems.Update(situacaoAcomodacaoToConsultar);
                    }
                }
            }



            //Create Integration Event to be published through the Event Bus

            var finalizaAtividadeEvent = new FinalizaAtividadeAcomodacaoIE("INICIAR OU PARC",
                                                            atividadeAcomodacaoToConsultar.Id_AtividadeAcomodacao,
                                                            atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao);


            var finalizaIniciarEvent = new FinalizaAcaoAcomodacaoIE("INICIAR OU PARC",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);

            //Create Integration Event to be published through the Event Bus
            var geraFinalizarTotEvent = new GeraAcaoAcomodacaoIE("FINALIZAR_TOTALMENTE",
                                                                        acaoToSave.Id_AcaoAtividadeAcomodacao,
                                                                        acaoToSave.Id_AtividadeAcomodacao,
                                                                        acaoToSave.Id_TipoAcaoAcomodacao,
                                                                        acaoToSave.dt_InicioAcaoAtividade,
                                                                        acaoToSave.dt_FimAcaoAtividade,
                                                                        acaoToSave.Id_SLA,
                                                                        acaoToSave.Id_UsuarioExecutor);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndGeraAcaoFinalizarTotalAsync(geraFinalizarTotEvent, finalizaIniciarEvent, finalizaAtividadeEvent,acaoToSave, encaminharToSave.lstEvt, encaminharToSave.lstItem, lstEvt, lstRespostas);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // Publish through the Event Bus and mark the saved event as published

            //Encaminhamentos / Fluxos
            if (encaminharToSave.lstEvt.Count  > 0)
            {
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(encaminharToSave.lstEvt);
            }

   

            return CreatedAtAction(nameof(GeraFinalizarTotalmente), "OK");
        }



        /// <summary>
        /// Efetua o Finalizar Parcialmente de uma atividade (O Finalizar Parcialmente sempre vem de uma atividade no Status INICIAR.)
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario executor
        /// </param>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        /// <param name="lstRespostas">
        /// Lista de respostas referente ao questionário
        /// </param>
        /// <param name="lstAtividadeEncaminhamento">
        /// Lista de encaminhamentos de atividades
        /// </param>
        [HttpPost]
        [Route("items/finalizarparcialmente/empresa/{idEmpresa}/usuario/{idUsuario}/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GeraFinalizarParcialmente(int idEmpresa, int idUsuario, int idAtividadeAcomodacao, [FromBody] List<RespostasChecklistItem> lstRespostas, List<TipoAtividade> lstAtividadeEncaminhamento)
        {
            if ((idAtividadeAcomodacao < 1) || (idEmpresa < 1) || (idUsuario < 1))
            {
                return BadRequest();
            }


            //VALIDA SE A ACAO ATUAL É INICIAR
            var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && (c.Id_TipoAcaoAcomodacao == (int)TipoAcao.INICIAR) && c.dt_FimAcaoAtividade == null);

            if (acaoAtividadeAcomodacaoToUpdate is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            //CONSULTA AS CHAVES
            var atividadeAcomodacaoToConsultar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == idAtividadeAcomodacao && c.dt_FimAtividadeAcomodacao == null);

            if (atividadeAcomodacaoToConsultar is null)
            {
                string msgStatus = _localizer["VALIDA_STATUS"];
                return BadRequest(msgStatus);
            }

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;
            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            int idSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_SituacaoAcomodacao;
            int idTipoSituacaoAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoSituacaoAcomodacao;
            int idTipoAtividadeAcomodacao = atividadeAcomodacaoToConsultar.Id_TipoAtividadeAcomodacao;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //FINALIZA A ACAO
            acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;

            //GERA A ACAO FINALIZAR
            AcaoItem acaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idAtividadeAcomodacao, (int)TipoAcao.FINALIZAR_PARCIALMENTE, DateTime.Now, DateTime.Now, idUsuario.ToString(), idTipoAcomodacao);
            _operacionalContext.AcaoItems.Add(acaoToSave);

            //BUSCA OS FLUXOS CHECK
            List<ConsultarFluxoAutomaticoCheckTO> fluxoCheckToView = await ConsultaFluxoCheckAsync(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao);

            //REGISTRA AS RESPOSTAS
            List<IntegrationEvent> lstEvt = new List<IntegrationEvent>();
            List<TipoAtividade> lstEncaminharCheck = new List<TipoAtividade>();
            List<LogCheck> listCheckLog = new List<LogCheck>();
            foreach (RespostasChecklistItem Resposta in lstRespostas)
            {
                string ver;
                if (Resposta.Valor == "S")
                    ver = "V";
                else
                    ver = "F";

                
                List<ConsultarFluxoAutomaticoCheckTO> result = fluxoCheckToView.FindAll(item => item.Id_Checklist == Resposta.Id_Checklist && item.Id_ItemChecklist == Resposta.Id_ItemChecklist && item.Cod_Resposta == ver);
                if (result != null && result.Count > 0)
                {
                    foreach (ConsultarFluxoAutomaticoCheckTO itloc in result)
                    {
                        lstEncaminharCheck.Add((TipoAtividade)itloc.Id_TipoAtividadeAcomodacao);
                        LogCheck itLog = new LogCheck(Resposta.Id_ItemChecklist, (TipoAtividade)itloc.Id_TipoAtividadeAcomodacao);
                        listCheckLog.Add(itLog);
                    }
                }
                
                lstEvt.Add(new RespostaChecklistSaveIE(Resposta.Id_Checklist, Resposta.Id_ItemChecklist, Resposta.Valor, Resposta.Id_AtividadeAcomodacao, 0, Resposta.Id_CheckTSTAT));
                //_operacionalContext.RespostasChecklistItems.Add(Resposta);
            }

            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == idSituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();

            EncaminharOut encaminharToSave = new EncaminharOut();


            //VALIDA COLISAO ENCAMINHAMENTO COM ATIVIDADES EXISTENTES  E CHECKLOG
            LogAtividade lstEncaminhar = new LogAtividade();
            lstEncaminhar = await ConsultaValidaEncaminhamento(lstAtividadeEncaminhamento, atividadeAcomodacaoExistentes, lstEncaminharCheck,listCheckLog);
            ////////////////////////////////////////////////////////////////////
            if (lstEncaminhar.LstAt.Count() > 0)
            {
                //ENCAMINHAR
                encaminharToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstEncaminhar.LstAt, idTipoAcomodacao, idAcomodacao);
                
                foreach (AtividadeItem Atividade in encaminharToSave.lstItem)
                {

                    var result = listCheckLog.Find(item => (int)item.TipoAtv == Atividade.Id_TipoAtividadeAcomodacao);
                    if (result != null)
                    {
                        var res = lstRespostas.Find(itemR => itemR.Id_ItemChecklist == result.Log);
                        if (result != null)
                        {
                            List<CheckRespostaAtividadeItem> lstCheck = new List<CheckRespostaAtividadeItem>();
                            CheckRespostaAtividadeItem checkLog = new CheckRespostaAtividadeItem();
                            lstCheck.Add(checkLog);
                            Atividade.CheckRespostaAtividadeItems = lstCheck;
                            if (res != null)
                            {
                                res.CheckRespostaAtividadeItems = lstCheck;
                                _operacionalContext.RespostasChecklistItems.Add(res);
                                lstRespostas.Remove(res);
                            }
                        }
                    }

                    _operacionalContext.AtividadeItems.Add(Atividade);
                }
                foreach (RespostasChecklistItem rp in lstRespostas)
                {
                    _operacionalContext.RespostasChecklistItems.Add(rp);
                    //lstRespostas.Remove(rp);
                }
            }

            //FINALIZA ATIVIDADE
            atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao = DateTime.Now;
            _operacionalContext.AtividadeItems.Update(atividadeAcomodacaoToConsultar);

            //TRATA OS FLUXOS AUTOMATICOS
            //VERIFICA JA ENCAMINHADOS E ATIVIDADES JA ABERTAS
            List<ConsultarFluxoAutomaticoTO> lstFluxo = await ConsultaValidaFluxoAutomatico(ConfiguracaoURL, tokenURL, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, (int)TipoAcao.FINALIZAR_PARCIALMENTE, idEmpresa, encaminharToSave.lstItem, atividadeAcomodacaoExistentes);
            List<TipoAtividade> lstTipoFluxo = new List<TipoAtividade>();
            if (lstFluxo != null || lstFluxo.Count > 0)
            {
                foreach (ConsultarFluxoAutomaticoTO Atividade in lstFluxo)
                {
                    lstTipoFluxo.Add((TipoAtividade)Atividade.Id_TipoAtividadeAcomodacaoDestino);
                }
            }
            //GRAVA FLUXOS VALIDOS
            if (lstTipoFluxo.Count > 0)
            {
                EncaminharOut fluxoToSave = null;

                fluxoToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeAcomodacaoToConsultar, lstTipoFluxo, idTipoAcomodacao, idAcomodacao);

                foreach (AtividadeItem Atividade in fluxoToSave.lstItem)
                {
                    _operacionalContext.AtividadeItems.Add(Atividade);
                    encaminharToSave.lstItem.Add(Atividade);
                }

                foreach (IntegrationEvent evt in fluxoToSave.lstEvt)
                {//Adicinonando na mesma lista de eventos encaminhados
                    encaminharToSave.lstEvt.Add(evt);
                }
            }


            var finalizaAtividadeEvent = new FinalizaAtividadeAcomodacaoIE("INICIAR OU PARC",
                                                atividadeAcomodacaoToConsultar.Id_AtividadeAcomodacao,
                                                atividadeAcomodacaoToConsultar.dt_FimAtividadeAcomodacao);

            //Create Integration Event to be published through the Event Bus
            var finalizaIniciarEvent = new FinalizaAcaoAcomodacaoIE("INICIAR OU PARC",
                                                                        acaoAtividadeAcomodacaoToUpdate.Id_AcaoAtividadeAcomodacao,
                                                                        acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade);

            //Create Integration Event to be published through the Event Bus
            var geraFinalizarParcEvent = new GeraAcaoAcomodacaoIE("FINALIZAR_PARCIALMENTE",
                                                                        acaoToSave.Id_AcaoAtividadeAcomodacao,
                                                                        acaoToSave.Id_AtividadeAcomodacao,
                                                                        acaoToSave.Id_TipoAcaoAcomodacao,
                                                                        acaoToSave.dt_InicioAcaoAtividade,
                                                                        acaoToSave.dt_FimAcaoAtividade,
                                                                        acaoToSave.Id_SLA,
                                                                        acaoToSave.Id_UsuarioExecutor);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndGeraAcaoFinalizarAsync(geraFinalizarParcEvent, finalizaIniciarEvent, finalizaAtividadeEvent, acaoToSave, encaminharToSave.lstEvt, encaminharToSave.lstItem, lstEvt, lstRespostas);


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(geraFinalizarParcEvent);
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaIniciarEvent);

            if (lstAtividadeEncaminhamento.Count() > 0)
            {
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(encaminharToSave.lstEvt);
            }


            return CreatedAtAction(nameof(GeraFinalizarParcialmente), "OK");
        }



        /// <summary>
        /// Inclui uma acao.
        /// </summary>
        /// <param name="acaoToSave">
        /// Objeto que representa a acao da acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/acao")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirAcao([FromBody]AcaoItem acaoToSave)
        {
            string msgRule = "";

            var acaoToValidate = _operacionalContext.AcaoItems
                        .OfType<AcaoItem>()
                        .SingleOrDefault(e => e.Id_AtividadeAcomodacao == acaoToSave.Id_AtividadeAcomodacao && e.Id_TipoAcaoAcomodacao == acaoToSave.Id_TipoAcaoAcomodacao && e.dt_FimAcaoAtividade == null);

            if (acaoToValidate != null)
            {

                string msgStatus = _localizer["VALIDA_ATIVIDADEATIVA"];
                return BadRequest(msgStatus);
            }

            _operacionalContext.AcaoItems.Add(acaoToSave);

            //Create Integration Event to be published through the Event Bus
            var acaoSaveEvent = new AcaoSaveIE(acaoToSave.Id_AcaoAtividadeAcomodacao ,
                                                                        acaoToSave.Id_AtividadeAcomodacao,
                                                                        acaoToSave.Id_TipoAcaoAcomodacao,
                                                                        acaoToSave.dt_InicioAcaoAtividade,
                                                                        acaoToSave.dt_FimAcaoAtividade,
                                                                        acaoToSave.Id_SLA,
                                                                        acaoToSave.Id_UsuarioExecutor );

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAcaoContextChangesAsync(acaoSaveEvent, acaoToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(acaoSaveEvent);


            return CreatedAtAction(nameof(IncluirAcao), acaoToSave.Id_AcaoAtividadeAcomodacao);
        }

    }




}