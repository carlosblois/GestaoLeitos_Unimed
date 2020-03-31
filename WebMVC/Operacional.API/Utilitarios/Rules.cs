using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Operacional.API.Controllers;
using Operacional.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Operacional.API.Utilitarios;
using Administrativo.API.TO;
using Configuracao.API.TO;
using Operacional.API.Model;

namespace Operacional.API.Utilitarios
{
    public class Valid<T>
    {
        public bool retorno;
        public string msg;
        public int Id;
        public T obj;
    }
    public class Rules
    {


        internal static async Task<Valid<bool>>  ValidaSituacaoOrigemDestino(OperacionalContext context,
                                                              IStringLocalizer<IntegracaoController> localizer,
                                                              int IdEmpresa,
                                                              int Id_TipoSituacaoAcomodacaoOrigem,
                                                              int Id_TipoSituacaoAcomodacaoDestino)
        {
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<bool> vl = new Valid<bool>();


            string msgStatus = "";
            vl.retorno = true;
            vl.msg = msgStatus;
            return vl;
        }

        internal static async Task<Valid<SituacaoItem>> ValidaSituacaoAtual(OperacionalContext context,
                                                      IStringLocalizer<IntegracaoController> localizer,
                                                      int IdAcomodacao,
                                                      int Id_TipoSituacao)
        {
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<SituacaoItem> vl = new Valid<SituacaoItem>();


            //CONSULTA A SITUACAO DA ACOMODACAO 
            var situacaoToOr = context.SituacaoItems
            .OfType<SituacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.dt_FimSituacaoAcomodacao == null);

            if (situacaoToOr.Id_TipoSituacaoAcomodacao == Id_TipoSituacao)
            {
                string msgStatus = "";
                vl.retorno = true;
                vl.msg = msgStatus;
                vl.Id = situacaoToOr.Id_TipoSituacaoAcomodacao;
                vl.obj = situacaoToOr;
                return vl;
            }
            else
            {
                string msgStatus = "";
                vl.retorno = false;
                vl.msg = msgStatus;
                return vl;
            }

        }

        internal static async Task<Valid<ConsultarAcomodacaoPorIdEmpresaCodExternoTO>> ValidaExisteAcomodacao(OperacionalSettings settings,
                                                                IStringLocalizer<IntegracaoController> localizer,
                                                                int IdEmpresa,
                                                                string CodExternoAcomodacao)
        {
             OperacionalSettings _settings;
            _settings = settings;
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> vl = new Valid<ConsultarAcomodacaoPorIdEmpresaCodExternoTO>();
            int idAcomodacao;

            string AdministracaoURL = _settings.AdministrativoURL;
            string tokenURL = _settings.TokenURL;

            var LstacomodacaoToSave = await Util.ConsultaAcomodacaoAsync(AdministracaoURL, tokenURL, IdEmpresa, CodExternoAcomodacao);
            //VERIFICA SE EXISTE A ACOMODACAO
            if (LstacomodacaoToSave is null || LstacomodacaoToSave.Count == 0)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                vl.retorno = false;
                vl.msg = msgStatus;
                return vl;
            }

            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> lst = LstacomodacaoToSave;
            ConsultarAcomodacaoPorIdEmpresaCodExternoTO itemAcomodacao = LstacomodacaoToSave[0];

            //VERIFICA SE EXISTE A ACOMODACAO
            if (itemAcomodacao is null)
            {
                string msgStatus = _localizer["VALIDA_EXISTENCIA"];
                vl.retorno = false;
                vl.msg = msgStatus;
                return vl;
            }
            else
            {
                idAcomodacao = itemAcomodacao.Id_Acomodacao;
            }

