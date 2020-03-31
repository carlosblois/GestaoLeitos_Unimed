using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using Newtonsoft.Json;
using GLeitos.Models;
using ExpoFramework.Framework;
using System.Threading.Tasks;
using System.Threading;


namespace GLeitos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Application["tokenOperacional"] = GetAccessToken(Enumeradores.modulo.operacional);
            Application["tokenUsuario"] = GetAccessToken(Enumeradores.modulo.usuario);
            Application["tokenConfiguracao"] = GetAccessToken(Enumeradores.modulo.configuracao);
            Application["tokenAdministrativo"] = GetAccessToken(Enumeradores.modulo.administrativo);
        }

        /// <summary>
        /// This method uses the OAuth Client Credentials Flow to get an Access Token to provide
        /// Authorization to the APIs.
        /// </summary>
        /// <returns></returns>
        private static Token GetAccessToken(Enumeradores.modulo pModulo)
        {

            // Envio da requisição a fim de autenticar
            // e obter o token de acesso
            string _urlBase = ConfigurationManager.AppSettings["urlGetToken"];
            string _UserID = "";
            string _UserSecret = "";
            string _ScopeKey = "";
            Token token = new Token();

            switch (pModulo)
            {
                case Enumeradores.modulo.administrativo:
                    _UserID = ConfigurationManager.AppSettings["clientIdentificationAdministrativo"];
                    _UserSecret = ConfigurationManager.AppSettings["clientSecretAdministrativo"];
                    _ScopeKey = ConfigurationManager.AppSettings["scopeAdministrativo"];
                    break;
                case Enumeradores.modulo.usuario:
                    _UserID = ConfigurationManager.AppSettings["clientIdentificationUsuario"];
                    _UserSecret = ConfigurationManager.AppSettings["clientSecretUsuario"];
                    _ScopeKey = ConfigurationManager.AppSettings["scopeUsuario"];
                    break;
                case Enumeradores.modulo.configuracao:
                    _UserID = ConfigurationManager.AppSettings["clientIdentificationConfiguracao"];
                    _UserSecret = ConfigurationManager.AppSettings["clientSecretConfiguracao"];
                    _ScopeKey = ConfigurationManager.AppSettings["scopeConfiguracao"];
                    break;
                case Enumeradores.modulo.operacional:
                    _UserID = ConfigurationManager.AppSettings["clientIdentificationOperacional"];
                    _UserSecret = ConfigurationManager.AppSettings["clientSecretOperacional"];
                    _ScopeKey = ConfigurationManager.AppSettings["scopeOperacional"];
                    break;
                default:
                    break;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["urlGetToken"]);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Construindo os dados para POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                postData.Add(new KeyValuePair<string, string>("client_id", _UserID));
                postData.Add(new KeyValuePair<string, string>("client_secret", _UserSecret));
                postData.Add(new KeyValuePair<string, string>("scope", _ScopeKey));


                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

               
                HttpResponseMessage response = client.PostAsync("Token", content).Result;
                string jsonString = response.Content.ReadAsStringAsync().Result;
                object responseData = JsonConvert.DeserializeObject(jsonString);

                string strObj = JsonConvert.SerializeObject(responseData);
                token = JsonConvert.DeserializeObject<Token>(strObj);
                return token;
            }
        }
    }
}
