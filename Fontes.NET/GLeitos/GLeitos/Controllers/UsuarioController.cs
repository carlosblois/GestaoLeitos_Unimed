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
    public class UsuarioController : BaseController
    {
        #region "Perfis"

        private List<PerfilTO> ListaPerfis()
        {
            List<PerfilTO> ListaPerfils = new List<PerfilTO>();

            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                string url = "api/Perfil/items";

                ListaPerfils = base.Listar<PerfilTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaPerfils;
        }

        public ActionResult ListaPerfil()
        {
            ListaPerfilView vwListaPerfils = new ListaPerfilView();
            try
            {

                //List<PerfilTO> lstRetorno = new List<PerfilTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                string url = string.Format("api/Perfil/items");

                ViewBag.MenuPerfil = "menu-ativo";

                vwListaPerfils.ListaPerfis = base.Listar<PerfilTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaPerfils.erro = "Ocorreu um erro ao listar perfis. Detalhe: " + ex.Message;
                // throw ex;
            }

            return View("ListaPerfil", vwListaPerfils);
        }

        public ActionResult Perfil(decimal? id_perfil)
        {
            PerfilView vwPerfil = new PerfilView();
            PerfilTO objPerfil = new PerfilTO();
            List<PerfilTO> objLstPerfil = new List<PerfilTO>();
            List<AcessoEmpresaPerfilTsTaConsultaTO> objLstAcessoEmpresaPerfilTsTa = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            List<TipoAtividadeAcomodacaoTO> objLstTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoSituacaoAcomodacaoTO> objLstTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAcessoTO> objLstTipoAcesso = new List<TipoAcessoTO>();
            List<TipoPerfilTO> objLstTipoPerfil = new List<TipoPerfilTO>();
            List<EmpresaPerfilConsultaTO> objLstEmpresaPerfil = new List<EmpresaPerfilConsultaTO>();
            EmpresaPerfilConsultaTO objEmpresaPerfilConsulta = new EmpresaPerfilConsultaTO();
            try
            {
                objLstTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();

                objLstTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();

                objLstTipoAcesso = base.RetornaTipoAcesso();

                objLstTipoPerfil = base.RetornaTipoPerfil();

                if (!ModelState.IsValid)
                {
                    return View(vwPerfil);
                }

                //Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];
                //string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];
                //string url = "api/Perfil/items";

                //vwPerfil.ListaPerfil = base.Listar<PerfilTO>(accessToken, _urlBase, url);

                vwPerfil.ListaTipoAtividadeAcomodacao = objLstTipoAtividadeAcomodacao;
                vwPerfil.ListaTipoSituacaoAcomodacao = objLstTipoSituacaoAcomodacao;
                vwPerfil.ListaTipoAcesso = objLstTipoAcesso;
                vwPerfil.ListaTipoPerfil = objLstTipoPerfil;

                if (id_perfil == null)
                {
                    vwPerfil.Perfil = new PerfilTO();
                }
                else
                {

                    List<PerfilTO> lstRetorno = new List<PerfilTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    string url = string.Format("api/EmpresaPerfil/items/{0}", Session["id_EmpresaSel"].ToString());

                    objLstEmpresaPerfil = base.Listar<EmpresaPerfilConsultaTO>(accessToken, _urlBase, url);

                    objEmpresaPerfilConsulta = objLstEmpresaPerfil.Where(m => m.id_Perfil == id_perfil.ToString()).FirstOrDefault();

                    if (objEmpresaPerfilConsulta != null)
                    {
                        vwPerfil.codTipoPerfil = objEmpresaPerfilConsulta.cod_Tipo_Perfil;
                    }

                    url = string.Format("api/Perfil/items");

                    objLstPerfil = Listar<PerfilTO>(accessToken, _urlBase, url);

                    vwPerfil.Perfil = objLstPerfil.Where(m => m.id_Perfil == id_perfil.ToString()).FirstOrDefault();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), id_perfil);

                    objLstAcessoEmpresaPerfilTsTa = base.Listar<AcessoEmpresaPerfilTsTaConsultaTO>(accessToken, _urlBase, url);

                    vwPerfil.ListaAcessoEmpresaPerfilTsTa = objLstAcessoEmpresaPerfilTsTa;

                }

            }
            catch (Exception ex)
            {
                vwPerfil.erro = "Ocorreu um erro ao listar perfis. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuPerfil = "menu-ativo";
            return View("Perfil", vwPerfil);
        }

        public ActionResult AdicionaRelacionamentoPerfilTSTA(string id_perfil, string cod_tipo, string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao)
        {
            PerfilView vwPerfil = new PerfilView();
            PerfilTO objPerfil = new PerfilTO();
            List<PerfilTO> objLstPerfil = new List<PerfilTO>();
            List<AcessoEmpresaPerfilTsTaConsultaTO> objLstAcessoEmpresaPerfilTsTa = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            List<TipoAtividadeAcomodacaoTO> objLstTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoSituacaoAcomodacaoTO> objLstTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAcessoTO> objLstTipoAcesso = new List<TipoAcessoTO>();
            List<TipoPerfilTO> objLstTipoPerfil = new List<TipoPerfilTO>();
            AcessoEmpresaPerfilTsTaTO objAcessoEmpresaPerfilTsTa = new AcessoEmpresaPerfilTsTaTO();
            List<EmpresaPerfilConsultaTO> objLstEmpresaPerfil = new List<EmpresaPerfilConsultaTO>();
            EmpresaPerfilConsultaTO objEmpresaPerfilConsulta = new EmpresaPerfilConsultaTO();

            string id_PerfilAcessoPerfil = "";
            try
            {
                objLstTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();

                objLstTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();

                objLstTipoAcesso = base.RetornaTipoAcesso();

                objLstTipoPerfil = base.RetornaTipoPerfil();

                //Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];
                //string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];
                //string url = "api/Perfil/items";

                //vwPerfil.ListaPerfil = base.Listar<PerfilTO>(accessToken, _urlBase, url);

                vwPerfil.ListaTipoAtividadeAcomodacao = objLstTipoAtividadeAcomodacao;
                vwPerfil.ListaTipoSituacaoAcomodacao = objLstTipoSituacaoAcomodacao;
                vwPerfil.ListaTipoAcesso = objLstTipoAcesso;
                vwPerfil.ListaTipoPerfil = objLstTipoPerfil;

                if (string.IsNullOrWhiteSpace(id_perfil) || string.IsNullOrWhiteSpace(cod_tipo) || string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao))
                {
                    vwPerfil.erro = "O identificador  do perfil, o tipo de acesso, tipo de situação da acomodação e o tipo de atividade da acomodação são obrigatórios.";
                    return View("Perfil", vwPerfil);
                }
                else
                {
                    ModelState.Clear();

                    List<PerfilTO> lstRetorno = new List<PerfilTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/AcessoEmpresaPerfilTSTA/items");

                    objAcessoEmpresaPerfilTsTa.id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade = "0";
                    objAcessoEmpresaPerfilTsTa.id_Empresa = Session["id_EmpresaSel"].ToString();
                    objAcessoEmpresaPerfilTsTa.id_Perfil = id_perfil;
                    objAcessoEmpresaPerfilTsTa.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                    objAcessoEmpresaPerfilTsTa.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                    objAcessoEmpresaPerfilTsTa.cod_Tipo = cod_tipo;

                    base.Salvar<AcessoEmpresaPerfilTsTaTO>(accessToken, _urlBase, url, objAcessoEmpresaPerfilTsTa, ref id_PerfilAcessoPerfil);

                    url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), id_perfil);

                    objLstAcessoEmpresaPerfilTsTa = base.Listar<AcessoEmpresaPerfilTsTaConsultaTO>(accessToken, _urlBase, url);
                    
                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    url = string.Format("api/Perfil/items");

                    objLstPerfil = Listar<PerfilTO>(accessToken, _urlBase, url);


                    url = string.Format("api/EmpresaPerfil/items/{0}", Session["id_EmpresaSel"].ToString());

                    objLstEmpresaPerfil = base.Listar<EmpresaPerfilConsultaTO>(accessToken, _urlBase, url);

                    objEmpresaPerfilConsulta = objLstEmpresaPerfil.Where(m => m.id_Perfil == id_perfil.ToString()).FirstOrDefault();

                    if (objEmpresaPerfilConsulta != null)
                    {
                        vwPerfil.codTipoPerfil = objEmpresaPerfilConsulta.cod_Tipo_Perfil;
                    }

                    vwPerfil.Perfil = objLstPerfil.Where(m => m.id_Perfil == id_perfil.ToString()).FirstOrDefault();

                    vwPerfil.ListaAcessoEmpresaPerfilTsTa = objLstAcessoEmpresaPerfilTsTa;
                    
                    vwPerfil.mensagem = "Perfil atualizado com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwPerfil.erro = "Erro ao salvar ao adicionar relacionamento. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuPerfil = "menu-ativo";
            return View("Perfil", vwPerfil);
        }

        public ActionResult ExcluirRelacionamentoAcessoPerfil(string id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade, string id_perfil)
        {
            PerfilView vwPerfil = new PerfilView();
            PerfilTO objPerfil = new PerfilTO();
            List<PerfilTO> objLstPerfil = new List<PerfilTO>();
            List<AcessoEmpresaPerfilTsTaConsultaTO> objLstAcessoEmpresaPerfilTsTa = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            List<TipoAtividadeAcomodacaoTO> objLstTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoSituacaoAcomodacaoTO> objLstTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAcessoTO> objLstTipoAcesso = new List<TipoAcessoTO>();
            List<TipoPerfilTO> objLstTipoPerfil = new List<TipoPerfilTO>();
            AcessoEmpresaPerfilTsTaTO objAcessoEmpresaPerfilTsTa = new AcessoEmpresaPerfilTsTaTO();

            try
            {
                objLstTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();

                objLstTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();

                objLstTipoAcesso = base.RetornaTipoAcesso();

                objLstTipoPerfil = base.RetornaTipoPerfil();

                //Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];
                //string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];
                //string url = "api/Perfil/items";

                //vwPerfil.ListaPerfil = base.Listar<PerfilTO>(accessToken, _urlBase, url);

                vwPerfil.ListaTipoAtividadeAcomodacao = objLstTipoAtividadeAcomodacao;
                vwPerfil.ListaTipoSituacaoAcomodacao = objLstTipoSituacaoAcomodacao;
                vwPerfil.ListaTipoAcesso = objLstTipoAcesso;
                vwPerfil.ListaTipoPerfil = objLstTipoPerfil;

                if (string.IsNullOrWhiteSpace(id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade) )
                {
                    vwPerfil.erro = "O identificador do relacionamento é inválido.";
                    return View("Perfil", vwPerfil);
                }
                else
                {
                    ModelState.Clear();

                    List<PerfilTO> lstRetorno = new List<PerfilTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/AcessoEmpresaPerfilTSTA/items?Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade={0}", id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade);

                    base.Excluir(accessToken, _urlBase, url);

                    url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), id_perfil);

                    objLstAcessoEmpresaPerfilTsTa = base.Listar<AcessoEmpresaPerfilTsTaConsultaTO>(accessToken, _urlBase, url);

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    url = string.Format("api/Perfil/items");

                    objLstPerfil = Listar<PerfilTO>(accessToken, _urlBase, url);

                    vwPerfil.Perfil = objLstPerfil.Where(m => m.id_Perfil == id_perfil.ToString()).FirstOrDefault();

                    vwPerfil.ListaAcessoEmpresaPerfilTsTa = objLstAcessoEmpresaPerfilTsTa;

                    vwPerfil.mensagem = "Perfil atualizado com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwPerfil.erro = "Erro ao salvar ao adicionar relacionamento. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuPerfil = "menu-ativo";
            return View("Perfil", vwPerfil);
        }

        public ActionResult ExcluirPerfil(string id_perfil)
        {
            ListaPerfilView vwPerfil = new ListaPerfilView();
            List<PerfilTO> objLstPerfil = new List<PerfilTO>();
            try
            {

                if (id_perfil == null)
                {
                    throw new Exception("Identificador do perfil é inválido.");
                }
                else
                {

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    string url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), id_perfil);
                    
                    base.Excluir(accessToken, _urlBase, url);

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    _urlBase = ConfigurationManager.AppSettings["urlUsuario"];


                    url = string.Format("api/EmpresaPerfil/items?id_Empresa={0}&id_Perfil={1}", Session["id_EmpresaSel"].ToString(), id_perfil);

                    base.Excluir(accessToken, _urlBase, url);


                    url = string.Format("api/Perfil/items?id_Perfil={0}", id_perfil);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir um perfil.");
                    }

                    url = string.Format("api/Perfil/items");

                    objLstPerfil = Listar<PerfilTO>(accessToken, _urlBase, url);

                    vwPerfil.ListaPerfis = objLstPerfil;

                    vwPerfil.mensagem = "Perfil excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwPerfil.erro = "Erro ao tentar excluir perfil. Erro:" + ex.Message;
            }
            ViewBag.MenuPerfil = "menu-ativo";

            return View("ListaPerfil", vwPerfil);
        }

        public ActionResult SalvarPerfil(PerfilView model)
        {
            PerfilView vwPerfil = new PerfilView();
            PerfilTO objPerfil = new PerfilTO();
            List<PerfilTO> objLstPerfil = new List<PerfilTO>();
            List<AcessoEmpresaPerfilTsTaConsultaTO> objLstAcessoEmpresaPerfilTsTa = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            List<TipoAtividadeAcomodacaoTO> objLstTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            List<TipoSituacaoAcomodacaoTO> objLstTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            List<TipoAcessoTO> objLstTipoAcesso = new List<TipoAcessoTO>();
            List<TipoPerfilTO> objLstTipoPerfil = new List<TipoPerfilTO>();
            EmpresaPerfilTO objEmpresaPerfil = new EmpresaPerfilTO();

            string id_Perfil = "";
            string id_EmpresaPerfil = "";
            try
            {
                objLstTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();

                objLstTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();

                objLstTipoAcesso = base.RetornaTipoAcesso();

                objLstTipoPerfil = base.RetornaTipoPerfil();


                vwPerfil.ListaTipoAcesso = objLstTipoAcesso;
                vwPerfil.ListaTipoAtividadeAcomodacao = objLstTipoAtividadeAcomodacao;
                vwPerfil.ListaTipoSituacaoAcomodacao = objLstTipoSituacaoAcomodacao;
                vwPerfil.ListaTipoPerfil = objLstTipoPerfil;

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];
                string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];
                
                string url = "api/Perfil/items";

                vwPerfil.ListaPerfil = base.Listar<PerfilTO>(accessToken, _urlBase, url);

                if (!ModelState.IsValid)
                {
                    return View("Perfil", model);
                }
                else
                {
                    ModelState.Clear();

                    List<PerfilTO> lstRetorno = new List<PerfilTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    url = string.Format("api/Perfil/items");

                    if (string.IsNullOrWhiteSpace(model.Perfil.id_Perfil))
                    {
                        model.Perfil.id_Perfil = "0";
                    }
                    else
                    {
                        id_Perfil = model.Perfil.id_Perfil;
                    }

                    base.Salvar<PerfilTO>(accessToken, _urlBase, url, model.Perfil, ref id_Perfil);

                    //if (model.Perfil.id_Perfil != "0")
                    //    {
                    //    url = string.Format("api/EmpresaPerfil/items?id_Empresa={0}&id_Perfil={1}", Session["id_EmpresaSel"].ToString(), model.Perfil.id_Perfil);
                    //    base.Excluir(accessToken, _urlBase, url);
                    //}
                    
                    objEmpresaPerfil.id_Empresa = Session["id_EmpresaSel"].ToString();
                    objEmpresaPerfil.id_Perfil = id_Perfil;
                    objEmpresaPerfil.cod_Tipo = model.codTipoPerfil;

                    url = string.Format("api/EmpresaPerfil/items");

                    base.Salvar<EmpresaPerfilTO>(accessToken, _urlBase, url, objEmpresaPerfil, ref id_EmpresaPerfil);
                    
                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                    url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), id_Perfil);

                    objLstAcessoEmpresaPerfilTsTa = base.Listar<AcessoEmpresaPerfilTsTaConsultaTO>(accessToken, _urlBase, url);

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    url = string.Format("api/Perfil/items");

                    objLstPerfil = Listar<PerfilTO>(accessToken, _urlBase, url);

                    vwPerfil.Perfil = objLstPerfil.Where(m => m.id_Perfil == id_Perfil.ToString()).FirstOrDefault();

                    vwPerfil.ListaAcessoEmpresaPerfilTsTa = objLstAcessoEmpresaPerfilTsTa;

                    vwPerfil.codTipoPerfil = model.codTipoPerfil;

                    vwPerfil.mensagem = "Perfil salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwPerfil.erro = "Erro ao salvar o perfil. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuPerfil = "menu-ativo";
            return View("Perfil", vwPerfil);
        }

        #endregion

        #region "Usuário"

        public ActionResult ListaUsuario()
        {
            ListaUsuarioView vwListaUsuarios = new ListaUsuarioView();
            try
            {

                //List<UsuarioTO> lstRetorno = new List<UsuarioTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                string url = string.Format("api/Usuario/items/empresa?idEmpresa={0}", Session["id_EmpresaSel"].ToString());

                ViewBag.MenuUsuario = "menu-ativo";

                vwListaUsuarios.ListaUsuarios = base.Listar<UsuarioTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaUsuarios.erro = "Ocorreu um erro ao listar usuários. Detalhe: " + ex.Message;
                // throw ex;
            }

            return View("ListaUsuario", vwListaUsuarios);
        }

        public ActionResult Usuario(string id_usuario)
        {
            UsuarioView vwUsuario = new UsuarioView();
            UsuarioTO objUsuario = new UsuarioTO();
            List<UsuarioTO> objLstUsuario = new List<UsuarioTO>();
            List<PerfilItemsTO> perfilItems = new List<PerfilItemsTO>();
            try
            {
                vwUsuario.ListaPerfil = ListaPerfis();

                if (id_usuario == null)
                {
                    vwUsuario.Usuario = new UsuarioTO();
                }
                else
                {

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    string url = string.Format("api/Usuario/items/empresa?idEmpresa={0}", Session["id_EmpresaSel"].ToString());
                    //string url = string.Format("api/Usuario/items/{0}", id_usuario);

                    objLstUsuario = Listar<UsuarioTO>(accessToken, _urlBase, url);

                    objUsuario = objLstUsuario.Where(m => m.id_Usuario == id_usuario).FirstOrDefault();

                    url = string.Format("api/UsuarioEmpresaPerfil/items/usuario/{0}", id_usuario);

                    perfilItems = Listar<PerfilItemsTO>(accessToken, _urlBase, url);

                    //foreach (PerfilItemsTO item in objLstUsuario[0].usuarioEmpresaPerfilItems)
                    //{
                    //    vwUsuario.ListaPerfilSel += item.id_Perfil.PadLeft(2, '0') + "#";
                    //}
                    objUsuario.usuarioEmpresaPerfilItems = perfilItems;
                    foreach (PerfilItemsTO item in objUsuario.usuarioEmpresaPerfilItems)
                    {
                        vwUsuario.ListaPerfilSel += item.id_Perfil.PadLeft(2, '0') + "#";
                    }

                    //objUsuario.senha_Usuario = "";
                    //vwUsuario.Usuario = objLstUsuario[0];
                    vwUsuario.Usuario = objUsuario;
                }

            }
            catch (Exception ex)
            {
                vwUsuario.erro = "Ocorreu um erro ao consultar o usuário. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuUsuario = "menu-ativo";
            return View("Usuario", vwUsuario);
        }

        public ActionResult ExcluirUsuario(decimal? id_usuario)
        {
            ListaUsuarioView vwListaUsuario = new ListaUsuarioView();
            List<UsuarioTO> objLstUsuario = new List<UsuarioTO>();
            try
            {

                if (id_usuario == null)
                {
                    throw new Exception("Identificador do usuário é inválido.");
                }
                else
                {

                    List<UsuarioTO> lstRetorno = new List<UsuarioTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    string url = string.Format("api/Usuario/items?id_Usuario={0}", id_usuario);

                    //base.Consultar<UsuarioTO>(accessToken, _urlBase, url, ref objUsuario);
                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir usuário.");
                    }


                    url = string.Format("api/Usuario/items/empresa?idEmpresa={0}", Session["id_EmpresaSel"].ToString());


                    objLstUsuario = Listar<UsuarioTO>(accessToken, _urlBase, url);

                    vwListaUsuario.ListaUsuarios = objLstUsuario;

                    vwListaUsuario.mensagem = "Usuario excluído com sucesso.";

                }

            }
            catch (Exception ex)
            {
                vwListaUsuario.erro = "Erro ao tentar excluir usuário. Erro:" + ex.Message;
            }
            ViewBag.MenuUsuario = "menu-ativo";
            return View("ListaUsuario", vwListaUsuario);
        }

        public ActionResult SalvarUsuario(UsuarioView model)
        {
            UsuarioView vwUsuario = new UsuarioView();
            UsuarioTO objUsuario = new UsuarioTO();
            List<UsuarioTO> objLstUsuario = new List<UsuarioTO>();
            List<PerfilItemsTO> perfilItems = new List<PerfilItemsTO>();
            PerfilItemsTO perfilItem = new PerfilItemsTO();
            string id_Usuario = "";
            try
            {
                vwUsuario.ListaPerfil = ListaPerfis();
                
                if (!ModelState.IsValid)
                {
                    model.ListaPerfil = vwUsuario.ListaPerfil;
                    return View("Usuario", model);
                }
                else
                {
                    ModelState.Clear();

                    List<UsuarioTO> lstRetorno = new List<UsuarioTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                    string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                    string url = string.Format("api/Usuario/items");


                    if (string.IsNullOrWhiteSpace(model.Usuario.id_Usuario))
                    {
                        model.Usuario.id_Usuario = "0";
                        // carrega o objeto para salvar os itens.
                        foreach (string item in model.ListaPerfilSel.Split('#'))
                        {
                            if (!string.IsNullOrWhiteSpace(item.Trim()))
                            {
                                perfilItem = new PerfilItemsTO();
                                perfilItem.id_Usuario = model.Usuario.id_Usuario;
                                perfilItem.id_Empresa = Session["id_EmpresaSel"].ToString();
                                perfilItem.id_Perfil = int.Parse(item).ToString();
                                perfilItems.Add(perfilItem);
                            }

                        }
                        model.Usuario.usuarioEmpresaPerfilItems = perfilItems;
                        base.Salvar<UsuarioTO>(accessToken, _urlBase, url, model.Usuario, ref id_Usuario);
                        model.Usuario.id_Usuario = id_Usuario;
                    }
                    else
                    {

                        foreach (string item in model.ListaPerfilSel.Split('#'))
                        {
                            if (!string.IsNullOrWhiteSpace(item.Trim()))
                            {
                                perfilItem = new PerfilItemsTO();
                                perfilItem.id_Usuario = model.Usuario.id_Usuario;
                                perfilItem.id_Empresa = Session["id_EmpresaSel"].ToString();
                                perfilItem.id_Perfil = int.Parse(item).ToString();
                                perfilItems.Add(perfilItem);
                            }

                        }
                        model.Usuario.usuarioEmpresaPerfilItems = perfilItems;
                        base.Salvar<UsuarioTO>(accessToken, _urlBase, url, model.Usuario, ref id_Usuario);
                        model.Usuario.id_Usuario = id_Usuario;
                        
                        ////url = string.Format("api/Usuario/items/nome?id_Usuario={0}&nome_Usuario={1}", model.Usuario.id_Usuario, model.Usuario.nome_Usuario);

                        ////this.Enviar(accessToken, _urlBase, url, HttpMethod.Put);

                        ////url = string.Format("api/Usuario/items/login?id_Usuario={0}&login_Usuario={1}", model.Usuario.id_Usuario, model.Usuario.login_Usuario);

                        ////this.Enviar(accessToken, _urlBase, url, HttpMethod.Put);

                        ////if (!string.IsNullOrWhiteSpace(model.Usuario.senha_Usuario))
                        ////{
                        ////    url = string.Format("api/Usuario/items/senha?id_Usuario={0}&senha_Usuario={1}", model.Usuario.id_Usuario, model.Usuario.senha_Usuario);
                        ////    this.Enviar(accessToken, _urlBase, url, HttpMethod.Put);
                        ////}

                        ////if (!string.IsNullOrWhiteSpace(model.ListaPerfilSel))
                        ////{

                        ////    url = string.Format("api/UsuarioEmpresaPerfil/items/{0}", model.Usuario.id_Usuario);

                        ////    this.Excluir(accessToken, _urlBase, url);


                        ////    // carrega o objeto para salvar os itens.
                        ////    foreach (string item in model.ListaPerfilSel.Split('#'))
                        ////    {
                        ////        if (!string.IsNullOrWhiteSpace(item.Trim()))
                        ////        {
                        ////            perfilItem = new PerfilItemsTO();
                        ////            perfilItem.id_Usuario = model.Usuario.id_Usuario;
                        ////            perfilItem.id_Empresa = Session["id_EmpresaSel"].ToString();
                        ////            perfilItem.id_Perfil = int.Parse(item).ToString();
                        ////            perfilItems.Add(perfilItem);
                        ////        }

                        ////    }

                        ////    url = string.Format("api/UsuarioEmpresaPerfil/items/grupo");
                        ////    string lRetorno="";
                        ////    this.Salvar<List<PerfilItemsTO>>(accessToken, _urlBase, url, perfilItems, ref lRetorno);
                        ////}

                        ///**************************************  Alterar AQUI - Não atualiza o nome do usuário. (Não havia método)

                    }


                    url = string.Format("api/Usuario/items/empresa?idEmpresa={0}", Session["id_EmpresaSel"].ToString());

                    objLstUsuario = Listar<UsuarioTO>(accessToken, _urlBase, url);

                    objUsuario = objLstUsuario.Where(m => m.id_Usuario == model.Usuario.id_Usuario).FirstOrDefault();

                    url = string.Format("api/UsuarioEmpresaPerfil/items/usuario/{0}", model.Usuario.id_Usuario);

                    perfilItems = new List<PerfilItemsTO>(); 

                    perfilItems = Listar<PerfilItemsTO>(accessToken, _urlBase, url);

                    objUsuario.usuarioEmpresaPerfilItems = perfilItems;
                    foreach (PerfilItemsTO item in objUsuario.usuarioEmpresaPerfilItems)
                    {
                        vwUsuario.ListaPerfilSel += item.id_Perfil.PadLeft(2, '0') + "#";
                    }

                    //objUsuario.senha_Usuario = "";
                    
                    vwUsuario.Usuario = objUsuario;

                    vwUsuario.mensagem = "Usuário salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwUsuario.erro = "Erro ao salvar usuário. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuUsuario = "menu-ativo";
            return View("Usuario", vwUsuario);
        }

        #endregion
    }
}
