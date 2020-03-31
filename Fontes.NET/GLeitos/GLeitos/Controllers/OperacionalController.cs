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
    public class OperacionalController : BaseController
    {
        #region "Home"
            public ActionResult Operacoes()
            {
                OperacaoView vwOperacao = new OperacaoView();
                return View(vwOperacao);
            }
        #endregion

        #region "Internação"
                
        public ActionResult Internacao()
        {
            OperacaoView vwOperacao = new OperacaoView();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Operacoes", vwOperacao);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                    string url = string.Format("api/Integracao/items/internacao/empresa/{0}/codexterno/{1}/nomepaciente/{2}/codexternopaciente/{3}/numatendimento/{4}/genero/{5}/datanascimento/{6}", "1", "PED01", "Beatriz", "1004", "25015", "F", "2001-03-27 00:00:00");
                    //string url = string.Format("api/Integracao/items/internacao/empresa/{0}/codexterno/{1}/nomepaciente/{2}/codexternopaciente/{3}/numatendimento/{4}/genero/{5}/datanascimento/{6}", "1", HttpUtility.UrlEncode("118 A"), "Alexandre", "1001", "25012", "M", HttpUtility.UrlEncode("1975-12-16 00:00:00"));
                    //string url = string.Format("api/Integracao/items/internacao/empresa={0}&codexterno={1}&nomepaciente={2}&codexternopaciente={3}&numatendimento={4}&genero={5}&datanascimento={6}", "1", "118 A", "Alexandre", "1001", "25012", "M", "1975-12-16 00:00:00");


                    vwOperacao.mensagem = base.Enviar(accessToken, _urlBase, url, HttpMethod.Post);

                }

            }
            catch (Exception ex)
            {
                vwOperacao.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Operacoes", vwOperacao);
        }

        public ActionResult Reserva()
        {
            OperacaoView vwOperacao = new OperacaoView();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Operacoes", vwOperacao);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                    string url = string.Format("api/Integracao/items/reserva/empresa/{0}/codexterno/{1}/nomepaciente/{2}/codexternopaciente/{3}/genero/{4}/datanascimento/{5}", "1", "PED01", "Beatriz", "1004", "M", "2001-03-27 00:00:00");
                    //string url = string.Format("api/Integracao/items/internacao/empresa/{0}/codexterno/{1}/nomepaciente/{2}/codexternopaciente/{3}/numatendimento/{4}/genero/{5}/datanascimento/{6}", "1", HttpUtility.UrlEncode("118 A"), "Alexandre", "1001", "25012", "M", HttpUtility.UrlEncode("1975-12-16 00:00:00"));
                    //string url = string.Format("api/Integracao/items/internacao/empresa={0}&codexterno={1}&nomepaciente={2}&codexternopaciente={3}&numatendimento={4}&genero={5}&datanascimento={6}", "1", "118 A", "Alexandre", "1001", "25012", "M", "1975-12-16 00:00:00");


                    vwOperacao.mensagem = base.Enviar(accessToken, _urlBase, url, HttpMethod.Post);

                }

            }
            catch (Exception ex)
            {
                vwOperacao.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Operacoes", vwOperacao);
        }

        public ActionResult AltaMedica()
        {
            OperacaoView vwOperacao = new OperacaoView();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Operacoes", vwOperacao);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                    string url = string.Format("api/Integracao/items/altamedica/empresa/{0}/codexterno/{1}/codexternopaciente/{2}/numatendimento/{3}/pendenciafinanceira/{4}", "1", "PED02", "1004", "25015", "N");

                    vwOperacao.mensagem = base.Enviar(accessToken, _urlBase, url, HttpMethod.Post);

                }

            }
            catch (Exception ex)
            {
                vwOperacao.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Operacoes", vwOperacao);
        }

        public ActionResult AltaHospitalar()
        {
            OperacaoView vwOperacao = new OperacaoView();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Operacoes", vwOperacao);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                    string url = string.Format("api/Integracao/items/altahospitalar/empresa/{0}/codexterno/{1}/codexternopaciente/{2}/numatendimento/{3}", "1", "PED02", "1004", "25015");

                    vwOperacao.mensagem = base.Enviar(accessToken, _urlBase, url, HttpMethod.Post);

                }

            }
            catch (Exception ex)
            {
                vwOperacao.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Operacoes", vwOperacao);
        }

        public ActionResult Transferencia()
        {
            OperacaoView vwOperacao = new OperacaoView();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Operacoes", vwOperacao);
                }
                else
                {
                    ModelState.Clear();

                    List<EmpresaTO> lstRetorno = new List<EmpresaTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                    string url = string.Format("api/Integracao/items/transferencia/empresa/{0}/codexternoOrigem/{1}/numatendimentoOrigem/{2}/codexternoDestino/{3}/numatendimentoDestino/{4}", "1", "PED01", "25015", "PED02", "25015");

                    vwOperacao.mensagem = base.Enviar(accessToken, _urlBase, url, HttpMethod.Post);

                }

            }
            catch (Exception ex)
            {
                vwOperacao.erro = "Erro ao enviar operacao. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuEmpresa = "menu-ativo";
            return View("Operacoes", vwOperacao);
        }

        #endregion
                
    }
}
