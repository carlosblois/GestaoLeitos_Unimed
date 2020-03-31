using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web.Security;
using System.Configuration;
using GLeitos.Models;
using ExpoFramework.Framework.Utils;
using GLeitos.GLeitosTO;
using System.Linq;
using System.Threading;

namespace GLeitos.Controllers
{
    public class LoginController : BaseController
    {

        //[AllowAnonymous]
        public ActionResult Login(LoginView pLogin)
        {
            LoginView lLogin = new LoginView();
            List<LogarUsuarioEmpresaPerfilTO> llogarUsuarioEmpresaPerfil = new List<LogarUsuarioEmpresaPerfilTO>();

            if (string.IsNullOrWhiteSpace(pLogin.Login.login))
            {

                return View(lLogin);
            }

            if (!ModelState.IsValid)
            {
                return View(lLogin);
            }

            try
            {
                llogarUsuarioEmpresaPerfil = autenticar(pLogin.Login.login, pLogin.Login.senha);

                if (llogarUsuarioEmpresaPerfil==null)
                {
                    pLogin.erro = "Usuário inválido.";
                }

                if (llogarUsuarioEmpresaPerfil[0].id_Usuario==0)
                {
                    pLogin.erro = "Usuário inválido.";
                }

                if (Session["ListaUsuarioEmpresaPerfil"]!=null)
                {
                    pLogin.logarUsuarioEmpresaPerfilTO = (List<LogarUsuarioEmpresaPerfilTO>)Session["ListaUsuarioEmpresaPerfil"];
                }

                if (pLogin.logarUsuarioEmpresaPerfilTO.Count <= 0) {

                    DateTime cookieIssuedDate = DateTime.Now;

                    var _ticket = new FormsAuthenticationTicket(0,
                        pLogin.Login.login,
                        cookieIssuedDate,
                        cookieIssuedDate.AddMinutes(double.Parse(ConfigurationManager.AppSettings.Get("timeoutLogin"))),
                        true,
                        pLogin.Login.login.ToString(),
                        FormsAuthentication.FormsCookiePath);

                    string _encrypted = FormsAuthentication.Encrypt(_ticket);

                    FormsAuthentication.SetAuthCookie(pLogin.Login.login, true);

                    var _cookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, true);

                    _cookie.Value = _encrypted;
                    _cookie.Expires = _ticket.Expiration;

                    System.Web.HttpContext.Current.User = new GenericPrincipal(new ExpoIdentity(_ticket), null);

                    System.Web.HttpContext.Current.Response.Cookies.Add(_cookie);

                    pLogin.logarUsuarioEmpresaPerfilTO = llogarUsuarioEmpresaPerfil;

                    Session.Add("id_Usuario", llogarUsuarioEmpresaPerfil[0].id_Usuario);
                    Session.Add("nome_Usuario", llogarUsuarioEmpresaPerfil[0].nome_Usuario);
                    Session.Add("ListaUsuarioEmpresaPerfil", llogarUsuarioEmpresaPerfil);

                }
                else
                {
                    DateTime cookieIssuedDate = DateTime.Now;

                    var _ticket = new FormsAuthenticationTicket(0,
                        pLogin.Login.login,
                        cookieIssuedDate,
                        cookieIssuedDate.AddMinutes(double.Parse(ConfigurationManager.AppSettings.Get("timeoutLogin"))),
                        true,
                        pLogin.Login.login.ToString(),
                        FormsAuthentication.FormsCookiePath);

                    string _encrypted = FormsAuthentication.Encrypt(_ticket);

                    FormsAuthentication.SetAuthCookie(pLogin.Login.login, true);

                    var _cookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, true);

                    _cookie.Value = _encrypted;
                    _cookie.Expires = _ticket.Expiration;

                    System.Web.HttpContext.Current.User = new GenericPrincipal(new ExpoIdentity(_ticket), null);

                    System.Web.HttpContext.Current.Response.Cookies.Add(_cookie);
                }
                
                pLogin.logarUsuarioEmpresaTO = RetornarEmpresas(llogarUsuarioEmpresaPerfil);