            vl.retorno = true;
            vl.msg = "SUCESSO";
            vl.Id = idAcomodacao;
            vl.obj = itemAcomodacao;
            return vl;
        }


        internal static async Task<Valid<ConsultarSLASituacaoTO>> xValidaExisteSLA(OperacionalSettings settings,
                                                        IStringLocalizer<IntegracaoController> localizer,
                                                        int IdEmpresa,
                                                        Enum.ExpoEnum.TipoSituacao IdTiposituacao)
        {
            OperacionalSettings _settings;
            _settings = settings;
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<ConsultarSLASituacaoTO> vl = new Valid<ConsultarSLASituacaoTO>();
            int IdSLA;

            ////CONSULTA O SLA DA SITUACAO 
            //string tokenURL = _settings.TokenURL;
            //string ConfiguracaoURL = _settings.ConfiguracaoURL;


            //var lstSlaSituacaoToView = await Util.ConsultaSLASituacaoAsync(ConfiguracaoURL, tokenURL, IdEmpresa, (int)IdTiposituacao);
            ////VERIFICA SE EXISTE SLA DA SITUACAO DA ACOMODACAO
            //if (lstSlaSituacaoToView is null || lstSlaSituacaoToView.Count == 0)
            //{
            //    string msgStatus = _localizer["VALIDA_EXISTENCIASLA"];
            //    vl.retorno = false;
            //    vl.msg = msgStatus;
            //    return vl;
            //}

            //ConsultarSLASituacaoTO itemSLA = lstSlaSituacaoToView[0];
            //IdSLA = itemSLA.Id_SLA;

            //vl.retorno = true;
            //vl.msg = "SUCESSO";
            //vl.Id = IdSLA;
            //vl.obj = itemSLA;
            return vl;
        }

        private static Valid<bool> ValidaOutroPacienteAtivo(OperacionalContext context,
                                                                    IOptionsSnapshot<OperacionalSettings> settings,
                                                                    IStringLocalizer<IntegracaoController> localizer,
                                                                    int IdEmpresa,
                                                                    int IdAcomodacao,
                                                                    int IdPaciente)
        {
            OperacionalSettings _settings;
            _settings = settings.Value;
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<bool> vl = new Valid<bool>();

            //CONSULTA  SE A ACOMODACAO TEM UM PACIENTE ATIVO NAO SENDO O PACIENTE INDICADO
            var pacienteToValidate = context.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente != IdPaciente);


            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO NAO SENDO O PACIENTE INDICADO
            if (pacienteToValidate != null)
            {
                string msgStatus = _localizer["VALIDA_ACOMODACAOSEMPACIENTE"];
                vl.retorno = false;
                vl.msg = msgStatus;
                return vl;
            }

            vl.retorno = true;
            vl.msg = "SUCESSO";
            vl.obj = true;
            return vl;


        }
        private static Valid<bool> ValidaExistePacienteAcomodacao(OperacionalContext context,
                                                                        IOptionsSnapshot<OperacionalSettings> settings,
                                                                        IStringLocalizer<IntegracaoController> localizer,
                                                                        int IdEmpresa,
                                                                        int IdAcomodacao,
                                                                        int IdPaciente)
        {
            OperacionalSettings _settings;
            _settings = settings.Value;
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<bool> vl = new Valid<bool>();

            //CONSULTA SE O PACIENTE ESTÁ ATIVO NA ACOMODACAO
            var pacienteToValidate = context.PacienteAcomodacaoItems
            .OfType<PacienteAcomodacaoItem>()
            .SingleOrDefault(c => c.Id_Acomodacao == IdAcomodacao && c.Dt_Saida == null && c.Id_Paciente == IdPaciente);


            //VALIDA SE A ACOMODACAO TEM UM PACIENTE ATIVO
            if (pacienteToValidate == null)
            {
                PacienteAcomodacaoItem pac = new PacienteAcomodacaoItem();
                pac.Id_Paciente = IdPaciente;
                pac.Id_Acomodacao = IdAcomodacao;
                pac.Dt_Entrada = DateTime.Now;

                context.PacienteAcomodacaoItems.Add(pac);
            }

            vl.retorno = true;
            vl.msg = "SUCESSO";
            return vl;

        }

        private static Valid<bool> ValidaExistePacienteCadastrado(OperacionalContext context,
                                                                IOptionsSnapshot<OperacionalSettings> settings,
                                                                IStringLocalizer<IntegracaoController> localizer,
                                                                int IdEmpresa,
                                                                int IdAcomodacao,
                                                                string CodExternoPaciente,
                                                                string NomePaciente,
                                                                DateTime DataNascimentoPaciente,
                                                                string GeneroPaciente
                                                                )
        {
            OperacionalSettings _settings;
            _settings = settings.Value;
            IStringLocalizer<IntegracaoController> _localizer;
            _localizer = localizer;
            Valid<bool> vl = new Valid<bool>();

            //CONSULTA O PACIENTE
            var pacienteToExist = context.PacienteItems
            .OfType<PacienteItem>()
            .SingleOrDefault(c => c.Cod_Externo == CodExternoPaciente);

            //VALIDA SE EXISTE O PACIENTE
            if (pacienteToExist == null)
            {
                //INCLUI PACIENTE
                PacienteItem pacienteToSave = new PacienteItem();
                pacienteToSave.Nome_Paciente = NomePaciente;
                pacienteToSave.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToSave.GeneroPaciente = GeneroPaciente;
                pacienteToSave.Cod_Externo = CodExternoPaciente;
                pacienteToSave.PendenciaFinanceira = "N";

                //ASSOCIA O PACIENTE
                List<PacienteAcomodacaoItem> lstPacienteAcomodacao = new List<PacienteAcomodacaoItem>();

                PacienteAcomodacaoItem pacienteAcomodacaoToSave = new PacienteAcomodacaoItem();
                pacienteAcomodacaoToSave.Id_Acomodacao = IdAcomodacao;
                pacienteAcomodacaoToSave.NumAtendimento = "INTERNO";
                pacienteAcomodacaoToSave.Dt_Entrada = DateTime.Now;
                pacienteAcomodacaoToSave.Dt_Saida = null;

                lstPacienteAcomodacao.Add(pacienteAcomodacaoToSave);

                pacienteToSave.PacienteAcomodacaoItems = lstPacienteAcomodacao;

                context.PacienteItems.Add(pacienteToSave);
            }
            else
            {
                //ATUALIZA
                pacienteToExist.Nome_Paciente = NomePaciente;
                pacienteToExist.Dt_NascimentoPaciente = DataNascimentoPaciente;
                pacienteToExist.GeneroPaciente = GeneroPaciente;
                pacienteToExist.PendenciaFinanceira = "N";

                //VALIDA ASSOCIACAO
                //SENAO ESTA ASSOCIADO ASSOCIA.
                ValidaExistePacienteAcomodacao(context, settings, localizer, IdEmpresa, IdAcomodacao, pacienteToExist.Id_Paciente);

                context.PacienteItems.Update(pacienteToExist);
            }

            vl.retorno = true;
            vl.msg = "SUCESSO";
            return vl;

        }

    }
    }
