using Administrativo.API.TO;
using Configuracao.API.TO;
using EventBus.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Operacional.API.Infrastructure;
using Operacional.API.IntegrationEvents;
using Operacional.API.IntegrationEvents.Events;
using Operacional.API.Model;
using Operacional.API.Utilitarios;
using Operacional.API.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Operacional.API.Enum.ExpoEnum;
using static Operacional.API.Utilitarios.Util;

namespace Operacional.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegracaoController : ControllerBase
    {

        private const string cachePrefix = "INTEGRACAO#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<IntegracaoController> _localizer;

        public IntegracaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<IntegracaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Inclui / atualiza um Paciente.
        /// </summary>
        /// <param name="NomePaciente">
        /// Nome do paciente (100)
        /// </param>
        /// <param name="CodExterno">
        /// Identificador externo do paciente para finalidade de integração (10)
        /// </param>
        /// <param name="DataNascimentoPaciente">
        /// Data de Nascimento do Paciente YYYY-MM-DD 00:00:00
        /// </param>
        /// <param name="GeneroPaciente">
        /// Genero do Paciente (1)
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items/paciente/codexterno/{CodExterno}/nomepaciente/{NomePaciente}/genero/{GeneroPaciente}/datanascimento/{DataNascimentoPaciente}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarPaciente(string CodExterno,
                                                        string NomePaciente,
                                                        DateTime DataNascimentoPaciente, 
                                                        string GeneroPaciente
            )
        {
            //AREA DE VALIDACAO
            //FIM AREA DE VALIDACAO

            var pacienteToValidate = _operacionalContext.PacienteItems
                                    .OfType<PacienteItem>()
                                    .SingleOrDefault  (e => e.Cod_Externo == CodExterno);

            if (pacienteToValidate!=null)
            {
                pacienteToValidate.Nome_Paciente = NomePaciente;
                pacienteToValidate.Dt_NascimentoPaciente= DataNascimentoPaciente;
                pacienteToValidate.GeneroPaciente= GeneroPaciente;
                _operacionalContext.PacienteItems.Update(pacienteToValidate);
            }
            else
            {
                PacienteItem pacienteToSave = new PacienteItem();
                pacienteToSave.Nome_Paciente = NomePaciente;
                pacienteToSave.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToSave.GeneroPaciente = GeneroPaciente;
                pacienteToSave.Cod_Externo = CodExterno;
                _operacionalContext.PacienteItems.Add(pacienteToSave);

                pacienteToValidate = pacienteToSave;
            }

            //Create Integration Event to be published through the Event Bus
            var pacienteSaveEvent = new PacienteSaveIE(pacienteToValidate.Id_Paciente ,
                                                                    pacienteToValidate.Nome_Paciente,
                                                                    pacienteToValidate.Cod_Externo ,
                                                                    pacienteToValidate.GeneroPaciente ,
                                                                    pacienteToValidate.Dt_NascimentoPaciente,
                                                                    pacienteToValidate.PendenciaFinanceira);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndPacienteContextChangesAsync(pacienteSaveEvent, pacienteToValidate);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
                
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(pacienteSaveEvent);


            return CreatedAtAction(nameof(SalvarPaciente), pacienteToValidate.Id_Paciente);
        }

        /// <summary>
        /// Integração da operação CANCELAMENTO RESERVA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA SE TEM O PACIENTE ATIVO
        /// 03) VALIDA ACOMODACAO STATUS RESERVA
        /// 04) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        [HttpPost]
        [Route("items/cancelamentoreserva/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/codexternopaciente/{CodExternoPaciente}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelamentoReserva(int idEmpresa,
                                                    string codExternoAcomodacao,
                                                    string CodExternoPaciente
                                                    )
        {

            string NumAtendimento;
            NumAtendimento = "INTERNO";
            int IdAcomodacao;
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao) 
                || (string.IsNullOrEmpty(CodExternoPaciente))))
            {
                return BadRequest();
            }


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count == 0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }

            //VERIFICA SE TEM O PACIENTE ATIVO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente != pacienteToExist.Id_Paciente);

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate != null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADACOMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //VERIFICA SE TEM O PACIENTE ATIVO
            var pacienteAtual = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == pacienteToExist.Id_Paciente);

            //VALIDA SE A ACOMODACAO TEM O PACIENTE ATIVO
            if (pacienteAtual == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADANAOPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE A SITUACAO DA ACOMODACAO É RESERVA
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.RESERVADO)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAORESERVA"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.VAGO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }

            //ATUALIZA O PACIENTE NA ACOMODACAO
            pacienteAtual.Dt_Saida =  DateTime.Now;
            _operacionalContext.PacienteAcomodacaoItems.Update(pacienteAtual);

            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = "0";
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;
            situacaoToSave.PacienteItem = null;
            //Trata Sla Nulo
            if (IdSLA > 0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.VAGO ;

            _operacionalContext.SituacaoItems.Add(situacaoToSave);



            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                situacaoToUpdate.dt_FimSituacaoAcomodacao,
                situacaoToUpdate.cod_NumAtendimento,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndCancelamentoReservaAsync(saveNewSituacaoEvent, finalizaSituacaoEvent,  situacaoToSave);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }

            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);

            return CreatedAtAction(nameof(CancelamentoReserva), "OK");
        }

        /// <summary>
        /// Integração da operação RESERVA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA SE TEM PACIENTE ATIVO
        /// 03) VALIDA ACOMODACAO STATUS VAGO
        /// 04) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// 05) VERIFICA SE O PACIENTE ESTÁ INTERNADO.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="NomePaciente">
        /// Nome do Paciente
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="GeneroPaciente">
        /// Genero do Paciente
        /// </param>
        /// <param name="DataNascimentoPaciente">
        /// Data de Nascimento do Paciente YYYY-MM-DD 00:00:00
        /// </param>
        [HttpPost]
        [Route("items/reserva/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/nomepaciente/{NomePaciente}/codexternopaciente/{CodExternoPaciente}/genero/{GeneroPaciente}/datanascimento/{DataNascimentoPaciente}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Reserva(int idEmpresa,
                                                    string codExternoAcomodacao,
                                                    string NomePaciente,
                                                    string CodExternoPaciente,              
                                                    string GeneroPaciente,
                                                    DateTime DataNascimentoPaciente
                                                    )
        {

            string NumAtendimento;
            NumAtendimento = "INTERNO";
            int IdAcomodacao;
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao) || (string.IsNullOrEmpty(NomePaciente))
                || (string.IsNullOrEmpty(CodExternoPaciente)) 
                || (string.IsNullOrEmpty(GeneroPaciente))))
            {
                return BadRequest();
            }


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //VERIFICA SE TEM PACIENTE ATIVO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null);

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate != null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADACOMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null );

            //VALIDA SE A SITUACAO DA ACOMODACAO É VAGO
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.VAGO)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAO"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.RESERVADO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE O PACIENTE JÁ ESTÀ CADASTRADO
            PacienteItem pacienteToSave;
            PacienteAcomodacaoItem pacienteAcomodacaoToSave;
            if (pacienteToExist == null)
            {
                //INCLUI PACIENTE
                pacienteToSave = new PacienteItem();
                pacienteToSave.Nome_Paciente = NomePaciente;
                pacienteToSave.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToSave.GeneroPaciente = GeneroPaciente;
                pacienteToSave.Cod_Externo = CodExternoPaciente;

                List<PacienteAcomodacaoItem> lstPacienteAcomodacao = new List<PacienteAcomodacaoItem>();

                pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
                pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacao;
                pacienteAcomodacaoToSave.NumAtendimento = NumAtendimento;
                pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
                pacienteAcomodacaoToSave.Dt_Saida = null;

                lstPacienteAcomodacao.Add(pacienteAcomodacaoToSave);

                pacienteToSave.PacienteAcomodacaoItems = lstPacienteAcomodacao;

                _operacionalContext.PacienteItems.Add(pacienteToSave);

            }
            else
            {

                var pacienteToValid = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Paciente == pacienteToExist.Id_Paciente && c.Dt_Saida == null);

                //VERIFICA SE O PACIENTE ESTÁ COM DT SAIDA NULA OU SEJA INTERNADO.
                if (pacienteToValid != null)
                {
                    string msgStatus = _localizer["VALIDA_PACIENTEINTERNADO"];
                    return BadRequest(msgStatus);
                }


                pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
                pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacao;
                pacienteAcomodacaoToSave.NumAtendimento = NumAtendimento;
                pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
                pacienteAcomodacaoToSave.Dt_Saida = null;
                pacienteAcomodacaoToSave.Id_Paciente = pacienteToExist.Id_Paciente;

                _operacionalContext.PacienteAcomodacaoItems.Add(pacienteAcomodacaoToSave);

                pacienteToExist.Nome_Paciente = NomePaciente;
                pacienteToExist.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToExist.GeneroPaciente = GeneroPaciente;

                _operacionalContext.PacienteItems.Update(pacienteToExist);

                pacienteToSave = pacienteToExist;
            }


            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = NumAtendimento;
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;
            if (pacienteToExist == null) { situacaoToSave.PacienteItem = pacienteToSave; }
            else { situacaoToSave.PacienteItem = pacienteToExist; };
            if (IdSLA>0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.RESERVADO;

            _operacionalContext.SituacaoItems.Add(situacaoToSave);



            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                situacaoToUpdate.dt_FimSituacaoAcomodacao,
                situacaoToUpdate.cod_NumAtendimento,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);


            var saveNewPacienteEvent = new PacienteSaveIE(pacienteToSave.Id_Paciente,
                pacienteToSave.Nome_Paciente,
                pacienteToSave.Cod_Externo,
                pacienteToSave.GeneroPaciente,
                pacienteToSave.Dt_NascimentoPaciente,
                pacienteToSave.PendenciaFinanceira);

            var saveNewPacienteAcomodacaoEvent = new PacienteAcomodacaoSaveIE(pacienteAcomodacaoToSave.Id_PacienteAcomodacao,
                pacienteAcomodacaoToSave.Id_Paciente,
                pacienteAcomodacaoToSave.Dt_Entrada,
                pacienteAcomodacaoToSave.Dt_Saida,
                pacienteAcomodacaoToSave.Id_Acomodacao,
                pacienteAcomodacaoToSave.NumAtendimento);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndInternacaoAsync(saveNewPacienteEvent, saveNewPacienteAcomodacaoEvent, saveNewSituacaoEvent, finalizaSituacaoEvent, pacienteToSave, situacaoToSave);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteAcomodacaoEvent);

            return CreatedAtAction(nameof(Reserva), "OK");
        }

        /// <summary>
        /// Integração da operação CANCELAMENTO ALTA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA O PACIENTE EXISTE
        /// 03) VERIFICA SE TEM O PACIENTE ATIVO NA ACOMODACAO
        /// 04) VALIDA ACOMODACAO STATUS ALTA MEDICA
        /// 05) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="NumAtendimento">
        /// Numero de Atendimento para integração.
        /// </param>
        [HttpPost]
        [Route("items/cancelamentoalta/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/codexternopaciente/{CodExternoPaciente}/numatendimento/{NumAtendimento}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelamentoAlta(int idEmpresa,
                                                string codExternoAcomodacao,
                                                string CodExternoPaciente,
                                                string NumAtendimento
                                                )
        {
            //AREA DE VALIDACAO
            int IdAcomodacao;
            //!(DateTime.TryParse(DataNascimentoPaciente, out dDate)
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao)
                || (string.IsNullOrEmpty(CodExternoPaciente)) || (string.IsNullOrEmpty(NumAtendimento))))
            {
                return BadRequest();
            }

            //FIM AREA DE VALIDACAO
            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }


            //CONSULTA SE O PACIENTE ESTÁ ATIVO NA ACOMODACAO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == pacienteToExist.Id_Paciente);


            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADANAOPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE A SITUACAO DA ACOMODACAO É ALTA MEDICA
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.ALTAMEDICA)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAOALTAMEDICA"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.OCUPADO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }

            
            //CONSULTA AS ATIVIDADES DA SITUACAO DA ACOMODACAO 
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == situacaoToUpdate.Id_SituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeExistentes = query.ToList();

            foreach (AtividadeItem at in atividadeExistentes)
            {

                //CONSULTA AS ACOES DA ATIVIDADE DA SITUACAO (ALTA MEDICA) 
                IEnumerable<AcaoItem> queryAc = _operacionalContext.AcaoItems
                .OfType<AcaoItem>()
                .Where(c => c.Id_AtividadeAcomodacao == at.Id_AtividadeAcomodacao && c.dt_FimAcaoAtividade == null);
                List<AcaoItem> acaoExistentes = queryAc.ToList();

                foreach (AcaoItem ac in acaoExistentes)
                {
                    ac.dt_FimAcaoAtividade = DateTime.Now;
                    _operacionalContext.AcaoItems.Update(ac);
                }

                at.dt_FimAtividadeAcomodacao = DateTime.Now;
                _operacionalContext.AtividadeItems.Update(at);

                List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, at.Id_TipoSituacaoAcomodacao, at.Id_TipoAtividadeAcomodacao);

                lstAtvEvento.Add(Util.CriaEventoAtividade(at, Perfis, IdAcomodacao));
            }
            

            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = NumAtendimento;
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;            
            situacaoToSave.PacienteItem = pacienteToExist; 
            //Trata SLA Nulo
            if (IdSLA > 0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.OCUPADO;

            _operacionalContext.SituacaoItems.Add(situacaoToSave);


            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                situacaoToUpdate.dt_FimSituacaoAcomodacao,
                situacaoToUpdate.cod_NumAtendimento,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);



            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndCancelamentoAltaAsync( saveNewSituacaoEvent, finalizaSituacaoEvent, situacaoToSave, lstAtvEvento);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);

            return CreatedAtAction(nameof(CancelamentoAlta), "OK");
        }

        /// <summary>
        /// Integração da operação ALTA ADMINISTRATIVA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA O PACIENTE EXISTE
        /// 03) VERIFICA SE TEM O PACIENTE ATIVO NA ACOMODACAO
        /// 04) VALIDA ACOMODACAO STATUS ALTA MEDICA
        /// 05) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="NumAtendimento">
        /// Numero de Atendimento para integração.
        /// </param>
        [HttpPost]
        [Route("items/altaadministrativa/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/codexternopaciente/{CodExternoPaciente}/numatendimento/{NumAtendimento}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AltaAdministrativa(int idEmpresa,
                                                        string codExternoAcomodacao,
                                                        string CodExternoPaciente,
                                                        string NumAtendimento)
        {
            //AREA DE VALIDACAO
            int IdAcomodacao;
            //!(DateTime.TryParse(DataNascimentoPaciente, out dDate)
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao)
                || (string.IsNullOrEmpty(CodExternoPaciente)) || (string.IsNullOrEmpty(NumAtendimento))))
            {
                return BadRequest();
            }

            //FIM AREA DE VALIDACAO


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count == 0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }


            //CONSULTA SE O PACIENTE ESTÁ ATIVO NA ACOMODACAO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == pacienteToExist.Id_Paciente);


            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSEMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE A SITUACAO DA ACOMODACAO É ALTA MEDICA
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.ALTAMEDICA)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAOALTAMEDICA"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.VAGO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }


            //TEM ATIVIDADES
            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> lstquery = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
           .Where(c => (c.Id_SituacaoAcomodacao == situacaoToUpdate.Id_SituacaoAcomodacao &&
                             c.dt_FimAtividadeAcomodacao == null));

            List<AtividadeItem> atividadeAcomodacaoSit = lstquery.ToList();


            if (atividadeAcomodacaoSit.Count == 0)
            {
                //ATUALIZA A SITUACAO PASSADA
                situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
                situacaoToUpdate.Alta_Administrativa = "S";
                _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

                //INCLUI SITUACAO NOVA
                SituacaoItem situacaoToSave = new SituacaoItem();
                situacaoToSave.cod_NumAtendimento = "0";
                situacaoToSave.Cod_Prioritario = "N";
                situacaoToSave.dt_FimSituacaoAcomodacao = null;
                situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
                situacaoToSave.PacienteItem = null;     
                situacaoToSave.Id_Acomodacao = IdAcomodacao;
                
                //Trata o SLA nulo
                if (IdSLA > 0) { situacaoToSave.Id_SLA = IdSLA; }
                situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.VAGO;

                _operacionalContext.SituacaoItems.Add(situacaoToSave);

                //ATUALIZA O PACIENTE
                pacienteToValidate.Dt_Saida = DateTime.Now;
                _operacionalContext.PacienteAcomodacaoItems.Update(pacienteToValidate);

                //ATUALIZA A SITUACAO FINANCEIRA DO PACIENTE
                pacienteToExist.PendenciaFinanceira = "N";
                _operacionalContext.PacienteItems.Update(pacienteToExist);

                //Create Integration Event to be published through the Event Bus
                var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                    situacaoToUpdate.Id_Acomodacao,
                    situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                    situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                    situacaoToUpdate.dt_FimSituacaoAcomodacao,
                    situacaoToUpdate.cod_NumAtendimento,
                    situacaoToUpdate.Id_SLA,
                    situacaoToUpdate.Cod_Prioritario, null);


                var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                    situacaoToSave.Id_Acomodacao,
                    situacaoToSave.Id_TipoSituacaoAcomodacao,
                    situacaoToSave.dt_InicioSituacaoAcomodacao,
                    situacaoToSave.dt_FimSituacaoAcomodacao,
                    situacaoToSave.cod_NumAtendimento,
                    situacaoToSave.Id_SLA,
                    situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);

                var savePacienteAcomodacaoEvent = new PacienteAcomodacaoSaveIE(pacienteToValidate.Id_PacienteAcomodacao,
                    pacienteToValidate.Id_Paciente,
                    pacienteToValidate.Dt_Entrada,
                    pacienteToValidate.Dt_Saida,
                    pacienteToValidate.Id_Acomodacao,
                    pacienteToValidate.NumAtendimento);

                try
                {
                    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _operacionalIntegrationEventService.SaveEventAndAltaHospitalarAsync(savePacienteAcomodacaoEvent, saveNewSituacaoEvent, finalizaSituacaoEvent, situacaoToSave);
                }
                catch (Exception e)
                {
                    ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                    Dberro.Main(e);
                    return BadRequest(e.Message);
                }
                // Publish through the Event Bus and mark the saved event as published
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
                // Publish through the Event Bus and mark the saved event as published
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);
                // Publish through the Event Bus and mark the saved event as published
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(savePacienteAcomodacaoEvent);
            }
            else
            {
                //ATUALIZA A SITUACAO PASSADA
                situacaoToUpdate.Alta_Administrativa = "S";
                _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

                //ATUALIZA A SITUACAO FINANCEIRA DO PACIENTE
                pacienteToExist.PendenciaFinanceira = "N";
                _operacionalContext.PacienteItems.Update(pacienteToExist);

                var savePacienteAcomodacaoEvent = new PacienteSaveIE(pacienteToExist.Id_Paciente ,
                    pacienteToExist.Nome_Paciente,
                    pacienteToExist.Cod_Externo,
                    pacienteToExist.GeneroPaciente,
                    pacienteToExist.Dt_NascimentoPaciente,
                    pacienteToExist.PendenciaFinanceira);

                try
                {
                    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _operacionalIntegrationEventService.SaveEventAndPacienteContextChangesAsync(savePacienteAcomodacaoEvent, pacienteToExist);
                }
                catch (Exception e)
                {
                    ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                    Dberro.Main(e);
                    return BadRequest(e.Message);
                }

                // Publish through the Event Bus and mark the saved event as published
                await _operacionalIntegrationEventService.PublishThroughEventBusAsync(savePacienteAcomodacaoEvent);
            }




            return CreatedAtAction(nameof(AltaAdministrativa), "OK");
        }

        /// <summary>
        /// Integração da operação ALTA MEDICA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA O PACIENTE EXISTE
        /// 03) VERIFICA SE TEM O PACIENTE ATIVO NA ACOMODACAO
        /// 04) VALIDA ACOMODACAO STATUS OCUPADO
        /// 05) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="NomePaciente">
        /// Nome do Paciente
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="NumAtendimento">
        /// Numero de Atendimento para integração.
        /// </param>
        /// <param name="PendenciaFinanceiraPaciente">
        /// Indica se há ou não pendência financeira. 
        /// </param>
        [HttpPost]
        [Route("items/altamedica/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/codexternopaciente/{CodExternoPaciente}/numatendimento/{NumAtendimento}/pendenciafinanceira/{PendenciaFinanceiraPaciente}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AltaMedica(int idEmpresa,
                                                string codExternoAcomodacao,
                                                string CodExternoPaciente,
                                                string NumAtendimento,
                                                string PendenciaFinanceiraPaciente
                                                )
        {

            //string PendenciaFinanceiraPaciente = "S";
            ///PendenciaFinanceiraPaciente = "S";
            //AREA DE VALIDACAO
            int IdAcomodacao;
            //!(DateTime.TryParse(DataNascimentoPaciente, out dDate)
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao) 
                || (string.IsNullOrEmpty(CodExternoPaciente)) || (string.IsNullOrEmpty(NumAtendimento))
                || (string.IsNullOrEmpty(PendenciaFinanceiraPaciente))))
            {
                return BadRequest();
            }

            //FIM AREA DE VALIDACAO


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null )
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }


            //CONSULTA SE O PACIENTE ESTÁ ATIVO NA ACOMODACAO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == pacienteToExist.Id_Paciente);
          

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSEMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null);
            
            //VALIDA SE A SITUACAO DA ACOMODACAO É OCUPADA
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.OCUPADO)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAOOCUPADA"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.ALTAMEDICA, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }
            

            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //ATUALIZA A SITUACAO FINANCEIRA DO PACIENTE
            pacienteToExist.PendenciaFinanceira = PendenciaFinanceiraPaciente;
            _operacionalContext.PacienteItems.Update(pacienteToExist);


            ////TRATA FLUXO DE TRANSICAO DE SITUACAO
            List<AtividadeItem> lstAt = await Util.TrataFluxoSituacao(ConfiguracaoURL, tokenURL, idEmpresa, situacaoToUpdate.Id_TipoSituacaoAcomodacao, idTipoAcomodacao);

            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();

            if (lstAt.Count > 0)
            {
                foreach (AtividadeItem Atividade in lstAt)
                {
                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);
                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, IdAcomodacao));
                }

            }

            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = NumAtendimento;
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;
            //Trata SLA Nulo
             situacaoToSave.PacienteItem = pacienteToExist; 
            if (IdSLA > 0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.ALTAMEDICA;
            situacaoToSave.AtividadeItems=lstAt;

            _operacionalContext.SituacaoItems.Add(situacaoToSave);



            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                situacaoToUpdate.dt_FimSituacaoAcomodacao,
                situacaoToUpdate.cod_NumAtendimento,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);



            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAltaMedicaAsync(saveNewSituacaoEvent, finalizaSituacaoEvent, situacaoToSave, lstAtvEvento);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            //await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            //// Publish through the Event Bus and mark the saved event as published
            //await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(lstAtvEvento);

            return CreatedAtAction(nameof(AltaMedica), "OK");
        }

        /// <summary>
        /// Integração da operação ALTA MEDICA
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA O PACIENTE EXISTE
        /// 03) VERIFICA SE TEM O PACIENTE ATIVO NA ACOMODACAO
        /// 04) VALIDA ACOMODACAO STATUS OCUPADO
        /// 05) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="NumAtendimento">
        /// Numero de Atendimento para integração.
        /// </param>
        /// <param name="PendenciaFinanceiraPaciente">
        /// Indica se há ou não pendência financeira. 
        /// </param>
        [HttpPost]
        [Route("items/ajusteuti/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/codexternopaciente/{CodExternoPaciente}/numatendimento/{NumAtendimento}/pendenciafinanceira/{PendenciaFinanceiraPaciente}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AjusteUTI(int idEmpresa,
                                                string codExternoAcomodacao,
                                                string CodExternoPaciente,
                                                string NumAtendimento,
                                                string PendenciaFinanceiraPaciente
                                                )
        {

            //string PendenciaFinanceiraPaciente = "S";
            ///PendenciaFinanceiraPaciente = "S";
            //AREA DE VALIDACAO
            int IdAcomodacao;
            //!(DateTime.TryParse(DataNascimentoPaciente, out dDate)
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao)
                || (string.IsNullOrEmpty(CodExternoPaciente)) || (string.IsNullOrEmpty(NumAtendimento))
                || (string.IsNullOrEmpty(PendenciaFinanceiraPaciente))))
            {
                return BadRequest();
            }

            //FIM AREA DE VALIDACAO


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count == 0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }


            //CONSULTA SE O PACIENTE ESTÁ ATIVO NA ACOMODACAO
            var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == pacienteToExist.Id_Paciente);


            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSEMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE A SITUACAO DA ACOMODACAO É OCUPADA
            if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.OCUPADO)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAOOCUPADA"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.ALTAMEDICA, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }


            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //ATUALIZA A SITUACAO FINANCEIRA DO PACIENTE
            pacienteToExist.PendenciaFinanceira = PendenciaFinanceiraPaciente;
            _operacionalContext.PacienteItems.Update(pacienteToExist);


            ////TRATA FLUXO DE TRANSICAO DE SITUACAO
            ////List<AtividadeItem> lstAt = await Util.TrataFluxoSituacao(ConfiguracaoURL, tokenURL, idEmpresa, situacaoToUpdate.Id_TipoSituacaoAcomodacao, idTipoAcomodacao);


            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = NumAtendimento;
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;
            situacaoToSave.PacienteItem = pacienteToExist; 
            //Trata SLA Nulo
            if (IdSLA > 0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.ALTAMEDICA;
            situacaoToSave.AtividadeItems = new List<AtividadeItem>();

            _operacionalContext.SituacaoItems.Add(situacaoToSave);



            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao,
                situacaoToUpdate.dt_FimSituacaoAcomodacao,
                situacaoToUpdate.cod_NumAtendimento,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);



            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAltaMedicaAsync(saveNewSituacaoEvent, finalizaSituacaoEvent, situacaoToSave);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);


            return CreatedAtAction(nameof(AjusteUTI), "OK");
        }


        /// <summary>
        /// Integração da operação INTERNAÇÃO
        /// REGRAS:
        /// 01) VERIFICA SE EXISTE A ACOMODACAO
        /// 02) VERIFICA SE TEM PACIENTE ATIVO
        /// 03) VALIDA ACOMODACAO STATUS VAGO
        /// 04) VERIFICA EXISTENCIA DE SLA DA SITUACAO
        /// 05) VERIFICA SE O PACIENTE ESTÁ INTERNADO.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacao">
        /// Código da acomodação para integração 
        /// </param>
        /// <param name="NomePaciente">
        /// Nome do Paciente
        /// </param>
        /// <param name="CodExternoPaciente">
        /// Código do Paciente para integração 
        /// </param>
        /// <param name="NumAtendimento">
        /// Numero de Atendimento para integração.
        /// </param>
        /// <param name="GeneroPaciente">
        /// Genero do Paciente
        /// </param>
        /// <param name="DataNascimentoPaciente">
        /// Data de Nascimento do Paciente YYYY-MM-DD 00:00:00
        /// </param>
        [HttpPost]
        [Route("items/internacao/empresa/{idEmpresa}/codexterno/{codExternoAcomodacao}/nomepaciente/{NomePaciente}/codexternopaciente/{CodExternoPaciente}/numatendimento/{NumAtendimento}/genero/{GeneroPaciente}/datanascimento/{DataNascimentoPaciente}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Internacao(int idEmpresa, 
                                                    string codExternoAcomodacao ,
                                                    string NomePaciente,
                                                    string CodExternoPaciente,
                                                    string NumAtendimento,
                                                    string GeneroPaciente,
                                                    DateTime DataNascimentoPaciente
                                                    )
        {
            //AREA DE VALIDACAO
            int IdAcomodacao;
            //!(DateTime.TryParse(DataNascimentoPaciente, out dDate)
            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacao)|| (string.IsNullOrEmpty(NomePaciente))
                || (string.IsNullOrEmpty(CodExternoPaciente)) || (string.IsNullOrEmpty(NumAtendimento))
                || (string.IsNullOrEmpty(GeneroPaciente))))
            {
                return BadRequest();
            }

            //FIM AREA DE VALIDACAO


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA O PACIENTE
            var pacienteToExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);


            //VERIFICA SE TEM PACIENTE ATIVO
            bool flagPacReserva = false;
            PacienteAcomodacaoItem pacienteToValidate = null;
            if (pacienteToExist ==null)
            {
                //NESSE CASO SÓ VALIDA A EXISTENCIA DO PACIENTE NA ACOMODACAO
                 pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null);

                flagPacReserva = false;
            }
            else
            {
                pacienteToExist.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToExist.Nome_Paciente = NomePaciente;
                pacienteToExist.GeneroPaciente = GeneroPaciente;

                _operacionalContext.PacienteItems.Update(pacienteToExist);

                //NESSE CASO EXISTENCIA DO PACIENTE RESERVADO PARA ELE

                pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente != pacienteToExist.Id_Paciente);

                flagPacReserva = true;
            }

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate != null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADACOMPACIENTE"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToUpdate = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == itemAcomodacao.Id_Acomodacao && c.dt_FimSituacaoAcomodacao == null) ;

            if (!flagPacReserva)
            {
                //VALIDA SE A SITUACAO DA ACOMODACAO É LIBERADO
                if (situacaoToUpdate == null || situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.LIBERADO )
                {
                    string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAO"];
                    return BadRequest(msgStatus);
                }
            }
            else
            {
                //VALIDA SE A SITUACAO DA ACOMODACAO É RESERVA
                if (situacaoToUpdate == null || 
                    ((situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.RESERVADO)
                     && (situacaoToUpdate.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.LIBERADO)))
                {
                    string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAORESERVA"];
                    return BadRequest(msgStatus);
                }
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL ;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.OCUPADO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }

            //VALIDA SE O PACIENTE JÁ ESTÀ CADASTRADO
            PacienteItem pacienteToSave;
            PacienteAcomodacaoItem pacienteAcomodacaoToSave;
            pacienteAcomodacaoToSave = null;
            if (pacienteToExist == null)
            {
                //INCLUI PACIENTE
                pacienteToSave = new PacienteItem();
                pacienteToSave.Nome_Paciente = NomePaciente;
                pacienteToSave.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToSave.GeneroPaciente = GeneroPaciente;
                pacienteToSave.Cod_Externo = CodExternoPaciente;
                pacienteToSave.PendenciaFinanceira = "N";

                List<PacienteAcomodacaoItem> lstPacienteAcomodacao = new List<PacienteAcomodacaoItem>();

                pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
                pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacao;
                pacienteAcomodacaoToSave.NumAtendimento = NumAtendimento;
                pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
                pacienteAcomodacaoToSave.Dt_Saida = null;

                lstPacienteAcomodacao.Add(pacienteAcomodacaoToSave);

                pacienteToSave.PacienteAcomodacaoItems = lstPacienteAcomodacao;

                _operacionalContext.PacienteItems.Add(pacienteToSave);

            }
            else
            {

                var pacienteToValid = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Paciente  == pacienteToExist.Id_Paciente && c.Dt_Saida  == null);

                //VERIFICA SE O PACIENTE ESTÁ COM DT SAIDA NULA OU SEJA INTERNADO (nao sendo reserva).
                if (pacienteToValid != null && flagPacReserva ==false)
                {
                    string msgStatus = _localizer["VALIDA_PACIENTEINTERNADO"];
                    return BadRequest(msgStatus);
                }

                if (pacienteToValid == null)
                {
                    pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
                    pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacao;
                    pacienteAcomodacaoToSave.NumAtendimento = NumAtendimento;
                    pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
                    pacienteAcomodacaoToSave.Dt_Saida = null;
                    pacienteAcomodacaoToSave.Id_Paciente = pacienteToExist.Id_Paciente;

                    _operacionalContext.PacienteAcomodacaoItems.Add(pacienteAcomodacaoToSave);
                }
                else
                {
                    pacienteAcomodacaoToSave = pacienteToValid;
                }
                pacienteToSave = pacienteToExist;
            }


            //ATUALIZA A SITUACAO PASSADA
            situacaoToUpdate.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToUpdate);

            //INCLUI SITUACAO NOVA
            SituacaoItem situacaoToSave = new SituacaoItem();
            situacaoToSave.cod_NumAtendimento = NumAtendimento;
            situacaoToSave.Cod_Prioritario = "N";
            situacaoToSave.dt_FimSituacaoAcomodacao = null;
            situacaoToSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToSave.Id_Acomodacao = IdAcomodacao;
            if (pacienteToExist == null) { situacaoToSave.PacienteItem = pacienteToSave; }
            else { situacaoToSave.PacienteItem = pacienteToExist; };
            if (IdSLA>0) { situacaoToSave.Id_SLA = IdSLA; }
            situacaoToSave.Id_TipoSituacaoAcomodacao = (int) TipoSituacao.OCUPADO;

            _operacionalContext.SituacaoItems.Add(situacaoToSave);



            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoEvent = new SituacaoSaveIE(situacaoToUpdate.Id_SituacaoAcomodacao,
                situacaoToUpdate.Id_Acomodacao,
                situacaoToUpdate.Id_TipoSituacaoAcomodacao ,
                situacaoToUpdate.dt_InicioSituacaoAcomodacao ,
                situacaoToUpdate.dt_FimSituacaoAcomodacao ,
                situacaoToUpdate.cod_NumAtendimento ,
                situacaoToUpdate.Id_SLA,
                situacaoToUpdate.Cod_Prioritario, null);


            var saveNewSituacaoEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                situacaoToSave.Id_Acomodacao,
                situacaoToSave.Id_TipoSituacaoAcomodacao,
                situacaoToSave.dt_InicioSituacaoAcomodacao,
                situacaoToSave.dt_FimSituacaoAcomodacao,
                situacaoToSave.cod_NumAtendimento,
                situacaoToSave.Id_SLA,
                situacaoToSave.Cod_Prioritario, situacaoToSave.AtividadeItems);


            var saveNewPacienteEvent = new PacienteSaveIE(pacienteToSave.Id_Paciente,
                pacienteToSave.Nome_Paciente,
                pacienteToSave.Cod_Externo,
                pacienteToSave.GeneroPaciente,
                pacienteToSave.Dt_NascimentoPaciente,
                pacienteToSave.PendenciaFinanceira);


            var saveNewPacienteAcomodacaoEvent = new PacienteAcomodacaoSaveIE(pacienteAcomodacaoToSave.Id_PacienteAcomodacao,
                pacienteAcomodacaoToSave.Id_Paciente,
                pacienteAcomodacaoToSave.Dt_Entrada,
                pacienteAcomodacaoToSave.Dt_Saida,
                pacienteAcomodacaoToSave.Id_Acomodacao,
                pacienteAcomodacaoToSave.NumAtendimento);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndInternacaoAsync(saveNewPacienteEvent, saveNewPacienteAcomodacaoEvent,saveNewSituacaoEvent, finalizaSituacaoEvent, pacienteToSave, situacaoToSave);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteAcomodacaoEvent);

            return CreatedAtAction(nameof(Internacao), "OK");
        }


        /// <summary>
        /// Integração da operação TRANSFERENCIA
        /// REGRAS:
        ///01) VERIFICA SE EXISTE A ACOMODACAO ORIGEM
        ///02) VERIFICA SE EXISTE A ACOMODACAO DESTINO
        ///03) VERIFICA SE A ACOMODACAO TEM UM PACIENTE ATIVO ORIGEM
        ///04) VERIFICA SE A ACOMODACAO TEM UM PACIENTE ATIVO DESTINO
        ///05) VALIDA SE A SITUACAO DA ACOMODACAO DE DESTINO É VAGO
        ///06) VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO(VAGO) ORIGEM
        ///07) VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO(OCUPADO) DESTINO
        ///08) VALIDA A EXISTENCIA DE UMA SITUACAO DA ACOMODACAO ORIGEM(Para Fechar)
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa do contexto
        /// </param>
        /// <param name="codExternoAcomodacaoOrigem">
        /// Código da acomodação para integração ORIGEM 
        /// </param>
        /// <param name="NumAtendimentoOrigem">
        /// Numero do Atendimento para integração ORIGEM
        /// </param>
        /// <param name="lstOrigem">
        /// Lista de ipo de atividades a serem geradas na transferencia referente a acomodacao ORIGEM
        /// </param>
        /// <param name="codExternoAcomodacaoDestino">
        /// Código da acomodação para integração DESTINO  
        /// </param>
        /// <param name="NumAtendimentoDestino">
        /// Numero do Atendimento para integração DESTINO
        /// </param>
        /// <param name="lstDestino">
        /// Lista de ipo de atividades a serem geradas na transferencia referente a acomodacao DESTINO
        /// </param>
        [HttpPost]
        [Route("items/transferencia/empresa/{idEmpresa}/codexternoOrigem/{codExternoAcomodacaoOrigem}/numatendimentoOrigem/{NumAtendimentoOrigem}/codexternoDestino/{codExternoAcomodacaoDestino}/numatendimentoDestino/{NumAtendimentoDestino}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Transferencia(int idEmpresa,
                                                    string codExternoAcomodacaoOrigem,
                                                    string NumAtendimentoOrigem,
                                                    List<TipoAtividade>lstOrigem,
                                                    string codExternoAcomodacaoDestino,
                                                    string NumAtendimentoDestino,
                                                    List<TipoAtividade> lstDestino
                                                    )
        {
            int IdAcomodacaoOr;
            int IdAcomodacaoDs;

            if ((idEmpresa < 1) || (string.IsNullOrEmpty(codExternoAcomodacaoOrigem) || (string.IsNullOrEmpty(NumAtendimentoOrigem))
                || (string.IsNullOrEmpty(codExternoAcomodacaoDestino)) || (string.IsNullOrEmpty(NumAtendimentoDestino))))
            {
                return BadRequest();
            }


            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToViewOr = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacaoOrigem);
            //VERIFICA SE EXISTE A ACOMODACAO ORIGEM
            if (LstacomodacaoToViewOr is null || LstacomodacaoToViewOr.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA_ORIGEM"];
                return BadRequest(msgStatus);
            }

            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToViewOr[0];

            //VERIFICA SE EXISTE A ACOMODACAO ORIGEM
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA_ORIGEM"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacaoOr = itemAcomodacao.Id_Acomodacao;
            }

            var LstacomodacaoToViewDs = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacaoDestino);
            //VERIFICA SE EXISTE A ACOMODACAO DESTINO
            if (LstacomodacaoToViewDs is null || LstacomodacaoToViewDs.Count==0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA_DESTINO"];
                return BadRequest(msgStatus);
            }

            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacaoDs = LstacomodacaoToViewDs[0];

            //VERIFICA SE EXISTE A ACOMODACAO DESTINO
            if (itemAcomodacaoDs is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA_DESTINO"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacaoDs = itemAcomodacaoDs.Id_Acomodacao;
            }

            //VERIFICA SE TEM PACIENTE ATIVO
            var pacienteToValidateOr = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacaoOr && c.Dt_Saida == null);

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO ORIGEM
            if (pacienteToValidateOr == null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADACOMPACIENTEORIGEM"];
                return BadRequest(msgStatus);
            }

            //VERIFICA SE TEM PACIENTE ATIVO
            var pacienteToOrExist = _operacionalContext.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Id_Paciente == pacienteToValidateOr.Id_Paciente);

            //VALIDA SE O PACIENTE ATIVO DE ORIGEM EXISTE
            if (pacienteToOrExist == null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIAPACIENTE"];
                return BadRequest(msgStatus);
            }

            //VERIFICA SE TEM PACIENTE ATIVO DESTINO
            var pacienteToValidateDs = _operacionalContext.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacaoDs && c.Dt_Saida == null);

            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidateDs != null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOOCUPADACOMPACIENTEDESTINO"];
                return BadRequest(msgStatus);
            }

            //CONSULTA A SITUACAO DA ACOMODACAO DESTINO
            var situacaoToDs = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacaoDs && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE A SITUACAO DA ACOMODACAO DE DESTINO É VAGO / LIBERADO / RESERVADO
            if (situacaoToDs == null || 
                (situacaoToDs.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.VAGO)&&
                (situacaoToDs.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.LIBERADO)&&
                (situacaoToDs.Id_TipoSituacaoAcomodacao != (int)TipoSituacao.RESERVADO) )
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAODESTINO"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa,IdAcomodacaoOr);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO ALTA
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoOrToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.ALTAMEDICA, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLAOr;
            if (lstSlaSituacaoOrToView is null || lstSlaSituacaoOrToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLAVAGO"];
                //return BadRequest(msgStatus);
                IdSLAOr = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemOrSLA = lstSlaSituacaoOrToView[0];
                IdSLAOr = itemOrSLA.Id_SLA;
            }
            


            //CONSULTA O SLA DA SITUACAO OCUPADO

            var lstSlaSituacaoDsToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, (int)TipoSituacao.OCUPADO, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLADs;
            if (lstSlaSituacaoDsToView is null || lstSlaSituacaoDsToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLAOCUPADO"];
                //return BadRequest(msgStatus);
                IdSLADs = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemDsSLA = lstSlaSituacaoDsToView[0];
                IdSLADs = itemDsSLA.Id_SLA;
            }


            //CONSULTA A SITUACAO DA ACOMODACAO ORIGEM
            var situacaoToOr = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacaoOr && c.dt_FimSituacaoAcomodacao == null);

            //VALIDA SE EXITE
            if (situacaoToOr == null )
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSITUACAOORIEGMEXISTE"];
                return BadRequest(msgStatus);
            }

            //ATUALIZA A SITUACAO ORIGEM ANTERIOR
            situacaoToOr.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToOr);

            //ATUALIZA A SITUACAO DESTINO ANTERIOR
            situacaoToDs.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToDs);


            //VALIDA INTEGRIDADE DAS ATIVIDADES COM AS SITUACOES
            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> TSTAORIGEM = await ConsultaTipoSituacaoTipoAtividadeAsync(ConfiguracaoURL,
                                                            tokenURL,
                                                            (int)TipoSituacao.ALTAMEDICA);
            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> TSTADESTINO = await ConsultaTipoSituacaoTipoAtividadeAsync(ConfiguracaoURL,
                                                            tokenURL,
                                                            (int)TipoSituacao.OCUPADO);

            List<AtividadeItem> lstAtOr = new List<AtividadeItem>();
            List<AtividadeItem> lstAtDs = new List<AtividadeItem>();
            if (lstOrigem != null)
            {
                //GERA ATIVIDADES ORIGEM
                foreach (TipoAtividade tpIt in lstOrigem)
                {
                    var resultOr = TSTAORIGEM.Find(item => item.Id_TipoAtividadeAcomodacao == (int)tpIt);
                    //VALIDA SE EXITE A COMBINACAO TS + TA
                    if (resultOr == null)
                    {
                        string msgStatus = _localizer["VALIDA_ATIVIDADEORIGEM"];
                        return BadRequest(msgStatus);
                    }

                    AtividadeItem atividadeToSave = new AtividadeItem();
                    atividadeToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.ALTAMEDICA;
                    atividadeToSave.Id_TipoAtividadeAcomodacao = (int)tpIt;
                    atividadeToSave.Id_UsuarioSolicitante = 0;
                    atividadeToSave.dt_InicioAtividadeAcomodacao = DateTime.Now;
                    atividadeToSave.Cod_Prioritario = "N";
                    atividadeToSave.Cod_Plus = "N";


                    List<Configuracao.API.TO.ConsultarSLATO> SLAItem = await ConsultaSLAAsync(ConfiguracaoURL,
                                                                                tokenURL,
                                                                                idEmpresa,
                                                                                (int)TipoSituacao.ALTAMEDICA,
                                                                                (int)tpIt,
                                                                                (int)TipoAcao.SOLICITAR, idTipoAcomodacao);

                    AcaoItem acaoToSave = new AcaoItem();
                    acaoToSave.Id_TipoAcaoAcomodacao = (int)TipoAcao.SOLICITAR;
                    acaoToSave.dt_InicioAcaoAtividade = DateTime.Now;
                    if (SLAItem != null && SLAItem.Count > 0) { acaoToSave.Id_SLA = SLAItem[0].Id_SLA; }
                    acaoToSave.Id_UsuarioExecutor = "0";
                    List<AcaoItem> lstAcao = new List<AcaoItem>();
                    atividadeToSave.AcaoItems = lstAcao;
                    atividadeToSave.AcaoItems.Add(acaoToSave);

                    lstAtOr.Add(atividadeToSave);

                }
            }

            //INCLUI SITUACAO NOVA ORIGEM
            SituacaoItem situacaoToOrSave = new SituacaoItem();
            situacaoToOrSave.cod_NumAtendimento = NumAtendimentoOrigem;
            situacaoToOrSave.Cod_Prioritario = "N";
            situacaoToOrSave.dt_FimSituacaoAcomodacao = null;
            situacaoToOrSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToOrSave.Id_Acomodacao = IdAcomodacaoOr;
            situacaoToOrSave.PacienteItem = null; // pacienteToValidateOr.PacienteItem; 
            if (IdSLAOr > 0) { situacaoToOrSave.Id_SLA = IdSLAOr; }
            situacaoToOrSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.ALTAMEDICA;

            situacaoToOrSave.AtividadeItems = lstAtOr;

            _operacionalContext.SituacaoItems.Add(situacaoToOrSave);


            if (lstDestino != null)
            {
                //GERA ATIVIDADES DESTINO
                foreach (TipoAtividade tpIt in lstDestino)
                {
                    var resultDs = TSTADESTINO.Find(item => item.Id_TipoAtividadeAcomodacao == (int)tpIt);
                    //VALIDA SE EXITE A COMBINACAO TS + TA
                    if (resultDs == null)
                    {
                        string msgStatus = _localizer["VALIDA_ATIVIDADEDESTINO"];
                        return BadRequest(msgStatus);
                    }

                    AtividadeItem atividadeToSave = new AtividadeItem();
                    atividadeToSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.OCUPADO;
                    atividadeToSave.Id_TipoAtividadeAcomodacao = (int)tpIt;
                    atividadeToSave.Id_UsuarioSolicitante = 0;
                    atividadeToSave.dt_InicioAtividadeAcomodacao = DateTime.Now;
                    atividadeToSave.Cod_Prioritario = "N";
                    atividadeToSave.Cod_Plus = "N";

                    List<Configuracao.API.TO.ConsultarSLATO> SLAItem = await ConsultaSLAAsync(ConfiguracaoURL,
                                                                     tokenURL,
                                                                     idEmpresa,
                                                                     (int)TipoSituacao.OCUPADO ,
                                                                     (int)tpIt,
                                                                     (int)TipoAcao.SOLICITAR, idTipoAcomodacao);

                    AcaoItem acaoToSave = new AcaoItem();
                    acaoToSave.Id_TipoAcaoAcomodacao = (int)TipoAcao.SOLICITAR;
                    acaoToSave.dt_InicioAcaoAtividade = DateTime.Now;
                    if (SLAItem != null && SLAItem.Count > 0) { acaoToSave.Id_SLA = SLAItem[0].Id_SLA; }
                    acaoToSave.Id_UsuarioExecutor = "0";
                    List<AcaoItem> lstAcao = new List<AcaoItem>();
                    atividadeToSave.AcaoItems = lstAcao;
                    atividadeToSave.AcaoItems.Add(acaoToSave);

                    lstAtDs.Add(atividadeToSave);
                }
            }


            //INCLUI SITUACAO NOVA DESTINO
            SituacaoItem situacaoToDsSave = new SituacaoItem();
            situacaoToDsSave.cod_NumAtendimento = NumAtendimentoDestino;
            situacaoToDsSave.Cod_Prioritario = "N";
            situacaoToDsSave.dt_FimSituacaoAcomodacao = null;
            situacaoToDsSave.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToDsSave.Id_Acomodacao = IdAcomodacaoDs;
            situacaoToDsSave.PacienteItem = pacienteToOrExist;
            if (IdSLADs > 0) { situacaoToDsSave.Id_SLA = IdSLADs; }
            situacaoToDsSave.Id_TipoSituacaoAcomodacao = (int)TipoSituacao.OCUPADO;

            situacaoToDsSave.AtividadeItems = lstAtDs;

            _operacionalContext.SituacaoItems.Add(situacaoToDsSave);

            //Trata envio do Push das atividades de destino

            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();

            if (lstAtDs.Count > 0)
            {
                foreach (AtividadeItem Atividade in lstAtDs)
                {
                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);
                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, IdAcomodacaoDs));
                }

            }


            //ATUALIZA PACIENTE DE ORIGEM
            pacienteToValidateOr.Dt_Saida = DateTime.Now;
            _operacionalContext.PacienteAcomodacaoItems.Update(pacienteToValidateOr);

            //INCLUI PACIENTE DESTINO
            PacienteAcomodacaoItem pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
            pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
            pacienteAcomodacaoToSave.Dt_Saida = null;
            pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacaoDs;
            pacienteAcomodacaoToSave.Id_Paciente  = pacienteToValidateOr.Id_Paciente ;
            pacienteAcomodacaoToSave.NumAtendimento = NumAtendimentoDestino;

            _operacionalContext.PacienteAcomodacaoItems.Add(pacienteAcomodacaoToSave);


            //Create Integration Event to be published through the Event Bus
            var finalizaSituacaoOrEvent = new SituacaoSaveIE(situacaoToOr.Id_SituacaoAcomodacao,
                situacaoToOr.Id_Acomodacao,
                situacaoToOr.Id_TipoSituacaoAcomodacao,
                situacaoToOr.dt_InicioSituacaoAcomodacao,
                situacaoToOr.dt_FimSituacaoAcomodacao,
                situacaoToOr.cod_NumAtendimento,
                situacaoToOr.Id_SLA,
                situacaoToOr.Cod_Prioritario, null);


            var saveNewSituacaoOrEvent = new SituacaoSaveIE(situacaoToOrSave.Id_SituacaoAcomodacao,
                situacaoToOrSave.Id_Acomodacao,
                situacaoToOrSave.Id_TipoSituacaoAcomodacao,
                situacaoToOrSave.dt_InicioSituacaoAcomodacao,
                situacaoToOrSave.dt_FimSituacaoAcomodacao,
                situacaoToOrSave.cod_NumAtendimento,
                situacaoToOrSave.Id_SLA,
                situacaoToOrSave.Cod_Prioritario, situacaoToOrSave.AtividadeItems);

            var finalizaSituacaoDsEvent = new SituacaoSaveIE(situacaoToDs.Id_SituacaoAcomodacao,
                situacaoToDs.Id_Acomodacao,
                situacaoToDs.Id_TipoSituacaoAcomodacao,
                situacaoToDs.dt_InicioSituacaoAcomodacao,
                situacaoToDs.dt_FimSituacaoAcomodacao,
                situacaoToDs.cod_NumAtendimento,
                situacaoToDs.Id_SLA,
                situacaoToDs.Cod_Prioritario, null);


            var saveNewSituacaoDsEvent = new SituacaoSaveIE(situacaoToDsSave.Id_SituacaoAcomodacao,
                situacaoToDsSave.Id_Acomodacao,
                situacaoToDsSave.Id_TipoSituacaoAcomodacao,
                situacaoToDsSave.dt_InicioSituacaoAcomodacao,
                situacaoToDsSave.dt_FimSituacaoAcomodacao,
                situacaoToDsSave.cod_NumAtendimento,
                situacaoToDsSave.Id_SLA,
                situacaoToDsSave.Cod_Prioritario, situacaoToDsSave.AtividadeItems);


            var saveNewPacienteAcomodacaoOrEvent = new PacienteAcomodacaoSaveIE(pacienteToValidateOr.Id_PacienteAcomodacao,
                pacienteToValidateOr.Id_Paciente,
                pacienteToValidateOr.Dt_Entrada,
                pacienteToValidateOr.Dt_Saida,
                pacienteToValidateOr.Id_Acomodacao,
                pacienteToValidateOr.NumAtendimento);

            var saveNewPacienteAcomodacaoDsEvent = new PacienteAcomodacaoSaveIE(pacienteAcomodacaoToSave.Id_PacienteAcomodacao,
                pacienteAcomodacaoToSave.Id_Paciente,
                pacienteAcomodacaoToSave.Dt_Entrada,
                pacienteAcomodacaoToSave.Dt_Saida,
                pacienteAcomodacaoToSave.Id_Acomodacao,
                pacienteAcomodacaoToSave.NumAtendimento);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndTransferenciaAsync(finalizaSituacaoOrEvent, 
                    finalizaSituacaoDsEvent, saveNewSituacaoOrEvent, saveNewSituacaoDsEvent,
                    saveNewPacienteAcomodacaoOrEvent, saveNewPacienteAcomodacaoDsEvent,
                    situacaoToOrSave, situacaoToDsSave, pacienteToValidateOr, pacienteAcomodacaoToSave, lstAtvEvento);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoOrEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoOrEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoDsEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoDsEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteAcomodacaoOrEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewPacienteAcomodacaoDsEvent);
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(lstAtvEvento);

            return CreatedAtAction(nameof(Transferencia), "OK");
        }

        [HttpPost]
        [Route("items/ajuste/empresa/{idEmpresa}/codexternoacomodacao/{codExternoAcomodacao}/situacaodestino/{IdTipoSituacaoDestino}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AjusteSituacao(int idEmpresa,
                                        string codExternoAcomodacao,
                                        int IdTipoSituacaoDestino
                                        )
        {

            int IdAcomodacao;

            if ((idEmpresa < 1) || (IdTipoSituacaoDestino<1 || IdTipoSituacaoDestino >8) ||(string.IsNullOrEmpty(codExternoAcomodacao)))
            {
                return BadRequest();
            }

            List<IntegrationEvent> lstAtvEvento = new List<IntegrationEvent>();

            string AdministracaoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var LstacomodacaoToSave = await ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, idEmpresa, codExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count == 0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                return BadRequest(msgStatus);
            }
            else
            {
                IdAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToOr = _operacionalContext.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.dt_FimSituacaoAcomodacao == null);

            if (situacaoToOr is null)
            {
                string msgStatus = _localizer["VALIDA_SITUACAOATUAL"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, IdAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            //CONSULTA O SLA DA SITUACAO DESTINO 
            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            var lstSlaSituacaoToView = await ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, idEmpresa, IdTipoSituacaoDestino, idTipoAcomodacao);
            //VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            int IdSLA;
            if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            {
                //string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
                //return BadRequest(msgStatus);
                IdSLA = -1;
            }
            else
            {
                ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
                IdSLA = itemSLA.Id_SLA;
            }

            //CONSULTA AS ATIVIDADES DA SITUACAO DA ACOMODACAO (ATIVAS)
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == situacaoToOr.Id_SituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();

            ////////////////////////////////////////////////////////////////////
            if (atividadeAcomodacaoExistentes.Count() > 0)
            {
                foreach (AtividadeItem Atividade in atividadeAcomodacaoExistentes)
                {

                   //BUSCA A ACAO ATUAL
                    var acaoAtividadeAcomodacaoToUpdate = _operacionalContext.AcaoItems
                        .OfType<AcaoItem>()
                        .SingleOrDefault(c => c.Id_AtividadeAcomodacao == Atividade.Id_AtividadeAcomodacao && c.dt_FimAcaoAtividade == null);

                    if (acaoAtividadeAcomodacaoToUpdate is null)
                    {
                        string msgStatus = _localizer["VALIDA_STATUS"];
                        return BadRequest(msgStatus);
                    }

                    acaoAtividadeAcomodacaoToUpdate.dt_FimAcaoAtividade = DateTime.Now;
                    _operacionalContext.AcaoItems.Update(acaoAtividadeAcomodacaoToUpdate);

                    Atividade.dt_FimAtividadeAcomodacao = DateTime.Now;
                    _operacionalContext.AtividadeItems.Update(Atividade);

                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);

                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, IdAcomodacao));
                }
            }

            ////TRATA FLUXO DE TRANSICAO DE SITUACAO
            List<AtividadeItem> lstAt = await Util.TrataFluxoSituacao(ConfiguracaoURL, tokenURL, idEmpresa, situacaoToOr.Id_TipoSituacaoAcomodacao, idTipoAcomodacao);

            if (lstAt.Count >0 )
            {
                foreach (AtividadeItem Atividade in lstAt)
                {
                    List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, Atividade.Id_TipoSituacaoAcomodacao, Atividade.Id_TipoAtividadeAcomodacao);
                    lstAtvEvento.Add(Util.CriaEventoAtividade(Atividade, Perfis, IdAcomodacao));
                }
                    
            }


            //ATUALIZA A SITUACAO ORIGEM ANTERIOR
            situacaoToOr.dt_FimSituacaoAcomodacao = DateTime.Now;
            _operacionalContext.SituacaoItems.Update(situacaoToOr);

            //INCLUI SITUACAO NOVA DESTINO
            SituacaoItem situacaoToDs = new SituacaoItem();
            situacaoToDs.cod_NumAtendimento = "0";
            situacaoToDs.Cod_Prioritario = "N";
            situacaoToDs.dt_FimSituacaoAcomodacao = null;
            situacaoToDs.dt_InicioSituacaoAcomodacao = DateTime.Now;
            situacaoToDs.Id_Acomodacao = IdAcomodacao;

            if (IdTipoSituacaoDestino == (int)TipoSituacao.LIBERADO || IdTipoSituacaoDestino == (int)TipoSituacao.VAGO || IdTipoSituacaoDestino == (int)TipoSituacao.INTERDITADO)
            {
                situacaoToDs.PacienteItem = null;
            }
            else
            {
                situacaoToDs.PacienteItem = situacaoToOr.PacienteItem;
            }
                //Valida o SLA nulo
                if (IdSLA > 0) { situacaoToDs.Id_SLA = IdSLA; }
            situacaoToDs.Id_TipoSituacaoAcomodacao = IdTipoSituacaoDestino;

            situacaoToDs.AtividadeItems = lstAt;

            _operacionalContext.SituacaoItems.Add(situacaoToDs);

            if (IdTipoSituacaoDestino == (int)TipoSituacao.LIBERADO || IdTipoSituacaoDestino == (int)TipoSituacao.VAGO || IdTipoSituacaoDestino == (int)TipoSituacao.INTERDITADO)
            {
                //CONSULTA SE TEM PACIENTE ATIVO NA ACOMODACAO, SE HOUVER ATUALIZA A DATA DE SAÍDA
                var pacienteToValidate = _operacionalContext.PacienteAcomodacaoItems
                .OfType<PacienteAcomodacaoItem>()
                .Where(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null);

                foreach (PacienteAcomodacaoItem item in pacienteToValidate)
                {
                    item.Dt_Saida = DateTime.Now;
                    _operacionalContext.PacienteAcomodacaoItems.Update(item);
                }
            }


                //Create Integration Event to be published through the Event Bus
                var finalizaSituacaoOrEvent = new SituacaoSaveIE(situacaoToOr.Id_SituacaoAcomodacao,
                situacaoToOr.Id_Acomodacao,
                situacaoToOr.Id_TipoSituacaoAcomodacao,
                situacaoToOr.dt_InicioSituacaoAcomodacao,
                situacaoToOr.dt_FimSituacaoAcomodacao,
                situacaoToOr.cod_NumAtendimento,
                situacaoToOr.Id_SLA,
                situacaoToOr.Cod_Prioritario, null);


            var saveNewSituacaoOrEvent = new SituacaoSaveIE(situacaoToDs.Id_SituacaoAcomodacao,
                situacaoToDs.Id_Acomodacao,
                situacaoToDs.Id_TipoSituacaoAcomodacao,
                situacaoToDs.dt_InicioSituacaoAcomodacao,
                situacaoToDs.dt_FimSituacaoAcomodacao,
                situacaoToDs.cod_NumAtendimento,
                situacaoToDs.Id_SLA,
                situacaoToDs.Cod_Prioritario, situacaoToDs.AtividadeItems);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndDeParaAsync(finalizaSituacaoOrEvent, saveNewSituacaoOrEvent, situacaoToDs,lstAtvEvento);
            }
            catch (Exception e)
            {
                ErrorHandler.TrataErro Dberro = new ErrorHandler.TrataErro();
                Dberro.Main(e);
                return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            //await _operacionalIntegrationEventService.PublishThroughEventBusAsync(finalizaSituacaoOrEvent);
            //// Publish through the Event Bus and mark the saved event as published
            //await _operacionalIntegrationEventService.PublishThroughEventBusAsync(saveNewSituacaoOrEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(lstAtvEvento);

            ////////////if (IdTipoSituacaoDestino == (int)TipoSituacao.LIBERADO)
            {
                string urlApiLiberacao = _settings.urlApiLiberacao;
                string rotaApiLiberacao = _settings.rotaApiLiberacao;
                string userApiLiberacao = _settings.userApiLiberacao;
                string passwordApiLiberacao = _settings.passwordApiLiberacao;
                string hostApiLiberacao = _settings.hostApiLiberacao;
                //ESCREVE AQUI

                string result = await Util.LiberarAcomodacao(urlApiLiberacao, rotaApiLiberacao, userApiLiberacao, passwordApiLiberacao, hostApiLiberacao, codExternoAcomodacao);
                if (result != "OK")
                {
                    string msgStatus;
                    if (result == "Erro")
                    {
                        msgStatus = _localizer["VALIDA_FALHALIBERACAO"];
                    }
                    else
                    {
                        msgStatus = result;
                    }
                    return BadRequest(msgStatus);

                }
               
            }
            return CreatedAtAction(nameof(AjusteSituacao), "OK");
        }

    }
}