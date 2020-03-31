using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Security;
using System.Configuration;
using GLeitos.Models;
using ExpoFramework.Framework;
using GLeitos.GLeitosTO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace GLeitos.Controllers
{
    public class DashboardController : BaseController
    {
        #region "Dashboard principal"
                
        public ActionResult Dashboard()
        {
            DashboardView vwDashboard = new DashboardView();
            List<TipoAtividadeAcomodacaoTO> LstAtividades = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoSituacaoAcomodacaoTO> LstSituacoes = new List<TipoSituacaoAcomodacaoTO>();
            List<DashboardHeaderTO> LstDashboardHeaderSituacao = new List<DashboardHeaderTO>();
            List<DashboardHeaderTO> LstDashboardHeader = new List<DashboardHeaderTO>();
            DashboardHeaderTO objDashboardHeaderSituacao = new DashboardHeaderTO();
            List<DashboardBodyTO> LstDashboardBody = new List<DashboardBodyTO>();
            List<DashboardBodyAtividadeTO> LstDashboardBodyAtividade = new List<DashboardBodyAtividadeTO>();
            DashboardBodyAtividadeTO ObjDashboardBodyAtividade = new DashboardBodyAtividadeTO();
            List<LogAcessoTO> objLstLogAcessos = new List<LogAcessoTO>();
            LogAcessoTO objLogAcesso = new LogAcessoTO();
            try
            {
                LstAtividades = base.ListarTipoAtividadeAcomodacao();
                LstSituacoes = base.ListarTipoSituacaoAcomodacao();
                objLstLogAcessos = base.ListaAcessos();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                string url = string.Format("api/Gestao/items/dashboardheader/empresa/{0}", Session["id_EmpresaSel"].ToString());

                LstDashboardHeaderSituacao = base.Listar<DashboardHeaderTO>(accessToken, _urlBase, url);

                url = string.Format("api/Gestao/items/dashboardbody/empresa/{0}", Session["id_EmpresaSel"].ToString());

                LstDashboardBody = base.Listar<DashboardBodyTO>(accessToken, _urlBase, url);

                foreach (TipoSituacaoAcomodacaoTO item in LstSituacoes)
                {
                    DashboardHeaderTO objDashboardHeader = new DashboardHeaderTO();

                    objDashboardHeader = LstDashboardHeaderSituacao.Where(m => m.id_TipoSituacaoAcomodacao == item.id_TipoSituacaoAcomodacao).ToList().FirstOrDefault();

                    if (objDashboardHeader==null)
                    {
                        objDashboardHeaderSituacao = new DashboardHeaderTO();
                        objDashboardHeaderSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardHeaderSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardHeaderSituacao.perc = "0";
                        objDashboardHeaderSituacao.qtd = "0";
                        objDashboardHeaderSituacao.corsituacao = item.cor_TipoSituacaoAcomodacao;
                        objDashboardHeaderSituacao.imagem = item.imagem;
                        objDashboardHeaderSituacao.ordem = item.ordem;
                        LstDashboardHeader.Add(objDashboardHeaderSituacao);
                    }
                    else
                    {
                        objDashboardHeader.imagem = item.imagem;
                        objDashboardHeader.corsituacao = item.cor_TipoSituacaoAcomodacao;
                        objDashboardHeader.ordem = item.ordem;
                        LstDashboardHeader.Add(objDashboardHeader);
                    }
                }

                foreach (TipoAtividadeAcomodacaoTO item in LstAtividades)
                {
                    List<DashboardBodyTO> objDashboardBody = new List<DashboardBodyTO>();

                    objDashboardBody = LstDashboardBody.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao).ToList();
                    objLogAcesso = new LogAcessoTO();
                    objLogAcesso = objLstLogAcessos.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao).FirstOrDefault();

                    if (objDashboardBody==null)
                    {
                        ObjDashboardBodyAtividade = new DashboardBodyAtividadeTO();
                        ObjDashboardBodyAtividade = CriarDashboardAtividadeZerado(LstSituacoes, item);
                        ObjDashboardBodyAtividade.cor = item.cor_TipoAtividadeAcomodacao;
                        ObjDashboardBodyAtividade.imagem = item.imagem;
                        ObjDashboardBodyAtividade.ordem = item.ordem;
                        if (objLogAcesso != null)
                        {
                            ObjDashboardBodyAtividade.totalAcessos = objLogAcesso.totalUsuarios;
                        }
                        LstDashboardBodyAtividade.Add(ObjDashboardBodyAtividade);
                    }
                    else
                    {
                        if (objDashboardBody.Count > 0)
                        {
                            ObjDashboardBodyAtividade = new DashboardBodyAtividadeTO();
                            ObjDashboardBodyAtividade = CriarDashboardAtividade(LstSituacoes, objDashboardBody);
                            ObjDashboardBodyAtividade.cor = item.cor_TipoAtividadeAcomodacao;
                            ObjDashboardBodyAtividade.imagem = item.imagem;
                            ObjDashboardBodyAtividade.ordem = item.ordem;
                            if (objLogAcesso != null)
                            {
                                ObjDashboardBodyAtividade.totalAcessos = objLogAcesso.totalUsuarios;
                            }
                            LstDashboardBodyAtividade.Add(ObjDashboardBodyAtividade);
                        }
                        else
                        {
                            ObjDashboardBodyAtividade = new DashboardBodyAtividadeTO();
                            ObjDashboardBodyAtividade = CriarDashboardAtividadeZerado(LstSituacoes, item);
                            ObjDashboardBodyAtividade.cor = item.cor_TipoAtividadeAcomodacao;
                            ObjDashboardBodyAtividade.imagem = item.imagem;
                            ObjDashboardBodyAtividade.ordem = item.ordem;
                            if (objLogAcesso != null)
                            {
                                ObjDashboardBodyAtividade.totalAcessos = objLogAcesso.totalUsuarios;
                            }
                            LstDashboardBodyAtividade.Add(ObjDashboardBodyAtividade);
                        }
                    }
                }
                vwDashboard.HeaderSituacao = LstDashboardHeader;
                vwDashboard.BodyAtividade = LstDashboardBodyAtividade;

                vwDashboard.token_modulo = accessToken.access_token;
                vwDashboard.url_getAcomodacao = _urlBase + string.Format("api/SituacaoAcomodacao/items/situacao/");
               

            }
            catch (Exception ex)
            {
                vwDashboard.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
            }
            return View("Dashboard", vwDashboard);
        }

        private DashboardBodyAtividadeTO CriarDashboardAtividadeZerado(List<TipoSituacaoAcomodacaoTO> pListaTipoSituacao, TipoAtividadeAcomodacaoTO pTipoAtividade)
        {
            DashboardBodyAtividadeTO ObjBodyAtividadeRetorno = new DashboardBodyAtividadeTO();
            List<DashboardBodySituacaoTO> LstDashboardBodySituacao = new List<DashboardBodySituacaoTO>();
            DashboardBodySituacaoTO ObjDashboardBodySituacao = new DashboardBodySituacaoTO();
            string strSLAAtividade = "";
            string strCorSlaAtividade = "";
            try
            {
                foreach(TipoSituacaoAcomodacaoTO item in pListaTipoSituacao)
                {
                    ObjDashboardBodySituacao = new DashboardBodySituacaoTO();
                    ObjDashboardBodySituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                    ObjDashboardBodySituacao.nome_TipoSituacaoAcomodacao = Funcoes.ToTitleCase(item.nome_TipoSituacaoAcomodacao);
                    //ObjDashboardBodySituacao.imagem = DefineImagemSLA(1, pTipoAtividade.id_TipoAtividadeAcomodacao, item.id_TipoSituacaoAcomodacao, "", ref strSLAAtividade, ref strCorSlaAtividade);
                    ObjDashboardBodySituacao.imagem = DefineImagemSLABodyDashBoard("D", ref strCorSlaAtividade);
                    ObjDashboardBodySituacao.qtD_POR_SIT = "0";
                    ObjDashboardBodySituacao.sla_SIT = strSLAAtividade; 
                    ObjDashboardBodySituacao.tempO_Utilizado_SIT = "0";
                    ObjDashboardBodySituacao.tempO_UtilizadoAt = "00:00";
                    ObjDashboardBodySituacao.cor = item.cor_TipoSituacaoAcomodacao;
                    ObjDashboardBodySituacao.ordem = item.ordem;
                    ObjDashboardBodySituacao.MaiorTempo = "00:00";

                    LstDashboardBodySituacao.Add(ObjDashboardBodySituacao);
                }
                ObjBodyAtividadeRetorno.id_TipoAtividadeAcomodacao = pTipoAtividade.id_TipoAtividadeAcomodacao;
                ObjBodyAtividadeRetorno.nome_TipoAtividadeAcomodacao = pTipoAtividade.nome_TipoAtividadeAcomodacao;
                ObjBodyAtividadeRetorno.peR_POR_ATV = "0";
                ObjBodyAtividadeRetorno.qtD_POR_ATV = "0";
                ObjBodyAtividadeRetorno.tempO_Utilizado = "00:00";
                ObjDashboardBodySituacao.MaiorTempo = "00:00";
                ObjBodyAtividadeRetorno.cor = pTipoAtividade.cor_TipoAtividadeAcomodacao;
                ObjBodyAtividadeRetorno.imagem = pTipoAtividade.imagem;
                ObjBodyAtividadeRetorno.ordem = pTipoAtividade.ordem;
                ObjBodyAtividadeRetorno.corsla = strCorSlaAtividade;
                ObjBodyAtividadeRetorno.DashboardSituacao = LstDashboardBodySituacao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ObjBodyAtividadeRetorno;
        }

        private DashboardBodyAtividadeTO CriarDashboardAtividade(List<TipoSituacaoAcomodacaoTO> pListaTipoSituacao, List<DashboardBodyTO> pDashboardBodyAtividade)
        {
            DashboardBodyAtividadeTO ObjBodyAtividadeRetorno = new DashboardBodyAtividadeTO();
            List<DashboardBodySituacaoTO> LstDashboardBodySituacao = new List<DashboardBodySituacaoTO>();
            DashboardBodySituacaoTO ObjDashboardBodySituacao = new DashboardBodySituacaoTO();
            DashboardBodyTO ObjDashboardBody = new DashboardBodyTO();
            int total_utilizado = 0;
            int total_encontrato = 0;
            string strSLAAtividade = "";
            string strCorSlaAtividade = "";
            try
            {
                foreach (TipoSituacaoAcomodacaoTO item in pListaTipoSituacao)
                {
                    ObjDashboardBodySituacao = new DashboardBodySituacaoTO();
                    ObjDashboardBody = pDashboardBodyAtividade.Where(m => m.id_TipoSituacaoAcomodacao==item.id_TipoSituacaoAcomodacao).ToList().FirstOrDefault();

                    if (ObjDashboardBody==null)
                    {
                        ObjDashboardBodySituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        ObjDashboardBodySituacao.nome_TipoSituacaoAcomodacao = Funcoes.ToTitleCase(item.nome_TipoSituacaoAcomodacao);
                        //ObjDashboardBodySituacao.imagem = DefineImagemSLA(1, pDashboardBodyAtividade[0].id_TipoAtividadeAcomodacao, item.id_TipoSituacaoAcomodacao, "", ref strSLAAtividade, ref strCorSlaAtividade);
                        ObjDashboardBodySituacao.imagem = DefineImagemSLABodyDashBoard("D", ref strCorSlaAtividade);
                        ObjDashboardBodySituacao.qtD_POR_SIT = "0";
                        ObjDashboardBodySituacao.sla_SIT = strSLAAtividade;
                        ObjDashboardBodySituacao.tempO_Utilizado_SIT = "00:00";
                        ObjDashboardBodySituacao.cor = item.cor_TipoSituacaoAcomodacao;
                        ObjDashboardBodySituacao.ordem = item.ordem;
                        ObjDashboardBodySituacao.tempO_UtilizadoAt = "00:00";
                        ObjDashboardBodySituacao.MaiorTempo = "00:00";
                    }
                    else
                    {
                        ObjDashboardBodySituacao.id_TipoSituacaoAcomodacao = ObjDashboardBody.id_TipoSituacaoAcomodacao;
                        ObjDashboardBodySituacao.nome_TipoSituacaoAcomodacao = Funcoes.ToTitleCase(ObjDashboardBody.nome_TipoSituacaoAcomodacao);
                        //ObjDashboardBodySituacao.imagem = DefineImagemSLA(decimal.Parse(ObjDashboardBody.tempO_Utilizado), ObjDashboardBody.id_TipoAtividadeAcomodacao, item.id_TipoSituacaoAcomodacao, "", ref strSLAAtividade, ref strCorSlaAtividade);
                        ObjDashboardBodySituacao.imagem = DefineImagemSLABodyDashBoard(ObjDashboardBody.FORASLA, ref strCorSlaAtividade);
                        ObjDashboardBodySituacao.qtD_POR_SIT = ObjDashboardBody.qtD_POR_SIT;
                        ObjDashboardBodySituacao.sla_SIT = strSLAAtividade;
                        ObjDashboardBodySituacao.tempO_Utilizado_SIT = FormataTempoExecucaoEmHoras(ObjDashboardBody.tempO_Utilizado);
                        ObjDashboardBodySituacao.cor = item.cor_TipoSituacaoAcomodacao;
                        ObjDashboardBodySituacao.tempO_UtilizadoAt = FormataTempoExecucaoEmHoras(ObjDashboardBody.tempO_UtilizadoAt);
                        ObjDashboardBodySituacao.MaiorTempo = FormataTempoExecucaoEmHoras(ObjDashboardBody.MaiorTempo);
                        ObjDashboardBodySituacao.ordem = item.ordem;
                        total_utilizado += int.Parse(ObjDashboardBody.tempO_UtilizadoAt);
                        total_encontrato += 1;
                    }
                    LstDashboardBodySituacao.Add(ObjDashboardBodySituacao);
                }
                ObjBodyAtividadeRetorno.id_TipoAtividadeAcomodacao = pDashboardBodyAtividade[0].id_TipoAtividadeAcomodacao;
                ObjBodyAtividadeRetorno.nome_TipoAtividadeAcomodacao = pDashboardBodyAtividade[0].nome_TipoAtividadeAcomodacao;
                ObjBodyAtividadeRetorno.peR_POR_ATV = pDashboardBodyAtividade[0].peR_POR_ATV;
                ObjBodyAtividadeRetorno.qtD_POR_ATV = pDashboardBodyAtividade[0].qtD_POR_ATV;
                if (total_encontrato > 0) {
                    ObjBodyAtividadeRetorno.tempO_Utilizado = FormataTempoExecucaoEmHoras((total_utilizado/total_encontrato).ToString());
                }
                else
                {
                    ObjBodyAtividadeRetorno.tempO_Utilizado = FormataTempoExecucaoEmHoras(total_utilizado.ToString());
                }
                ObjBodyAtividadeRetorno.corsla = strCorSlaAtividade;
                ObjBodyAtividadeRetorno.DashboardSituacao = LstDashboardBodySituacao;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ObjBodyAtividadeRetorno;
        }

        #endregion
        
        #region "Dashboard Atividade"

        public ActionResult DashboardAtividade(string id_tipoatividadeacomodacao, string pId_Setor="")
        {
            DashboardAtividadeView vwDashboardAtividade = new DashboardAtividadeView();
            List<TipoSituacaoAcomodacaoTO> LstSituacoes = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAtividadeAcomodacaoTO> LstAtividades = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoAcaoAcomodacaoTO> objLstTipoAcao = new List<TipoAcaoAcomodacaoTO>();
            List<SetorTO> LstSetores = new List<SetorTO>();
            TipoSituacaoAcomodacaoTO objTipoSituacao = new TipoSituacaoAcomodacaoTO();
            List<DashboardAtividadeTO> objLstDashboardAtividade = new List<DashboardAtividadeTO>();
            List<DashboardAtividadeTO> objLstDashboardAtividadeFiltro = new List<DashboardAtividadeTO>();
            List<DashboardAtividade_SituacaoTO> objLstDashboardAtividadeSituacao = new List<DashboardAtividade_SituacaoTO>();
            DashboardAtividade_SituacaoTO objDashboardAtividadeSituacao = new DashboardAtividade_SituacaoTO();
            List<DashboardAtividade_AcomodacaoTO> objLstDashboardAtividadeAcomodacao = new List<DashboardAtividade_AcomodacaoTO>();
            DashboardAtividade_AcomodacaoTO objDashboardAtividadeAcomodacao = new DashboardAtividade_AcomodacaoTO();
           
            string strSLAAtividade = "";
            string strCorSlaAtividade = "";
            string lPerfilUsuario = Session["id_PerfilSel"].ToString();
            string lPerfilTipoUsuarioPerfil = Session["cod_TipoUsuarioPerfilSel"].ToString();
            string lPerfilAdministrador = ConfigurationManager.AppSettings["CodPerfilAdministrador"];
            try
            {
                LstSituacoes = base.ListarTipoSituacaoAcomodacao();
                LstAtividades = base.ListarTipoAtividadeAcomodacao();
                LstSetores = base.ListarSetores(Session["id_EmpresaSel"].ToString());
                objLstTipoAcao = base.ListarTipoAcaoAcomodacao();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                string url = string.Format("api/Gestao/items/dashboard/empresa/{0}/tipoatividade/{1}", Session["id_EmpresaSel"].ToString(), id_tipoatividadeacomodacao);

                objLstDashboardAtividadeFiltro = base.Listar<DashboardAtividadeTO>(accessToken, _urlBase, url);

                if (!string.IsNullOrWhiteSpace(pId_Setor))
                {
                    objLstDashboardAtividade = objLstDashboardAtividadeFiltro.Where(m => m.id_Setor == pId_Setor).ToList();
                }
                else
                {
                    objLstDashboardAtividade = objLstDashboardAtividadeFiltro;
                }

                string id_tiposituacaoAtual = "";
                foreach (DashboardAtividadeTO item in objLstDashboardAtividade.OrderBy(m => m.id_TipoSituacaoAcomodacao))
                {
                    if (id_tiposituacaoAtual != item.id_TipoSituacaoAcomodacao)
                    {
                        if (!string.IsNullOrWhiteSpace(item.id_TipoSituacaoAcomodacao))
                        {
                            objTipoSituacao = new TipoSituacaoAcomodacaoTO();
                            objTipoSituacao = LstSituacoes.Where(m => m.id_TipoSituacaoAcomodacao == item.id_TipoSituacaoAcomodacao).FirstOrDefault();
                            if (objTipoSituacao != null)
                            {
                                item.imagem = objTipoSituacao.imagem;
                                item.ordem = objTipoSituacao.ordem;
                                item.cor = objTipoSituacao.cor_TipoSituacaoAcomodacao;
                            }
                            else
                            {
                                item.imagem = "Sem Situacao";
                                item.cor = "Sem Cor";
                            }
                        }

                        objDashboardAtividadeSituacao = new DashboardAtividade_SituacaoTO();
                        objDashboardAtividadeSituacao.id_TipoAtividadeAcomodacao = item.id_TipoAtividadeAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoAtividadeAcomodacao = item.nome_TipoAtividadeAcomodacao;
                        objDashboardAtividadeSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.imagem = item.imagem;
                        objDashboardAtividadeSituacao.ordem = item.ordem;
                        objDashboardAtividadeSituacao.cor = item.cor;
                        objLstDashboardAtividadeAcomodacao = new List<DashboardAtividade_AcomodacaoTO>();
                        foreach (DashboardAtividadeTO leito in objLstDashboardAtividade.Where(m => m.id_TipoSituacaoAcomodacao==item.id_TipoSituacaoAcomodacao))
                        {
                            objDashboardAtividadeAcomodacao = new DashboardAtividade_AcomodacaoTO();

                            objDashboardAtividadeAcomodacao.slA_ATIVIDADE = leito.slA_ATIVIDADE;
                            objDashboardAtividadeAcomodacao.tempo_Utilizado_Atividade = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Atividade);
                            objDashboardAtividadeAcomodacao.nome_Acomodacao = leito.nome_Acomodacao;
                            objDashboardAtividadeAcomodacao.nome_Setor = leito.nome_Setor;
                            objDashboardAtividadeAcomodacao.id_Setor = leito.id_Setor;
                            objDashboardAtividadeAcomodacao.nome_TipoAcaoAcomodacao = leito.nome_TipoAcaoAcomodacao;
                            if (leito.nome_TipoAcaoAcomodacao != "")
                            {
                                objDashboardAtividadeAcomodacao.nome_Status_Label = objLstTipoAcao.Where(m => m.nome_TipoAcaoAcomodacao == leito.nome_TipoAcaoAcomodacao).FirstOrDefault().nome_Status_Label;
                            }
                            objDashboardAtividadeAcomodacao.slA_ACAO = leito.slA_ACAO;
                            objDashboardAtividadeAcomodacao.tempo_Utilizado_Acao = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Acao);
                            objDashboardAtividadeAcomodacao.cod_Prioritario = leito.cod_Prioritario;
                            objDashboardAtividadeAcomodacao.id_Acomodacao = leito.id_Acomodacao;
                            strSLAAtividade = leito.slA_ATIVIDADE;
                            strCorSlaAtividade = "";
                            //objDashboardAtividadeAcomodacao.imagem_sla = DefineImagemSLA(decimal.Parse(leito.tempo_Utilizado_Atividade), id_tipoatividadeacomodacao, item.id_TipoSituacaoAcomodacao, leito.tio ref strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardAtividadeAcomodacao.imagem_sla = DefineImagemSLACalculado(decimal.Parse(leito.tempo_Utilizado_Atividade), strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardAtividadeAcomodacao.imagem_prioridade = DefineImagemPrioridade(leito.prioritarioAtividade);
                            objDashboardAtividadeAcomodacao.imagem_isolamento = DefineImagemIsolamento(leito.cod_Isolamento);
                            objDashboardAtividadeAcomodacao.imagem_limpezaplus = DefineImagemLimpezaPlus(leito.cod_Plus);
                            objDashboardAtividadeAcomodacao.prioritarioAtividade = leito.prioritarioAtividade;
                            objDashboardAtividadeAcomodacao.cod_Isolamento = leito.cod_Isolamento;
                            objDashboardAtividadeAcomodacao.cod_Plus = leito.cod_Plus;
                            objDashboardAtividadeAcomodacao.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, id_tipoatividadeacomodacao);
                            objDashboardAtividadeAcomodacao.id_PerfilAdministrador = lPerfilTipoUsuarioPerfil;
                            objDashboardAtividadeAcomodacao.id_PerfilUsuario = lPerfilUsuario;
                            objDashboardAtividadeAcomodacao.pendenciaFinanceira = leito.pendenciaFinanceira;
                            objDashboardAtividadeAcomodacao.imagem_pendencia_financeira = DefineImagemPendenciaFinanceira(leito.pendenciaFinanceira);

                            objLstDashboardAtividadeAcomodacao.Add(objDashboardAtividadeAcomodacao);
                        }

                        objDashboardAtividadeSituacao.Acomodacoes = objLstDashboardAtividadeAcomodacao;

                        objLstDashboardAtividadeSituacao.Add(objDashboardAtividadeSituacao);

                    }


                    id_tiposituacaoAtual = item.id_TipoSituacaoAcomodacao;
                }

                foreach (TipoSituacaoAcomodacaoTO item in LstSituacoes) {
                    if (objLstDashboardAtividadeSituacao.Where(m => m.id_TipoSituacaoAcomodacao==item.id_TipoSituacaoAcomodacao).ToList().Count==0)
                    {
                        objDashboardAtividadeSituacao = new DashboardAtividade_SituacaoTO();
                        objDashboardAtividadeSituacao.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                        objDashboardAtividadeSituacao.nome_TipoAtividadeAcomodacao = LstAtividades.Where(m => m.id_TipoAtividadeAcomodacao==id_tipoatividadeacomodacao).FirstOrDefault().nome_TipoAtividadeAcomodacao;
                        objDashboardAtividadeSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.imagem = item.imagem;
                        objDashboardAtividadeSituacao.ordem = item.ordem;
                        objDashboardAtividadeSituacao.cor = item.cor_TipoSituacaoAcomodacao;
                        objLstDashboardAtividadeAcomodacao = new List<DashboardAtividade_AcomodacaoTO>();
                        objDashboardAtividadeSituacao.Acomodacoes = objLstDashboardAtividadeAcomodacao;
                        objLstDashboardAtividadeSituacao.Add(objDashboardAtividadeSituacao);
                    }

                }
                
                vwDashboardAtividade.LstTipoAtividade = LstAtividades;
                vwDashboardAtividade.LstSetor = LstSetores;
                vwDashboardAtividade.id_Setor = pId_Setor;
                vwDashboardAtividade.id_TipoAtividade = id_tipoatividadeacomodacao;
                vwDashboardAtividade.IdUsuarioLogado = Session["id_Usuario"].ToString();

                vwDashboardAtividade.DashboardAtividade = objLstDashboardAtividadeSituacao;

                //vwDashboardAtividade.token_modulo = accessToken.access_token;
                vwDashboardAtividade.tkn_operacional = accessToken.access_token;
                
                /// TODAS DO  OPERACIONAL
                vwDashboardAtividade.url_priorizaAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/prioridade/"); ///Complemento --- S?idAtividade=12222
                vwDashboardAtividade.url_limpezaPlusAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/plus/"); ///Complemento --- S?idAtividade=12222
                vwDashboardAtividade.url_getHistoricoAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/det/empresa/{0}/situacao/", Session["id_EmpresaSel"].ToString()); ///Complemento --- S?idAtividade=12222
                vwDashboardAtividade.url_EncaminharAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items?idEmpresa={0}&idUsuario={1}&", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardAtividade.url_CancelarAtividade = _urlBase + string.Format("api/AcaoAtividadeAcomodacao/items/cancelargenerico/empresa/{0}/usuario/{1}/atividade/", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardAtividade.url_EncaminharSituacao = _urlBase + string.Format("api/Integracao/items/ajuste/empresa/{0}", Session["id_EmpresaSel"].ToString());
                vwDashboardAtividade.url_getMensagens = _urlBase + "api/Mensagem/items/mensagem/idatividade/";
                vwDashboardAtividade.url_sendMensagem = _urlBase + "api/Mensagem/items/mensagem";
                //1 / codexternoacomodacao / 0002 / situacaodestino / 3

                /// MUDA PARA ADMINISTRATIVO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                vwDashboardAtividade.tkn_administrativo = accessToken.access_token;
                vwDashboardAtividade.url_getAcomodacao = _urlBase + string.Format("api/Acomodacao/items/acomodacaodetalhe/");
                vwDashboardAtividade.url_isolaAcomodacao = _urlBase + string.Format("api/Acomodacao/items/atividade/isolar/"); /// Complemento --- S?idAcomodacao=19

                /// MUDA PARA CONFIGURACAO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                vwDashboardAtividade.tkn_configuracao = accessToken.access_token;
                vwDashboardAtividade.url_listatipoatividade = _urlBase + string.Format("api/TipoAtividadeAcomodacao/items");
                vwDashboardAtividade.url_listatiposituacao = _urlBase + string.Format("api/TipoSituacaoAcomodacao/items");
                ViewBag.percentualAtencaoSLA = ConfigurationManager.AppSettings["percentualAtencaoSLA"];
            }
            catch (Exception ex)
            {
                vwDashboardAtividade.erro = "Erro ao carregar dashboard de atividades. Erro:" + ex.Message;
            }
            return View("DashboardAtividade", vwDashboardAtividade);
        }

        #endregion

        #region "Dashboard Situação"

        public ActionResult DashboardSituacao(string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao="", string pId_Setor = "")
        {
            DashboardSituacaoView vwDashboardSituacao = new DashboardSituacaoView();
            List<TipoSituacaoAcomodacaoTO> LstSituacoes = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAtividadeAcomodacaoTO> LstAtividades = new List<TipoAtividadeAcomodacaoTO>();
            TipoAtividadeAcomodacaoTO objTipoAtividade = new TipoAtividadeAcomodacaoTO();
            TipoSituacaoAcomodacaoTO objTipoSituacao = new TipoSituacaoAcomodacaoTO();
            List<TipoAcaoAcomodacaoTO> objLstTipoAcao = new List<TipoAcaoAcomodacaoTO>();
            List<SetorTO> LstSetores = new List<SetorTO>();
            List<DashboardSituacaoTO> objLstDashboardSituacao = new List<DashboardSituacaoTO>();
            List<DashboardSituacaoTO> objLstDashboardSituacaoFiltro = new List<DashboardSituacaoTO>();
            List<DashboardSituacao_AtividadeTO> objLstDashboardSituacaoSituacao = new List<DashboardSituacao_AtividadeTO>();
            DashboardSituacao_AtividadeTO objDashboardSituacaoSituacao = new DashboardSituacao_AtividadeTO();
            List<DashboardSituacao_AtividadeTO> objLstDashboardSituacaoBacklog = new List<DashboardSituacao_AtividadeTO>();
            List<DashboardSituacao_AcomodacaoTO> objLstDashboardSituacaoAcomodacao = new List<DashboardSituacao_AcomodacaoTO>();
            DashboardSituacao_AcomodacaoTO objDashboardSituacaoAcomodacao = new DashboardSituacao_AcomodacaoTO();
            string strSLAAtividade = "";
            string strCorSlaAtividade = "";
            string lPerfilUsuario = Session["id_PerfilSel"].ToString();
            string lPerfilTipoUsuarioPerfil = Session["cod_TipoUsuarioPerfilSel"].ToString();
            string lPerfilAdministrador = ConfigurationManager.AppSettings["CodPerfilAdministrador"];
            try
            {
                LstSituacoes = base.ListarTipoSituacaoAcomodacao();
                LstAtividades = base.ListarTipoAtividadeAcomodacao();
                LstSetores = base.ListarSetores(Session["id_EmpresaSel"].ToString());
                objLstTipoAcao = base.ListarTipoAcaoAcomodacao();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                string url = string.Format("api/Gestao/items/dashboard/empresa/{0}/tiposituacao/{1}", Session["id_EmpresaSel"].ToString(), id_tiposituacaoacomodacao);

                objLstDashboardSituacaoFiltro = base.Listar<DashboardSituacaoTO>(accessToken, _urlBase, url);



                if (!string.IsNullOrWhiteSpace(pId_Setor) && !string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao))
                {
                    objLstDashboardSituacao = objLstDashboardSituacaoFiltro.Where(m => m.id_Setor == pId_Setor && m.id_TipoAtividadeAcomodacao == id_tipoatividadeacomodacao).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(pId_Setor))
                {
                    objLstDashboardSituacao = objLstDashboardSituacaoFiltro.Where(m => m.id_Setor == pId_Setor).ToList();
                }
                else if(!string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao))
                {
                    objLstDashboardSituacao = objLstDashboardSituacaoFiltro.Where(m => m.id_TipoAtividadeAcomodacao == id_tipoatividadeacomodacao).ToList();
                }
                else
                {
                    objLstDashboardSituacao = objLstDashboardSituacaoFiltro;
                }


                string id_tipoAtividadeAtual = "";
                foreach (DashboardSituacaoTO item in objLstDashboardSituacao.Where(n => decimal.Parse(n.id_TipoAtividadeAcomodacao) > 0).OrderBy(m => m.id_TipoAtividadeAcomodacao))
                {
                    if (id_tipoAtividadeAtual != item.id_TipoAtividadeAcomodacao)
                    {
                        if (!string.IsNullOrWhiteSpace(item.id_TipoAtividadeAcomodacao))
                        {
                            objTipoAtividade = new TipoAtividadeAcomodacaoTO();
                            objTipoAtividade = LstAtividades.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao).FirstOrDefault();
                            if (objTipoAtividade != null)
                            {
                                item.imagem = objTipoAtividade.imagem;
                                item.cor = objTipoAtividade.cor_TipoAtividadeAcomodacao;
                                item.ordem = objTipoAtividade.ordem;
                            }
                            else
                            {
                                item.imagem = "Sem Atividade";
                                item.cor = "Sem Cor";
                            }
                        }

                        objDashboardSituacaoSituacao = new DashboardSituacao_AtividadeTO();
                        objDashboardSituacaoSituacao.id_TipoAtividadeAcomodacao = item.id_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.nome_TipoAtividadeAcomodacao = item.nome_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardSituacaoSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardSituacaoSituacao.imagem = item.imagem;
                        objDashboardSituacaoSituacao.cor = item.cor;
                        objDashboardSituacaoSituacao.ordem = item.ordem;
                        objLstDashboardSituacaoAcomodacao = new List<DashboardSituacao_AcomodacaoTO>();
                        foreach (DashboardSituacaoTO leito in objLstDashboardSituacao.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao))
                        {
                            objDashboardSituacaoAcomodacao = new DashboardSituacao_AcomodacaoTO();

                            objDashboardSituacaoAcomodacao.slA_ATIVIDADE = leito.slA_ATIVIDADE;
                            objDashboardSituacaoAcomodacao.tempo_Utilizado_Atividade = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Atividade);
                            objDashboardSituacaoAcomodacao.nome_Acomodacao = leito.nome_Acomodacao;
                            objDashboardSituacaoAcomodacao.nome_Setor = leito.nome_Setor;
                            objDashboardSituacaoAcomodacao.nome_TipoAcaoAcomodacao = leito.nome_TipoAcaoAcomodacao;
                            if (leito.nome_TipoAcaoAcomodacao != "")
                            {
                                objDashboardSituacaoAcomodacao.nome_Status_Label = objLstTipoAcao.Where(m => m.nome_TipoAcaoAcomodacao == leito.nome_TipoAcaoAcomodacao).FirstOrDefault().nome_Status_Label;
                            }
                            objDashboardSituacaoAcomodacao.slA_ACAO = leito.slA_ACAO;
                            objDashboardSituacaoAcomodacao.tempo_Utilizado_Acao = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Acao);
                            objDashboardSituacaoAcomodacao.cod_Prioritario = leito.cod_Prioritario;
                            objDashboardSituacaoAcomodacao.id_Acomodacao = leito.id_Acomodacao;
                            strSLAAtividade = leito.slA_ATIVIDADE;
                            strCorSlaAtividade = "";
                            //objDashboardSituacaoAcomodacao.imagem_sla = DefineImagemSLA(decimal.Parse(leito.tempo_Utilizado_Atividade), id_tiposituacaoacomodacao, item.id_TipoSituacaoAcomodacao, ref strSLAAtividade, ref strCorSlaAtividade);
                            
                            objDashboardSituacaoAcomodacao.imagem_sla = DefineImagemSLACalculado(decimal.Parse(leito.tempo_Utilizado_Atividade), strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardSituacaoAcomodacao.imagem_prioridade = DefineImagemPrioridade(leito.prioritarioAtividade);
                            objDashboardSituacaoAcomodacao.imagem_isolamento = DefineImagemIsolamento(leito.cod_Isolamento);
                            objDashboardSituacaoAcomodacao.imagem_limpezaplus = DefineImagemLimpezaPlus(leito.cod_Plus);
                            objDashboardSituacaoAcomodacao.prioritarioAtividade = leito.prioritarioAtividade;
                            objDashboardSituacaoAcomodacao.cod_Isolamento = leito.cod_Isolamento;
                            objDashboardSituacaoAcomodacao.cod_Plus = leito.cod_Plus;
                            

                            objDashboardSituacaoAcomodacao.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, item.id_TipoAtividadeAcomodacao);
                            objDashboardSituacaoAcomodacao.id_PerfilAdministrador = lPerfilTipoUsuarioPerfil;
                            objDashboardSituacaoAcomodacao.id_PerfilUsuario = lPerfilUsuario;

                            objDashboardSituacaoAcomodacao.pendenciaFinanceira = leito.pendenciaFinanceira;
                            objDashboardSituacaoAcomodacao.imagem_pendencia_financeira = DefineImagemPendenciaFinanceira(leito.pendenciaFinanceira);

                            objLstDashboardSituacaoAcomodacao.Add(objDashboardSituacaoAcomodacao);
                        }

                        objDashboardSituacaoSituacao.Acomodacoes = objLstDashboardSituacaoAcomodacao;

                        objLstDashboardSituacaoSituacao.Add(objDashboardSituacaoSituacao);

                    }


                    id_tipoAtividadeAtual = item.id_TipoAtividadeAcomodacao;
                }

                foreach (TipoAtividadeAcomodacaoTO item in LstAtividades)
                {
                    if (objLstDashboardSituacaoSituacao.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao).ToList().Count == 0)
                    {
                        objDashboardSituacaoSituacao = new DashboardSituacao_AtividadeTO();
                        objDashboardSituacaoSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                        objDashboardSituacaoSituacao.nome_TipoSituacaoAcomodacao = LstSituacoes.Where(m => m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).FirstOrDefault().nome_TipoSituacaoAcomodacao;
                        objDashboardSituacaoSituacao.id_TipoAtividadeAcomodacao = item.id_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.nome_TipoAtividadeAcomodacao = item.nome_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.imagem = item.imagem;
                        objDashboardSituacaoSituacao.cor = item.cor_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.ordem = item.ordem;
                        objLstDashboardSituacaoAcomodacao = new List<DashboardSituacao_AcomodacaoTO>();
                        objDashboardSituacaoSituacao.Acomodacoes = objLstDashboardSituacaoAcomodacao;
                        objLstDashboardSituacaoSituacao.Add(objDashboardSituacaoSituacao);
                    }

                }

                objTipoSituacao = new TipoSituacaoAcomodacaoTO();
                objTipoSituacao = LstSituacoes.Where(m => m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).FirstOrDefault();
                objLstDashboardSituacaoBacklog = new List<DashboardSituacao_AtividadeTO>();
                id_tipoAtividadeAtual = "";
                foreach (DashboardSituacaoTO item in objLstDashboardSituacao.Where(n => decimal.Parse(n.id_TipoAtividadeAcomodacao) == 0))
                {
                    if (id_tipoAtividadeAtual != item.id_TipoAtividadeAcomodacao)
                    {
                        if (objTipoSituacao != null)
                        {
                            item.imagem = objTipoSituacao.imagem;
                            item.cor = objTipoSituacao.cor_TipoSituacaoAcomodacao;
                            item.ordem = objTipoSituacao.ordem;
                        }
                        else
                        {
                            item.imagem = "Sem Atividade";
                            item.cor = "Sem Cor";
                            item.ordem = 0;
                        }

                        objDashboardSituacaoSituacao = new DashboardSituacao_AtividadeTO();
                        objDashboardSituacaoSituacao.id_TipoAtividadeAcomodacao = item.id_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.nome_TipoAtividadeAcomodacao = item.nome_TipoAtividadeAcomodacao;
                        objDashboardSituacaoSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardSituacaoSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardSituacaoSituacao.imagem = item.imagem;
                        objDashboardSituacaoSituacao.cor = item.cor;
                        objDashboardSituacaoSituacao.ordem = item.ordem;
                        objLstDashboardSituacaoAcomodacao = new List<DashboardSituacao_AcomodacaoTO>();
                        foreach (DashboardSituacaoTO leito in objLstDashboardSituacao.Where(m => m.id_TipoAtividadeAcomodacao == item.id_TipoAtividadeAcomodacao))
                        {
                            objDashboardSituacaoAcomodacao = new DashboardSituacao_AcomodacaoTO();

                            objDashboardSituacaoAcomodacao.slA_ATIVIDADE = leito.slA_ATIVIDADE;
                            objDashboardSituacaoAcomodacao.tempo_Utilizado_Atividade = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Atividade);
                            objDashboardSituacaoAcomodacao.nome_Acomodacao = leito.nome_Acomodacao;
                            objDashboardSituacaoAcomodacao.nome_Setor = leito.nome_Setor;
                            objDashboardSituacaoAcomodacao.nome_TipoAcaoAcomodacao = leito.nome_TipoAcaoAcomodacao;
                            objDashboardSituacaoAcomodacao.nome_Status_Label = leito.nome_Status_Label;
                            objDashboardSituacaoAcomodacao.slA_ACAO = leito.slA_ACAO;
                            objDashboardSituacaoAcomodacao.tempo_Utilizado_Acao = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Acao);
                            objDashboardSituacaoAcomodacao.cod_Prioritario = leito.cod_Prioritario;
                            objDashboardSituacaoAcomodacao.id_Acomodacao = leito.id_Acomodacao;
                            strSLAAtividade = leito.slA_ATIVIDADE;
                            strCorSlaAtividade = "";
                            //objDashboardSituacaoAcomodacao.imagem_sla = DefineImagemSLA(decimal.Parse(leito.tempo_Utilizado_Atividade), id_tiposituacaoacomodacao, item.id_TipoSituacaoAcomodacao, ref strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardSituacaoAcomodacao.imagem_sla = DefineImagemSLACalculado(decimal.Parse(leito.tempo_Utilizado_Atividade), strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardSituacaoAcomodacao.imagem_prioridade = DefineImagemPrioridade(leito.prioritarioAtividade);
                            objDashboardSituacaoAcomodacao.imagem_isolamento = DefineImagemIsolamento(leito.cod_Isolamento);
                            objDashboardSituacaoAcomodacao.imagem_limpezaplus = DefineImagemLimpezaPlus(leito.cod_Plus);
                            objDashboardSituacaoAcomodacao.prioritarioAtividade = leito.prioritarioAtividade;
                            objDashboardSituacaoAcomodacao.cod_Isolamento = leito.cod_Isolamento;
                            objDashboardSituacaoAcomodacao.cod_Plus = leito.cod_Plus;
                            objDashboardSituacaoAcomodacao.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, item.id_TipoAtividadeAcomodacao);
                            objDashboardSituacaoAcomodacao.id_PerfilAdministrador = lPerfilTipoUsuarioPerfil;
                            objDashboardSituacaoAcomodacao.id_PerfilUsuario = lPerfilUsuario;

                            objDashboardSituacaoAcomodacao.pendenciaFinanceira = leito.pendenciaFinanceira;
                            objDashboardSituacaoAcomodacao.imagem_pendencia_financeira = DefineImagemPendenciaFinanceira(leito.pendenciaFinanceira);

                            objLstDashboardSituacaoAcomodacao.Add(objDashboardSituacaoAcomodacao);
                        }

                        objDashboardSituacaoSituacao.Acomodacoes = objLstDashboardSituacaoAcomodacao;

                        objLstDashboardSituacaoBacklog.Add(objDashboardSituacaoSituacao);

                    }
                    break;
                    ///id_tipoAtividadeAtual = item.id_TipoAtividadeAcomodacao;

                }
                if (objLstDashboardSituacaoBacklog.Count==0)
                {
                    objDashboardSituacaoSituacao = new DashboardSituacao_AtividadeTO();
                    objDashboardSituacaoSituacao.id_TipoAtividadeAcomodacao = "0";
                    objDashboardSituacaoSituacao.nome_TipoAtividadeAcomodacao = "";
                    objDashboardSituacaoSituacao.id_TipoSituacaoAcomodacao = objTipoSituacao.id_TipoSituacaoAcomodacao;
                    objDashboardSituacaoSituacao.nome_TipoSituacaoAcomodacao = objTipoSituacao.nome_TipoSituacaoAcomodacao;
                    objDashboardSituacaoSituacao.imagem = objTipoSituacao.imagem;
                    objDashboardSituacaoSituacao.cor = objTipoSituacao.cor_TipoSituacaoAcomodacao;
                    objDashboardSituacaoSituacao.ordem = objTipoSituacao.ordem;
                    objLstDashboardSituacaoAcomodacao = new List<DashboardSituacao_AcomodacaoTO>();
                    objLstDashboardSituacaoBacklog.Add(objDashboardSituacaoSituacao);
                }

                if (!string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao))
                {

                }
                if (!string.IsNullOrWhiteSpace(pId_Setor))
                {

                }
                vwDashboardSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                vwDashboardSituacao.LstTipoAtividade = LstAtividades;
                vwDashboardSituacao.LstSetor = LstSetores;
                vwDashboardSituacao.id_Setor = pId_Setor;
                vwDashboardSituacao.id_TipoAtividade = id_tipoatividadeacomodacao;
                
                vwDashboardSituacao.DashboardSituacao = objLstDashboardSituacaoSituacao;
                vwDashboardSituacao.DashboardSituacaoBacklog = objLstDashboardSituacaoBacklog;
                //vwDashboardSituacao.token_modulo = accessToken.access_token;
                vwDashboardSituacao.tkn_operacional = accessToken.access_token;
                vwDashboardSituacao.IdUsuarioLogado = Session["id_Usuario"].ToString();

                /// TODAS DO  OPERACIONAL
                vwDashboardSituacao.url_priorizaAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/prioridade/"); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacao.url_limpezaPlusAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/plus/"); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacao.url_getHistoricoAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/det/empresa/{0}/situacao/", Session["id_EmpresaSel"].ToString()); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacao.url_EncaminharAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items?idEmpresa={0}&idUsuario={1}&", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardSituacao.url_GerarAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade?idEmpresa={0}", Session["id_EmpresaSel"].ToString());
                vwDashboardSituacao.url_CancelarAtividade = _urlBase + string.Format("api/AcaoAtividadeAcomodacao/items/cancelargenerico/empresa/{0}/usuario/{1}/atividade/", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardSituacao.url_EncaminharSituacao = _urlBase + string.Format("api/Integracao/items/ajuste/empresa/{0}", Session["id_EmpresaSel"].ToString());
                vwDashboardSituacao.url_getMensagens = _urlBase + "api/Mensagem/items/mensagem/idatividade/";
                vwDashboardSituacao.url_sendMensagem = _urlBase + "api/Mensagem/items/mensagem";

                //1 / codexternoacomodacao / 0002 / situacaodestino / 3

                /// MUDA PARA ADMINISTRATIVO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                vwDashboardSituacao.tkn_administrativo = accessToken.access_token;
                vwDashboardSituacao.url_getAcomodacao = _urlBase + string.Format("api/Acomodacao/items/acomodacaodetalhe/");
                vwDashboardSituacao.url_isolaAcomodacao = _urlBase + string.Format("api/Acomodacao/items/atividade/isolar/"); /// Complemento --- S?idAcomodacao=19

                /// MUDA PARA CONFIGURACAO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                vwDashboardSituacao.tkn_configuracao = accessToken.access_token;
                vwDashboardSituacao.url_listatipoatividade = _urlBase + string.Format("api/TipoAtividadeAcomodacao/items");
                vwDashboardSituacao.url_listatiposituacao = _urlBase + string.Format("api/TipoSituacaoAcomodacao/items");
                ViewBag.percentualAtencaoSLA = ConfigurationManager.AppSettings["percentualAtencaoSLA"];

            }
            catch (Exception ex)
            {
                vwDashboardSituacao.erro = "Erro ao carregar dashboard de situações. Erro:" + ex.Message;
            }
            return View("DashboardSituacao", vwDashboardSituacao);
        }

        #endregion




        #region "Dashboard Atividade"

        public ActionResult DashboardSituacaoAtividade(string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao, string pId_Setor = "")
        {
            DashboardSituacaoAtividadeView vwDashboardSituacaoAtividade = new DashboardSituacaoAtividadeView();
            List<TipoSituacaoAcomodacaoTO> LstSituacoes = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAtividadeAcomodacaoTO> LstAtividades = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoAcaoAcomodacaoTO> LstAcoes = new List<TipoAcaoAcomodacaoTO>();
            List<SetorTO> LstSetores = new List<SetorTO>();
            TipoAcaoAcomodacaoTO objTipoAcao = new TipoAcaoAcomodacaoTO();
            List<DashboardSituacaoAtividadeTO> objLstDashboardAtividade = new List<DashboardSituacaoAtividadeTO>();
            List<DashboardSituacaoAtividadeTO> objLstDashboardAtividadeFiltro = new List<DashboardSituacaoAtividadeTO>();
            List<DashboardSituacaoAtividade_AcaoTO> objLstDashboardAtividadeSituacao = new List<DashboardSituacaoAtividade_AcaoTO>();
            DashboardSituacaoAtividade_AcaoTO objDashboardAtividadeSituacao = new DashboardSituacaoAtividade_AcaoTO();
            List<DashboardSituacaoAtividade_AcomodacaoTO> objLstDashboardAtividadeAcomodacao = new List<DashboardSituacaoAtividade_AcomodacaoTO>();
            DashboardSituacaoAtividade_AcomodacaoTO objDashboardAtividadeAcomodacao = new DashboardSituacaoAtividade_AcomodacaoTO();
            
            string strSLAAtividade = "";
            string strCorSlaAtividade = "";
            string lPerfilUsuario = Session["id_PerfilSel"].ToString();
            string lPerfilTipoUsuarioPerfil = Session["cod_TipoUsuarioPerfilSel"].ToString();
            string lPerfilAdministrador = ConfigurationManager.AppSettings["CodPerfilAdministrador"];
            try
            {

                LstSituacoes = base.ListarTipoSituacaoAcomodacao();
                LstAtividades = base.ListarTipoAtividadeAcomodacao();
                LstSetores = base.ListarSetores(Session["id_EmpresaSel"].ToString());
                LstAcoes = base.ListarTipoAcaoAcomodacao();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                string url = string.Format("api/Gestao/items/dashboard/empresa/{0}/tipoatividade/{1}/tiposituacao/{2}", Session["id_EmpresaSel"].ToString(), id_tipoatividadeacomodacao, id_tiposituacaoacomodacao);

                objLstDashboardAtividadeFiltro = base.Listar<DashboardSituacaoAtividadeTO>(accessToken, _urlBase, url);

                if (!string.IsNullOrWhiteSpace(pId_Setor))
                {
                    objLstDashboardAtividade = objLstDashboardAtividadeFiltro.Where(m => m.Id_Setor == pId_Setor).ToList();
                }
                else
                {
                    objLstDashboardAtividade = objLstDashboardAtividadeFiltro;
                }

                string id_tipoAcaoAtual = "";
                foreach (DashboardSituacaoAtividadeTO item in objLstDashboardAtividade.OrderBy(m => m.id_TipoAcaoAcomodacao))
                {
                    if (id_tipoAcaoAtual != item.id_TipoAcaoAcomodacao)
                    {
                        if (!string.IsNullOrWhiteSpace(item.id_TipoAcaoAcomodacao))
                        {
                            objTipoAcao = new TipoAcaoAcomodacaoTO();
                            objTipoAcao = LstAcoes.Where(m => m.id_TipoAcaoAcomodacao == item.id_TipoAcaoAcomodacao).FirstOrDefault();
                            if (objTipoAcao != null)
                            {
                                item.imagem = objTipoAcao.imagem;
                                item.cor = objTipoAcao.cor_TipoAcaoAcomodacao;
                                item.ordemAcao = objTipoAcao.ordem;
                                item.nome_status_Label = objTipoAcao.nome_Status_Label;
                            }
                            else
                            {
                                item.imagem = "Sem Situacao";
                                item.cor = "Sem Cor";
                                item.ordemAcao = 0;
                                item.nome_status_Label = "Sem Label";
                            }
                        }

                        objDashboardAtividadeSituacao = new DashboardSituacaoAtividade_AcaoTO();
                        objDashboardAtividadeSituacao.id_TipoAtividadeAcomodacao = item.id_TipoAtividadeAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoAtividadeAcomodacao = item.nome_TipoAtividadeAcomodacao;
                        objDashboardAtividadeSituacao.id_TipoSituacaoAcomodacao = item.id_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoSituacaoAcomodacao = item.nome_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.id_TipoAcaoAcomodacao = item.id_TipoAcaoAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoAcaoAcomodacao = item.nome_status_Label; // item.nome_TipoAcaoAcomodacao;
                        objDashboardAtividadeSituacao.imagem = item.imagem;
                        objDashboardAtividadeSituacao.cor = item.cor;
                        objDashboardAtividadeSituacao.ordemAcao = item.ordemAcao;
                        objLstDashboardAtividadeAcomodacao = new List<DashboardSituacaoAtividade_AcomodacaoTO>();
                        foreach (DashboardSituacaoAtividadeTO leito in objLstDashboardAtividade.Where(m => m.id_TipoAcaoAcomodacao == item.id_TipoAcaoAcomodacao))
                        {
                            objDashboardAtividadeAcomodacao = new DashboardSituacaoAtividade_AcomodacaoTO();

                            objDashboardAtividadeAcomodacao.slA_ATIVIDADE = leito.slaAtividade;
                            objDashboardAtividadeAcomodacao.tempo_Utilizado_Atividade = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Atividade);
                            objDashboardAtividadeAcomodacao.nome_Acomodacao = leito.nome_Acomodacao;
                            objDashboardAtividadeAcomodacao.nome_Setor = leito.nome_Setor;
                            objDashboardAtividadeAcomodacao.id_Setor = leito.Id_Setor;
                            objDashboardAtividadeAcomodacao.nome_TipoAcaoAcomodacao = leito.nome_TipoAcaoAcomodacao;
                            objDashboardAtividadeAcomodacao.nome_Status_Label = leito.nome_status_Label;
                            objDashboardAtividadeAcomodacao.slA_ACAO = leito.slaAcao;
                            objDashboardAtividadeAcomodacao.tempo_Utilizado_Acao = FormataTempoExecucaoEmHoras(leito.tempo_Utilizado_Acao);
                            objDashboardAtividadeAcomodacao.cod_Prioritario = leito.prioritarioAtividade;
                            objDashboardAtividadeAcomodacao.id_Acomodacao = leito.id_Acomodacao;
                            strSLAAtividade = leito.slaAtividade;
                            strCorSlaAtividade = "";
                            //objDashboardAtividadeAcomodacao.imagem_sla = DefineImagemSLA(decimal.Parse(leito.tempoPercorrido), id_tipoatividadeacomodacao, item.id_TipoSituacaoAcomodacao, ref strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardAtividadeAcomodacao.imagem_sla = DefineImagemSLACalculado(decimal.Parse(leito.tempo_Utilizado_Atividade), strSLAAtividade, ref strCorSlaAtividade);
                            objDashboardAtividadeAcomodacao.imagem_prioridade = DefineImagemPrioridade(leito.prioritarioAtividade);
                            objDashboardAtividadeAcomodacao.imagem_isolamento = DefineImagemIsolamento(leito.cod_Isolamento);
                            objDashboardAtividadeAcomodacao.imagem_limpezaplus = DefineImagemLimpezaPlus(leito.cod_Plus);
                            objDashboardAtividadeAcomodacao.prioritarioAtividade = leito.prioritarioAtividade;
                            objDashboardAtividadeAcomodacao.cod_Isolamento = leito.cod_Isolamento;
                            objDashboardAtividadeAcomodacao.cod_Plus = leito.cod_Plus;

                            objDashboardAtividadeAcomodacao.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, item.id_TipoAtividadeAcomodacao);
                            objDashboardAtividadeAcomodacao.id_PerfilAdministrador = lPerfilTipoUsuarioPerfil;
                            objDashboardAtividadeAcomodacao.id_PerfilUsuario = lPerfilUsuario;

                            objDashboardAtividadeAcomodacao.pendenciaFinanceira = leito.pendenciaFinanceira;
                            objDashboardAtividadeAcomodacao.imagem_pendencia_financeira = DefineImagemPendenciaFinanceira(leito.pendenciaFinanceira);

                            objLstDashboardAtividadeAcomodacao.Add(objDashboardAtividadeAcomodacao);
                        }

                        objDashboardAtividadeSituacao.Acomodacoes = objLstDashboardAtividadeAcomodacao;

                        objLstDashboardAtividadeSituacao.Add(objDashboardAtividadeSituacao);

                    }


                    id_tipoAcaoAtual = item.id_TipoAcaoAcomodacao;
                }

                foreach (TipoAcaoAcomodacaoTO item in LstAcoes)
                {
                    if (objLstDashboardAtividadeSituacao.Where(m => m.id_TipoAcaoAcomodacao == item.id_TipoAcaoAcomodacao).ToList().Count == 0)
                    {
                        objDashboardAtividadeSituacao = new DashboardSituacaoAtividade_AcaoTO();
                        objDashboardAtividadeSituacao.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                        if (id_tipoatividadeacomodacao == "0")
                            objDashboardAtividadeSituacao.nome_TipoAtividadeAcomodacao = "Todas";
                        else
                            objDashboardAtividadeSituacao.nome_TipoAtividadeAcomodacao = LstAtividades.Where(m => m.id_TipoAtividadeAcomodacao == id_tipoatividadeacomodacao).FirstOrDefault().nome_TipoAtividadeAcomodacao;

                        objDashboardAtividadeSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                        objDashboardAtividadeSituacao.nome_TipoSituacaoAcomodacao = LstSituacoes.Where(m => m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).FirstOrDefault().nome_TipoSituacaoAcomodacao;
                        objDashboardAtividadeSituacao.id_TipoAcaoAcomodacao = item.id_TipoAcaoAcomodacao;
                        objDashboardAtividadeSituacao.nome_TipoAcaoAcomodacao = item.nome_Status_Label; //item.nome_TipoAcaoAcomodacao;
                        objDashboardAtividadeSituacao.imagem = item.imagem;
                        objDashboardAtividadeSituacao.cor = item.cor_TipoAcaoAcomodacao;
                        objDashboardAtividadeSituacao.ordemAcao = item.ordem;
                        objLstDashboardAtividadeAcomodacao = new List<DashboardSituacaoAtividade_AcomodacaoTO>();
                        objDashboardAtividadeSituacao.Acomodacoes = objLstDashboardAtividadeAcomodacao;
                        objLstDashboardAtividadeSituacao.Add(objDashboardAtividadeSituacao);
                    }

                }

                vwDashboardSituacaoAtividade.LstTipoAtividade = LstAtividades;
                vwDashboardSituacaoAtividade.LstSetor = LstSetores;
                vwDashboardSituacaoAtividade.id_Setor = pId_Setor;
                vwDashboardSituacaoAtividade.id_TipoAtividade = id_tipoatividadeacomodacao;
                vwDashboardSituacaoAtividade.id_TipoSituacao = id_tiposituacaoacomodacao;
                vwDashboardSituacaoAtividade.IdUsuarioLogado = Session["id_Usuario"].ToString();

                vwDashboardSituacaoAtividade.DashboardSituacaoAtividadeAcao = objLstDashboardAtividadeSituacao;

                //vwDashboardAtividade.token_modulo = accessToken.access_token;
                vwDashboardSituacaoAtividade.tkn_operacional = accessToken.access_token;

                /// TODAS DO  OPERACIONAL
                vwDashboardSituacaoAtividade.url_priorizaAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/prioridade/"); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacaoAtividade.url_limpezaPlusAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/plus/"); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacaoAtividade.url_getHistoricoAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/det/empresa/{0}/situacao/", Session["id_EmpresaSel"].ToString()); ///Complemento --- S?idAtividade=12222
                vwDashboardSituacaoAtividade.url_EncaminharAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items?idEmpresa={0}&idUsuario={1}&", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardSituacaoAtividade.url_CancelarAtividade = _urlBase + string.Format("api/AcaoAtividadeAcomodacao/items/cancelargenerico/empresa/{0}/usuario/{1}/atividade/", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwDashboardSituacaoAtividade.url_EncaminharSituacao = _urlBase + string.Format("api/Integracao/items/ajuste/empresa/{0}", Session["id_EmpresaSel"].ToString());
                vwDashboardSituacaoAtividade.url_getMensagens = _urlBase + "api/Mensagem/items/mensagem/idatividade/";
                vwDashboardSituacaoAtividade.url_sendMensagem = _urlBase + "api/Mensagem/items/mensagem";

                //1 / codexternoacomodacao / 0002 / situacaodestino / 3

                /// MUDA PARA ADMINISTRATIVO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                vwDashboardSituacaoAtividade.tkn_administrativo = accessToken.access_token;
                vwDashboardSituacaoAtividade.url_getAcomodacao = _urlBase + string.Format("api/Acomodacao/items/acomodacaodetalhe/");
                vwDashboardSituacaoAtividade.url_isolaAcomodacao = _urlBase + string.Format("api/Acomodacao/items/atividade/isolar/"); /// Complemento --- S?idAcomodacao=19

                /// MUDA PARA CONFIGURACAO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                vwDashboardSituacaoAtividade.tkn_configuracao = accessToken.access_token;
                vwDashboardSituacaoAtividade.url_listatipoatividade = _urlBase + string.Format("api/TipoAtividadeAcomodacao/items");
                vwDashboardSituacaoAtividade.url_listatiposituacao = _urlBase + string.Format("api/TipoSituacaoAcomodacao/items");
                ViewBag.percentualAtencaoSLA = ConfigurationManager.AppSettings["percentualAtencaoSLA"];
            }
            catch (Exception ex)
            {
                vwDashboardSituacaoAtividade.erro = "Erro ao carregar dashboard de atividades. Erro:" + ex.Message;
            }
            return View("DashboardSituacaoAtividade", vwDashboardSituacaoAtividade);
        }


        public ActionResult ListaDashboardPorSituacao(string id_tiposituacaoacomodacao)
        {
            ListaAcomodacaoPorSituacaoView vwListaAcomodacaoPorSituacao = new ListaAcomodacaoPorSituacaoView();
            string lPerfilUsuario = Session["id_PerfilSel"].ToString();
            string lPerfilTipoUsuarioPerfil = Session["cod_TipoUsuarioPerfilSel"].ToString();
            string lPerfilAdministrador = ConfigurationManager.AppSettings["CodPerfilAdministrador"];
            try
            {

                if (string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao))
                {
                    id_tiposituacaoacomodacao = "0"; // Vago
                }
                vwListaAcomodacaoPorSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Acomodacao/items/empresa/{0}/situacao/{1}", Session["id_EmpresaSel"].ToString(), id_tiposituacaoacomodacao);

                vwListaAcomodacaoPorSituacao.ListaAcomodacoesPorSituacao = base.Listar<AcomodacaoConsultaSituacaoTO>(accessToken, _urlBase, url);

                foreach (AcomodacaoConsultaSituacaoTO item in vwListaAcomodacaoPorSituacao.ListaAcomodacoesPorSituacao)
                {
                    item.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, "0");
                    item.id_PerfilAdministrador = lPerfilTipoUsuarioPerfil;
                    item.id_PerfilUsuario = lPerfilUsuario;
                }

                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                url = string.Format("api/TipoSituacaoAcomodacao/items");

                vwListaAcomodacaoPorSituacao.ListaTipoSituacaoAcomodacaoTO = base.Listar<TipoSituacaoAcomodacaoTO>(accessToken, _urlBase, url);


                url = string.Format("api/TipoSituacaoTipoAtividade/items/tiposituacao/{0}", id_tiposituacaoacomodacao);

                vwListaAcomodacaoPorSituacao.ListaTipoAtividadeAcomodacaoTO = base.Listar<TipoAtividadeAcomodacaoTO>(accessToken, _urlBase, url);

                /// MUDA PARA ADMINISTRATIVO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                vwListaAcomodacaoPorSituacao.tkn_administrativo = accessToken.access_token;
                vwListaAcomodacaoPorSituacao.url_getAcomodacao = _urlBase + string.Format("api/Acomodacao/items/acomodacaodetalhe/");
                vwListaAcomodacaoPorSituacao.url_isolaAcomodacao = _urlBase + string.Format("api/Acomodacao/items/atividade/isolar/"); /// Complemento --- S?idAcomodacao=19

                vwListaAcomodacaoPorSituacao.IdUsuarioLogado = Session["id_Usuario"].ToString();

                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];
                /// TODAS DO  OPERACIONAL
                vwListaAcomodacaoPorSituacao.tkn_operacional = accessToken.access_token;
                vwListaAcomodacaoPorSituacao.url_priorizaAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/prioridade/"); ///Complemento --- S?idAtividade=12222
                vwListaAcomodacaoPorSituacao.url_limpezaPlusAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade/plus/"); ///Complemento --- S?idAtividade=12222
                vwListaAcomodacaoPorSituacao.url_getHistoricoAcomodacao = _urlBase + string.Format("api/AtividadeAcomodacao/items/det/empresa/{0}/situacao/", Session["id_EmpresaSel"].ToString()); ///Complemento --- S?idAtividade=12222
                vwListaAcomodacaoPorSituacao.url_EncaminharAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items?idEmpresa={0}&idUsuario={1}&", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwListaAcomodacaoPorSituacao.url_GerarAtividade = _urlBase + string.Format("api/AtividadeAcomodacao/items/atividade?idEmpresa={0}", Session["id_EmpresaSel"].ToString());
                vwListaAcomodacaoPorSituacao.url_CancelarAtividade = _urlBase + string.Format("api/AcaoAtividadeAcomodacao/items/cancelargenerico/empresa/{0}/usuario/{1}/atividade/", Session["id_EmpresaSel"].ToString(), Session["id_Usuario"].ToString());
                vwListaAcomodacaoPorSituacao.url_EncaminharSituacao = _urlBase + string.Format("api/Integracao/items/ajuste/empresa/{0}", Session["id_EmpresaSel"].ToString());
                vwListaAcomodacaoPorSituacao.url_getMensagens = _urlBase + "api/Mensagem/items/mensagem/idatividade/";
                vwListaAcomodacaoPorSituacao.url_sendMensagem = _urlBase + "api/Mensagem/items/mensagem";


                /// MUDA PARA CONFIGURACAO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                vwListaAcomodacaoPorSituacao.tkn_configuracao = accessToken.access_token;
                vwListaAcomodacaoPorSituacao.url_listatipoatividade = _urlBase + string.Format("api/TipoAtividadeAcomodacao/items");
                vwListaAcomodacaoPorSituacao.url_listatiposituacao = _urlBase + string.Format("api/TipoSituacaoAcomodacao/items");
                ViewBag.percentualAtencaoSLA = ConfigurationManager.AppSettings["percentualAtencaoSLA"];

            }
            catch (Exception ex)
            {
                vwListaAcomodacaoPorSituacao.erro = "Ocorreu um erro ao listar acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("ListaDashboardPorSituacao", vwListaAcomodacaoPorSituacao);
        }

        #endregion

    }
}
