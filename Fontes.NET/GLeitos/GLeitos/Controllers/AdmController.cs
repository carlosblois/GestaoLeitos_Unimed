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
using ExpoFramework.Framework.Utils;
using GLeitos.GLeitosTO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace GLeitos.Controllers
{
    public class AdmController : BaseController
    {
        #region "Home"
            public ActionResult Index()
            {
                return View();
            }
        #endregion

        #region "Empresa"

        public ActionResult ListaEmpresa()
        {
            ListaEmpresaView vwListaEmpresas = new ListaEmpresaView();
            try
            {
                
                //List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = "api/Empresa/items";

                ViewBag.MenuEmpresa = "menu-ativo";

                vwListaEmpresas.ListaEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaEmpresas.erro = "Ocorreu um erro ao listar empresas. Detalhe: " + ex.Message;
                // throw ex;
            }

            return View("ListaEmpresa", vwListaEmpresas);
        }

        public ActionResult Empresa(decimal? id_empresa)
        {
            EmpresaView vwEmpresa = new EmpresaView();
            EmpresaTO objEmpresa = new EmpresaTO();
            List<EmpresaTO> objLstEmpresa = new List<EmpresaTO>();
            try
            {
                vwEmpresa.ListaUfs = RetornaUfs();

                if (id_empresa == null)
                {
                    vwEmpresa.Empresa = new EmpresaTO();
                }
                else 
                {

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/Empresa/items/{0}", id_empresa);

                    //base.Consultar<EmpresaTO>(accessToken, _urlBase, url, ref objEmpresa);
                    objLstEmpresa = Listar<EmpresaTO>(accessToken, _urlBase, url);

                    vwEmpresa.Empresa = objLstEmpresa[0];
                }

            }
            catch (Exception ex)
            {
                vwEmpresa.erro = "Ocorreu um erro ao consultar a empresa. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Empresa", vwEmpresa);
        }

        public ActionResult ExcluirEmpresa(decimal? id_empresa)
        {
            ListaEmpresaView vwListaEmpresa = new ListaEmpresaView();
            List<EmpresaTO> objLstEmpresa = new List<EmpresaTO>();
            try
            {

                if (id_empresa == null)
                {
                    throw new Exception("Identificador da empresa é inválido.");
                }
                else
                {

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/Empresa/items?id_Empresa={0}", id_empresa);

                    //base.Consultar<EmpresaTO>(accessToken, _urlBase, url, ref objEmpresa);
                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir empresa."); 
                    }


                    url = string.Format("api/Empresa/items");

                    objLstEmpresa = Listar<EmpresaTO>(accessToken, _urlBase, url);

                    vwListaEmpresa.ListaEmpresas = objLstEmpresa;

                    vwListaEmpresa.mensagem = "Empresa excluída com sucesso.";

                }

            }
            catch (Exception ex)
            {
                vwListaEmpresa.erro = "Erro ao tentar excluir empresa. Erro:" + ex.Message;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("ListaEmpresa", vwListaEmpresa);
        }

        public ActionResult SalvarEmpresa(EmpresaView model)
        {
            EmpresaView vwEmpresa = new EmpresaView();
            EmpresaTO objEmpresa = new EmpresaTO();
            List<EmpresaTO> objLstEmpresa = new List<EmpresaTO>();
            string id_Empresa = "";
            try
            {
                vwEmpresa.ListaUfs = RetornaUfs();

                if (!ModelState.IsValid)
                {
                    return View("Empresa", model);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/Empresa/items");

                    if (string.IsNullOrWhiteSpace(model.Empresa.id_Empresa))
                    {
                        model.Empresa.id_Empresa = "0";
                    }

                    base.Salvar<EmpresaTO>(accessToken, _urlBase, url, model.Empresa, ref id_Empresa);

                    url = string.Format("api/Empresa/items/{0}", id_Empresa);

                    objLstEmpresa = base.Listar<EmpresaTO>(accessToken, _urlBase, url);

                    vwEmpresa.Empresa = objLstEmpresa[0];

                    vwEmpresa.mensagem = "Empresa salva com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwEmpresa.erro = "Erro ao salvar empresa. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Empresa", vwEmpresa);
        }

        #endregion
        
        #region "Setores"

        public ActionResult ListaSetor()
        {
            ListaSetorView vwListaSetors = new ListaSetorView();
            try
            {

                List<SetorTO> lstRetorno = new List<SetorTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                vwListaSetors.ListaSetores = base.Listar<SetorTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaSetors.erro = "Ocorreu um erro ao listar setores. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSetor = "menu-ativo";
            return View("ListaSetor", vwListaSetors);
        }

        public ActionResult Setor(decimal? id_setor)
        {
            SetorView vwSetor = new SetorView();
            SetorTO objSetor = new SetorTO();
            List<SetorTO> objLstSetor = new List<SetorTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";

                vwSetor.ListaEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);


                if (id_setor == null)
                {
                    vwSetor.Setor = new SetorTO();
                }
                else
                {

                    List<SetorTO> lstRetorno = new List<SetorTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    //base.Consultar<SetorTO>(accessToken, _urlBase, url, ref objSetor);
                    objLstSetor = Listar<SetorTO>(accessToken, _urlBase, url);

                    vwSetor.Setor = objLstSetor.Where(m => m.id_Setor==id_setor.ToString()).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                vwSetor.erro = "Ocorreu um erro ao consultar o setor. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSetor = "menu-ativo";
            return View("Setor", vwSetor);
        }

        public ActionResult ExcluirSetor(string id_empresa, string id_setor)
        {
            ListaSetorView vwListaSetor = new ListaSetorView();
            List<SetorTO> objLstSetor = new List<SetorTO>();
            try
            {

                if (id_setor == null)
                {
                    throw new Exception("Identificador do setor é inválido.");
                }
                else
                {

                    List<SetorTO> lstRetorno = new List<SetorTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/Setor/items?id_Empresa={0}&id_Setor={1}", id_empresa, id_setor);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir setor.");
                    }


                    url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstSetor = Listar<SetorTO>(accessToken, _urlBase, url);

                    vwListaSetor.ListaSetores = objLstSetor;

                    vwListaSetor.mensagem = "Setor excluída com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwListaSetor.erro = "Erro ao tentar excluir setor. Erro:" + ex.Message;
            }
            ViewBag.MenuSetor = "menu-ativo";
            return View("ListaSetor", vwListaSetor);
        }

        public ActionResult SalvarSetor(SetorView model)
        {
            SetorView vwSetor = new SetorView();
            SetorTO objSetor = new SetorTO();
            List<SetorTO> objLstSetor = new List<SetorTO>();
            string id_Setor = "";
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";

                vwSetor.ListaEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);
                
                if (!ModelState.IsValid)
                {
                    return View("Setor", model);
                }
                else
                {
                    ModelState.Clear();

                    List<SetorTO> lstRetorno = new List<SetorTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/Setor/items");

                    if (string.IsNullOrWhiteSpace(model.Setor.id_Setor))
                    {
                        model.Setor.id_Setor = "0";
                    }

                    base.Salvar<SetorTO>(accessToken, _urlBase, url, model.Setor, ref id_Setor);

                    url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstSetor = Listar<SetorTO>(accessToken, _urlBase, url);

                    vwSetor.Setor = objLstSetor.Where(m => m.id_Setor == id_Setor.ToString()).FirstOrDefault();

                    vwSetor.mensagem = "Setor salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwSetor.erro = "Erro ao salvar setor. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSetor = "menu-ativo";
            return View("Setor", vwSetor);
        }

        #endregion

        #region "Característica de acomodações"

        public ActionResult CaracteristicaAcomodacao(decimal? id_caracteristica_acomodacao)
        {
            CaracteristicaAcomodacaoView vwCaracteristicaAcomodacao = new CaracteristicaAcomodacaoView();
            CaracteristicaAcomodacaoTO objCaracteristicaAcomodacao = new CaracteristicaAcomodacaoTO();
            List<CaracteristicaAcomodacaoTO> objLstCaracteristicaAcomodacao = new List<CaracteristicaAcomodacaoTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/CaracteristicaAcomodacao/items";

                vwCaracteristicaAcomodacao.ListaCaracteristicaAcomodacao = base.Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);


                if (id_caracteristica_acomodacao == null)
                {
                    vwCaracteristicaAcomodacao.CaracteristicaAcomodacao = new CaracteristicaAcomodacaoTO();
                }
                else
                {

                    List<CaracteristicaAcomodacaoTO> lstRetorno = new List<CaracteristicaAcomodacaoTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/CaracteristicaAcomodacao/items");

                    objLstCaracteristicaAcomodacao = Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                    vwCaracteristicaAcomodacao.CaracteristicaAcomodacao = objLstCaracteristicaAcomodacao.Where(m => m.id_CaracteristicaAcomodacao == id_caracteristica_acomodacao.ToString()).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                vwCaracteristicaAcomodacao.erro = "Ocorreu um erro ao listar características de acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCaracteristicaAcomodacao = "menu-ativo";
            return View("CaracteristicaAcomodacao", vwCaracteristicaAcomodacao);
        }

        public ActionResult ExcluirCaracteristicaAcomodacao(string id_caracteristica_acomodacao)
        {
            CaracteristicaAcomodacaoView vwCaracteristicaAcomodacao = new CaracteristicaAcomodacaoView();
            List<CaracteristicaAcomodacaoTO> objLstCaracteristicaAcomodacao = new List<CaracteristicaAcomodacaoTO>();
            try
            {

                if (id_caracteristica_acomodacao == null)
                {
                    throw new Exception("Identificador da característica da acomodação é inválido.");
                }
                else
                {

                    List<CaracteristicaAcomodacaoTO> lstRetorno = new List<CaracteristicaAcomodacaoTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/CaracteristicaAcomodacao/items?id_caracteristicaAcomodacao={0}", id_caracteristica_acomodacao);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir característica da acomodação.");
                    }


                    url = string.Format("api/CaracteristicaAcomodacao/items");

                    objLstCaracteristicaAcomodacao = Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                    vwCaracteristicaAcomodacao.ListaCaracteristicaAcomodacao = objLstCaracteristicaAcomodacao;

                    vwCaracteristicaAcomodacao.mensagem = "CaracteristicaAcomodacao excluída com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwCaracteristicaAcomodacao.erro = "Erro ao tentar excluir a característica da acamodação. Erro:" + ex.Message;
            }
            ViewBag.MenuCaracteristicaAcomodacao = "menu-ativo";

            return View("CaracteristicaAcomodacao", vwCaracteristicaAcomodacao);
        }

        public ActionResult SalvarCaracteristicaAcomodacao(CaracteristicaAcomodacaoView model)
        {
            CaracteristicaAcomodacaoView vwCaracteristicaAcomodacao = new CaracteristicaAcomodacaoView();
            CaracteristicaAcomodacaoTO objCaracteristicaAcomodacao = new CaracteristicaAcomodacaoTO();
            List<CaracteristicaAcomodacaoTO> objLstCaracteristicaAcomodacao = new List<CaracteristicaAcomodacaoTO>();
            string id_CaracteristicaAcomodacao = "";
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/CaracteristicaAcomodacao/items";

                vwCaracteristicaAcomodacao.ListaCaracteristicaAcomodacao = base.Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                if (!ModelState.IsValid)
                {
                    return View("CaracteristicaAcomodacao", model);
                }
                else
                {
                    ModelState.Clear();

                    List<CaracteristicaAcomodacaoTO> lstRetorno = new List<CaracteristicaAcomodacaoTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/CaracteristicaAcomodacao/items");

                    if (string.IsNullOrWhiteSpace(model.CaracteristicaAcomodacao.id_CaracteristicaAcomodacao))
                    {
                        model.CaracteristicaAcomodacao.id_CaracteristicaAcomodacao = "0";
                    }

                    base.Salvar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url, model.CaracteristicaAcomodacao, ref id_CaracteristicaAcomodacao);

                    url = string.Format("api/CaracteristicaAcomodacao/items");

                    objLstCaracteristicaAcomodacao = Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                    vwCaracteristicaAcomodacao.CaracteristicaAcomodacao = new CaracteristicaAcomodacaoTO();

                    vwCaracteristicaAcomodacao.ListaCaracteristicaAcomodacao = objLstCaracteristicaAcomodacao;

                    vwCaracteristicaAcomodacao.mensagem = "Característica da acomodação salva com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwCaracteristicaAcomodacao.erro = "Erro ao salvar a característica de acomodação. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCaracteristicaAcomodacao = "menu-ativo";
            return View("CaracteristicaAcomodacao", vwCaracteristicaAcomodacao);
        }

        #endregion
        
        #region "Tipo de Acomodação"

        public ActionResult ListaTipoAcomodacao()
        {
            ListaTipoAcomodacaoView vwListaTipoAcomodacaos = new ListaTipoAcomodacaoView();
            try
            {

                List<TipoAcomodacaoTO> lstRetorno = new List<TipoAcomodacaoTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                vwListaTipoAcomodacaos.ListaTipoAcomodacoes = base.Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaTipoAcomodacaos.erro = "Ocorreu um erro ao listar tipos de acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuTipoAcomodacao = "menu-ativo";
            return View("ListaTipoAcomodacao", vwListaTipoAcomodacaos);
        }

        public ActionResult TipoAcomodacao(decimal? id_tipoacomodacao)
        {
            TipoAcomodacaoView vwTipoAcomodacao = new TipoAcomodacaoView();
            TipoAcomodacaoTO objTipoAcomodacao = new TipoAcomodacaoTO();
            List<TipoAcomodacaoTO> objLstTipoAcomodacao = new List<TipoAcomodacaoTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";

                vwTipoAcomodacao.ListaEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);

                url = "api/CaracteristicaAcomodacao/items";

                vwTipoAcomodacao.ListaCaracteristicas = base.Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                if (id_tipoacomodacao == null)
                {
                    vwTipoAcomodacao.TipoAcomodacao = new TipoAcomodacaoTO();
                }
                else
                {

                    List<TipoAcomodacaoTO> lstRetorno = new List<TipoAcomodacaoTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    //base.Consultar<TipoAcomodacaoTO>(accessToken, _urlBase, url, ref objTipoAcomodacao);
                    objLstTipoAcomodacao = Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);

                    vwTipoAcomodacao.TipoAcomodacao = objLstTipoAcomodacao.Where(m => m.id_TipoAcomodacao == id_tipoacomodacao.ToString()).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                vwTipoAcomodacao.erro = "Ocorreu um erro ao consultar o tipo de acomodação. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuTipoAcomodacao = "menu-ativo";
            return View("TipoAcomodacao", vwTipoAcomodacao);
        }

        public ActionResult ExcluirTipoAcomodacao(string id_empresa, string id_tipoacomodacao)
        {
            ListaTipoAcomodacaoView vwListaTipoAcomodacao = new ListaTipoAcomodacaoView();
            List<TipoAcomodacaoTO> objLstTipoAcomodacao = new List<TipoAcomodacaoTO>();
            try
            {

                if (id_tipoacomodacao == null)
                {
                    throw new Exception("Identificador do tipo de acomodação é inválido.");
                }
                else
                {

                    List<TipoAcomodacaoTO> lstRetorno = new List<TipoAcomodacaoTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/TipoAcomodacao/items?id_Empresa={0}&id_TipoAcomodacao={1}", id_empresa, id_tipoacomodacao);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir o tipo de acomodação.");
                    }
                    
                    url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstTipoAcomodacao = Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);

                    vwListaTipoAcomodacao.ListaTipoAcomodacoes = objLstTipoAcomodacao;

                    vwListaTipoAcomodacao.mensagem = "O tipo de acomodação excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwListaTipoAcomodacao.erro = "Erro ao tentar excluir o tipo de acomodação. Erro:" + ex.Message;
            }
            ViewBag.MenuTipoAcomodacao = "menu-ativo";
            return View("ListaTipoAcomodacao", vwListaTipoAcomodacao);
        }

        public ActionResult SalvarTipoAcomodacao(TipoAcomodacaoView model)
        {
            TipoAcomodacaoView vwTipoAcomodacao = new TipoAcomodacaoView();
            TipoAcomodacaoTO objTipoAcomodacao = new TipoAcomodacaoTO();
            List<TipoAcomodacaoTO> objLstTipoAcomodacao = new List<TipoAcomodacaoTO>();
            List<EmpresaTO> objLstEmpresas = new List<EmpresaTO>();
            List<CaracteristicaAcomodacaoTO> objLstCaracteristicas = new List<CaracteristicaAcomodacaoTO>();
            string id_TipoAcomodacao = "";
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";

                objLstEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);


                url = "api/CaracteristicaAcomodacao/items";
                objLstCaracteristicas = base.Listar<CaracteristicaAcomodacaoTO>(accessToken, _urlBase, url);

                if (!ModelState.IsValid)
                {
                    model.ListaCaracteristicas = objLstCaracteristicas;
                    model.ListaEmpresas = objLstEmpresas;
                    return View("TipoAcomodacao", model);
                }
                else
                {
                    vwTipoAcomodacao.ListaCaracteristicas = objLstCaracteristicas;
                    vwTipoAcomodacao.ListaEmpresas = objLstEmpresas;

                    List<TipoAcomodacaoTO> lstRetorno = new List<TipoAcomodacaoTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/TipoAcomodacao/items");

                    if (string.IsNullOrWhiteSpace(model.TipoAcomodacao.id_TipoAcomodacao))
                    {
                        model.TipoAcomodacao.id_TipoAcomodacao = "0";
                    }
                    else
                    {
                        id_TipoAcomodacao = model.TipoAcomodacao.id_TipoAcomodacao;
                    }

                    base.Salvar<TipoAcomodacaoTO>(accessToken, _urlBase, url, model.TipoAcomodacao, ref id_TipoAcomodacao);

                    ModelState.Clear();

                    url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstTipoAcomodacao = Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);

                    vwTipoAcomodacao.TipoAcomodacao = objLstTipoAcomodacao.Where(m => m.id_TipoAcomodacao == id_TipoAcomodacao.ToString()).FirstOrDefault();

                    vwTipoAcomodacao.mensagem = "O tipo de acomodação foi salva com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwTipoAcomodacao.erro = "Erro ao salvar tipo de acomodação. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuTipoAcomodacao = "menu-ativo";

            return View("TipoAcomodacao", vwTipoAcomodacao);
        }

        #endregion
        
        #region "Acomodação"

        public ActionResult ListaAcomodacao()
        {
            ListaAcomodacaoView vwListaAcomodacaos = new ListaAcomodacaoView();
            try
            {

                List<AcomodacaoTO> lstRetorno = new List<AcomodacaoTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Acomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                vwListaAcomodacaos.ListaAcomodacoes = base.Listar<AcomodacaoTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaAcomodacaos.erro = "Ocorreu um erro ao listar acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("ListaAcomodacao", vwListaAcomodacaos);
        }

        public ActionResult Acomodacao(decimal? id_acomodacao)
        {
            AcomodacaoView vwAcomodacao = new AcomodacaoView();
            TipoAcomodacaoTO objTipoAcomodacao = new TipoAcomodacaoTO();
            List<AcomodacaoTO> objLstAcomodacao = new List<AcomodacaoTO>();
            List<TipoAcomodacaoTO> objLstTipoAcomodacao = new List<TipoAcomodacaoTO>();
            List<EmpresaTO> objLstEmpresas = new List<EmpresaTO>();
            List<SetorTO> objLstSetor = new List<SetorTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";

                
                objLstEmpresas = base.ListarEmpresas();

                //url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                objLstTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                //url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                objLstSetor = base.ListarSetores(Session["id_EmpresaSel"].ToString());//

                vwAcomodacao.ListaEmpresas = objLstEmpresas;
                vwAcomodacao.ListaTiposAcomodacoes = objLstTipoAcomodacao;
                vwAcomodacao.ListaSetores = objLstSetor;


                if (id_acomodacao == null)
                {
                    vwAcomodacao.Acomodacao = new AcomodacaoTO();
                }
                else
                {

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/Acomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstAcomodacao = Listar<AcomodacaoTO>(accessToken, _urlBase, url);

                    vwAcomodacao.Acomodacao = objLstAcomodacao.Where(m => m.id_Acomodacao == id_acomodacao.ToString()).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                vwAcomodacao.erro = "Ocorreu um erro ao consultar a acomodação. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("Acomodacao", vwAcomodacao);
        }

        public ActionResult ExcluirAcomodacao(string id_empresa, string id_acomodacao)
        {
            ListaAcomodacaoView vwListaAcomodacao = new ListaAcomodacaoView();
            List<AcomodacaoTO> objLstAcomodacao = new List<AcomodacaoTO>();
            try
            {

                if (id_acomodacao == null)
                {
                    throw new Exception("Identificador da acomodação é inválido.");
                }
                else
                {

                    List<TipoAcomodacaoTO> lstRetorno = new List<TipoAcomodacaoTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                    
                    string url = string.Format("api/Acomodacao/items?id_Acomodacao={0}",id_acomodacao);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir a acomodação.");
                    }
                    else
                    {
                        url = string.Format("api/Acomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());
                        objLstAcomodacao = Listar<AcomodacaoTO>(accessToken, _urlBase, url);
                        vwListaAcomodacao.ListaAcomodacoes = objLstAcomodacao;

                        vwListaAcomodacao.mensagem = "Acomodação excluída com sucesso.";
                    }

                }

            }
            catch (Exception ex)
            {
                vwListaAcomodacao.erro = "Erro ao tentar excluir a acomodação. Erro:" + ex.Message;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("ListaAcomodacao", vwListaAcomodacao);
        }

        public ActionResult SalvarAcomodacao(AcomodacaoView model)
        {
            AcomodacaoView vwAcomodacao = new AcomodacaoView();
            TipoAcomodacaoTO objTipoAcomodacao = new TipoAcomodacaoTO();
            List<AcomodacaoTO> objLstAcomodacao = new List<AcomodacaoTO>();
            List<TipoAcomodacaoTO> objLstTipoAcomodacao = new List<TipoAcomodacaoTO>();
            List<EmpresaTO> objLstEmpresas = new List<EmpresaTO>();
            List<SetorTO> objLstSetor = new List<SetorTO>();
            string id_Acomodacao = "";
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                string url = "api/Empresa/items";


                objLstEmpresas = base.Listar<EmpresaTO>(accessToken, _urlBase, url);

                url = string.Format("api/TipoAcomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                objLstTipoAcomodacao = base.Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);

                url = string.Format("api/Setor/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                objLstSetor = base.Listar<SetorTO>(accessToken, _urlBase, url);

               

                if (!ModelState.IsValid)
                {
                    model.ListaTiposAcomodacoes = objLstTipoAcomodacao;
                    model.ListaEmpresas = objLstEmpresas;
                    model.ListaSetores = objLstSetor;

                    return View("Acomodacao", model);
                }
                else
                {
                    vwAcomodacao.ListaEmpresas = objLstEmpresas;
                    vwAcomodacao.ListaTiposAcomodacoes = objLstTipoAcomodacao;
                    vwAcomodacao.ListaSetores = objLstSetor;

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/Acomodacao/items");

                    if (string.IsNullOrWhiteSpace(model.Acomodacao.id_Acomodacao))
                    {
                        model.Acomodacao.id_Acomodacao = "0";
                        model.Acomodacao.cod_Isolamento = "N";
                    }
                    else
                    {
                        id_Acomodacao = model.Acomodacao.id_Acomodacao;
                    }

                    base.Salvar<AcomodacaoTO>(accessToken, _urlBase, url, model.Acomodacao, ref id_Acomodacao);

                    ModelState.Clear();

                    url = string.Format("api/Acomodacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstAcomodacao = Listar<AcomodacaoTO>(accessToken, _urlBase, url);

                    vwAcomodacao.Acomodacao = objLstAcomodacao.Where(m => m.id_Acomodacao == id_Acomodacao.ToString()).FirstOrDefault();

                    vwAcomodacao.mensagem = "Acomodação salva com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwAcomodacao.erro = "Erro ao salvar a acomodação. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";

            return View("Acomodacao", vwAcomodacao);
        }
        
        public ActionResult ListaAcomodacaoPorSituacao(string id_tiposituacaoacomodacao)
        {
            ListaAcomodacaoPorSituacaoView vwListaAcomodacaoPorSituacao = new ListaAcomodacaoPorSituacaoView();
            string lPerfilUsuario = Session["id_PerfilSel"].ToString();
            string lPerfilAdministrador = ConfigurationManager.AppSettings["CodPerfilAdministrador"];
            try
            {

                if (string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao))
                {
                    id_tiposituacaoacomodacao = "6"; // Vago
                }
                vwListaAcomodacaoPorSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Acomodacao/items/empresa/{0}/situacao/{1}", Session["id_EmpresaSel"].ToString(), id_tiposituacaoacomodacao);

                vwListaAcomodacaoPorSituacao.ListaAcomodacoesPorSituacao = base.Listar<AcomodacaoConsultaSituacaoTO>(accessToken, _urlBase, url);

                foreach(AcomodacaoConsultaSituacaoTO item in vwListaAcomodacaoPorSituacao.ListaAcomodacoesPorSituacao)
                {
                    item.cod_Acesso = DefinirTipoAcessoPorSituacaoAtividade(lPerfilUsuario, item.id_TipoSituacaoAcomodacao, "0");
                    item.id_PerfilAdministrador = lPerfilAdministrador;
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

                /// MUDA PARA CONFIGURACAO
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                vwListaAcomodacaoPorSituacao.tkn_configuracao = accessToken.access_token;
                vwListaAcomodacaoPorSituacao.url_listatipoatividade = _urlBase + string.Format("api/TipoAtividadeAcomodacao/items");
                ViewBag.percentualAtencaoSLA = ConfigurationManager.AppSettings["percentualAtencaoSLA"];

            }
            catch (Exception ex)
            {
                vwListaAcomodacaoPorSituacao.erro = "Ocorreu um erro ao listar acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("ListaAcomodacaoPorSituacao", vwListaAcomodacaoPorSituacao);
        }

        public ActionResult GerarAtividadeAcomodacao(string id_acomodacao, string id_SituacaoAcomodacao, string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao)
        {
            ListaAcomodacaoPorSituacaoView vwListaAcomodacaoPorSituacao = new ListaAcomodacaoPorSituacaoView();

            SituacaoAcomodacaoTO situacaoAcomodacaoTO = new SituacaoAcomodacaoTO();
            AtividadeAcomodacaoTO atividadeAcomodacaoTO = new AtividadeAcomodacaoTO();
            AcaoAtividadeAcomodacaoTO acaoAtividadeAcomodacaoTO = new AcaoAtividadeAcomodacaoTO();
            string dataInicio = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            string id_SLASel = "0";
            List<SLASituacaoConsultaTO> ListaSLASituacao = new List<SLASituacaoConsultaTO>();
            ///string id_SLASituacaoSel = "0";
            string id_atividadeacomodacao = "0";
            string id_acaoatividadeacomodacao = "0";
            try
            {


                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                ListaSLAEmpresa = base.Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                id_SLASel = ListaSLAEmpresa.Where(m => m.id_TipoAcaoAcomodacao == "5" && m.id_TipoAtividadeAcomodacao == id_tipoatividadeacomodacao && m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).FirstOrDefault().id_SLA;


                //************* carregando a lista de acoes de uma atividade *******************/
                atividadeAcomodacaoTO.id_AtividadeAcomodacao = "0";
                atividadeAcomodacaoTO.id_SituacaoAcomodacao = id_SituacaoAcomodacao;
                atividadeAcomodacaoTO.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                atividadeAcomodacaoTO.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                atividadeAcomodacaoTO.dt_InicioAtividadeAcomodacao = dataInicio;
                atividadeAcomodacaoTO.dt_FimAtividadeAcomodacao = "";
                atividadeAcomodacaoTO.id_UsuarioSolicitante = Session["id_Usuario"].ToString();
                atividadeAcomodacaoTO.Cod_Prioritario = "N";
                atividadeAcomodacaoTO.Cod_Plus = "N";
                
                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                url = string.Format("api/AtividadeAcomodacao/items/atividade");

                id_atividadeacomodacao = "";

                base.Salvar<AtividadeAcomodacaoTO>(accessToken, _urlBase, url, atividadeAcomodacaoTO, ref id_atividadeacomodacao);


                //******************************************************************************/

                //************* carregando a lista de acoes de uma atividade *******************/
                acaoAtividadeAcomodacaoTO.id_AcaoAtividadeAcomodacao = "0";
                acaoAtividadeAcomodacaoTO.id_AtividadeAcomodacao = id_atividadeacomodacao;
                acaoAtividadeAcomodacaoTO.id_TipoAcaoAcomodacao = "5"; // Solicitar
                acaoAtividadeAcomodacaoTO.dt_InicioAcaoAtividade = dataInicio;
                acaoAtividadeAcomodacaoTO.dt_FimAcaoAtividade = "";
                acaoAtividadeAcomodacaoTO.id_SLA = id_SLASel;
                acaoAtividadeAcomodacaoTO.id_UsuarioExecutor = Session["id_Usuario"].ToString();

                url = string.Format("api/AcaoAtividadeAcomodacao/items/acao");

                id_acaoatividadeacomodacao = "";

                base.Salvar<AcaoAtividadeAcomodacaoTO>(accessToken, _urlBase, url, acaoAtividadeAcomodacaoTO, ref id_acaoatividadeacomodacao);

                //******************************************************************************/



                ///accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                //_urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                //url = string.Format("api/SLASituacao/items/empresa/{0}/tiposituacao/{1}", Session["id_EmpresaSel"].ToString(), id_tiposituacaoacomodacao);

                //ListaSLASituacao = base.Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url);

                //id_SLASituacaoSel = ListaSLASituacao.Where(m => m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).FirstOrDefault().id_SLA;

                //************* carregando a lista de acoes de uma atividade *******************/
                //situacaoAcomodacaoTO.id_SituacaoAcomodacao = id_SituacaoAcomodacao;
                //situacaoAcomodacaoTO.id_Acomodacao = id_acomodacao;
                //situacaoAcomodacaoTO.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                //situacaoAcomodacaoTO.dt_InicioSituacaoAcomodacao = dataInicio;
                //situacaoAcomodacaoTO.dt_FimSituacaoAcomodacao = "";
                //situacaoAcomodacaoTO.cod_NumAtendimento = "0";
                //situacaoAcomodacaoTO.id_SLA = id_SLASituacaoSel;
                //situacaoAcomodacaoTO.cod_Prioritario = "N";
                // situacaoAcomodacaoTO.atividadeItems = ListaAtividadeAcomodacaoTO;
                //*****************************************************************************/

                //accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                //_urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                //url = string.Format("api/SituacaoAcomodacao/items");

                //string id_situacaoacomodacao = "";

                //base.Salvar<SituacaoAcomodacaoTO>(accessToken, _urlBase, url, situacaoAcomodacaoTO, ref id_situacaoacomodacao);

                vwListaAcomodacaoPorSituacao.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;

                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                url = string.Format("api/Acomodacao/items/empresa/{0}/situacao/{1}", Session["id_EmpresaSel"].ToString(), id_tiposituacaoacomodacao);

                vwListaAcomodacaoPorSituacao.ListaAcomodacoesPorSituacao = base.Listar<AcomodacaoConsultaSituacaoTO>(accessToken, _urlBase, url);

                accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                url = string.Format("api/TipoSituacaoAcomodacao/items");

                vwListaAcomodacaoPorSituacao.ListaTipoSituacaoAcomodacaoTO = base.Listar<TipoSituacaoAcomodacaoTO>(accessToken, _urlBase, url);


                url = string.Format("api/TipoSituacaoTipoAtividade/items/tiposituacao/{0}", id_tiposituacaoacomodacao);

                vwListaAcomodacaoPorSituacao.ListaTipoAtividadeAcomodacaoTO = base.Listar<TipoAtividadeAcomodacaoTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaAcomodacaoPorSituacao.erro = "Ocorreu um erro ao listar acomodações. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuAcomodacao = "menu-ativo";
            return View("ListaAcomodacaoPorSituacao", vwListaAcomodacaoPorSituacao);
        }

        #endregion

    }
}