                Session.Add("ListaUsuarioEmpresa", pLogin.logarUsuarioEmpresaTO);
                if (pLogin.logarUsuarioEmpresaTO.Count == 1)
                {
                    pLogin.Login.id_empresa = pLogin.logarUsuarioEmpresaTO[0].id_Empresa.ToString();
                    pLogin.logarUsuarioPerfilTO = RetornarPerfisEmpresa(llogarUsuarioEmpresaPerfil, pLogin.logarUsuarioEmpresaTO[0].id_Empresa);
                    Session.Add("ListaUsuarioPerfil", pLogin.logarUsuarioPerfilTO);
                    if (pLogin.logarUsuarioPerfilTO.Count == 1)
                    {
                        pLogin.Login.id_perfil = pLogin.logarUsuarioPerfilTO[0].id_Perfil.ToString();
                        Session.Add("id_EmpresaSel", pLogin.Login.id_empresa);
                        Session.Add("id_PerfilSel", pLogin.Login.id_perfil);
                        Session.Add("cod_TipoUsuarioPerfilSel", pLogin.logarUsuarioEmpresaPerfilTO[0].cod_Tipo);
                        //return RedirectToAction("Index", "Adm");
                        return RedirectToAction("Dashboard", "Dashboard");

                    }
                    else
                    if (!string.IsNullOrWhiteSpace(pLogin.Login.id_perfil))
                    {
                        Session.Add("id_EmpresaSel", pLogin.Login.id_empresa);
                        Session.Add("id_PerfilSel", pLogin.Login.id_perfil);
                        string perfilSel = pLogin.logarUsuarioEmpresaPerfilTO.Where(m => m.id_Perfil.ToString() == pLogin.Login.id_perfil).FirstOrDefault().cod_Tipo;
                        Session.Add("cod_TipoUsuarioPerfilSel", perfilSel);
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                else
                if (!string.IsNullOrWhiteSpace(pLogin.Login.id_empresa)) 
                {
                    pLogin.logarUsuarioPerfilTO = RetornarPerfisEmpresa(llogarUsuarioEmpresaPerfil, int.Parse(pLogin.Login.id_empresa));
                    Session.Add("ListaUsuarioPerfil", pLogin.logarUsuarioPerfilTO);
                    if (string.IsNullOrWhiteSpace(pLogin.Login.id_perfil))
                    {
                        if (pLogin.logarUsuarioPerfilTO.Count == 1)
                        {
                            pLogin.Login.id_perfil = pLogin.logarUsuarioPerfilTO[0].id_Perfil.ToString();
                            Session.Add("id_EmpresaSel", pLogin.Login.id_empresa);
                            Session.Add("id_PerfilSel", pLogin.Login.id_perfil);
                            Session.Add("cod_TipoUsuarioPerfilSel", pLogin.logarUsuarioEmpresaPerfilTO[0].cod_Tipo);
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                    }
                    else
                    {
                        Session.Add("id_EmpresaSel", pLogin.Login.id_empresa);
                        Session.Add("id_PerfilSel", pLogin.Login.id_perfil);
                        string perfilSel = pLogin.logarUsuarioEmpresaPerfilTO.Where(m => m.id_Perfil.ToString() == pLogin.Login.id_perfil).FirstOrDefault().cod_Tipo;
                        Session.Add("cod_TipoUsuarioPerfilSel", perfilSel);
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                else
                {
                    throw new Exception("Selecione uma empresa.");
                }

                //return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                pLogin.erro = "Erro ao validar o usuário. " + ex.Message;
            }

            return View(pLogin);
        }

        public ActionResult Logoff()
        {
            LoginView lLogin = new LoginView();

            Session.Abandon();
            FormsAuthentication.SignOut();

            return View("Login", lLogin);
        }

        public ActionResult LogoffAcesso(string Mensagem)
        {
            LoginView lLogin = new LoginView();

            Session.Abandon();
            FormsAuthentication.SignOut();
            lLogin.erro = Mensagem;
            return View("Login", lLogin);
        }

        private List<LogarUsuarioEmpresaTO> RetornarEmpresas(List<LogarUsuarioEmpresaPerfilTO> pListaEmpresaPerfil)
        {
            List<LogarUsuarioEmpresaTO> lListaEmpresaRet = new List<LogarUsuarioEmpresaTO>();
            LogarUsuarioEmpresaTO lItemEmpresa = new LogarUsuarioEmpresaTO();
            try
            {
                int id_empresa_atual = 0;
                foreach(LogarUsuarioEmpresaPerfilTO item in pListaEmpresaPerfil.OrderBy(m => m.id_Empresa))
                {
                    if (id_empresa_atual!=item.id_Empresa)
                    {
                        lItemEmpresa = new LogarUsuarioEmpresaTO();
                        lItemEmpresa.id_Empresa = item.id_Empresa;
                        lItemEmpresa.nome_Empresa = item.nome_Empresa;
                        lListaEmpresaRet.Add(lItemEmpresa);
                    }

                    id_empresa_atual = item.id_Empresa;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lListaEmpresaRet;
        }

        private List<LogarUsuarioPerfilTO> RetornarPerfisEmpresa(List<LogarUsuarioEmpresaPerfilTO> pListaEmpresaPerfil, int pId_Empresa)
        {
            List<LogarUsuarioPerfilTO> lListaPerfisRet = new List<LogarUsuarioPerfilTO>();
            LogarUsuarioPerfilTO lItemPerfil = new LogarUsuarioPerfilTO();
            try
            {
                int id_perfil_atual = 0;
                foreach (LogarUsuarioEmpresaPerfilTO item in pListaEmpresaPerfil.Where(n => n.id_Empresa==pId_Empresa).OrderBy(m => m.id_Empresa))
                {
                    if (id_perfil_atual != item.id_Perfil)
                    {
                        lItemPerfil = new LogarUsuarioPerfilTO();
                        lItemPerfil.id_Perfil = item.id_Perfil;
                        lItemPerfil.nome_Perfil = item.nome_Perfil;
                        lListaPerfisRet.Add(lItemPerfil);
                    }

                    id_perfil_atual = item.id_Perfil;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lListaPerfisRet;
        }

        
    }
}