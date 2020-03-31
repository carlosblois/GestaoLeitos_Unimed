using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using GLeitos.Models;
using System.Web.Mvc;
using System.Web;
using System.Threading.Tasks;
using GLeitos.GLeitosTO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Runtime.Caching;
using ExpoFramework.Framework;

namespace GLeitos.Controllers
{
    public class BaseController: Controller
    {

        public static List<LogarUsuarioEmpresaPerfilTO> autenticar(string usuario, string senha)
        {
            try
            {

                List<LogarUsuarioEmpresaPerfilTO> lstRetorno = new List<LogarUsuarioEmpresaPerfilTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenUsuario"];

                //DateTime expiryDate = (DateTime)(accessToken.expires_in));

                string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_urlBase);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.access_token);

                    // create the URL string.
                    string url = string.Format("api/Usuario/items/perfil?login_Usuario={0}&senha_Usuario={1}", usuario, senha);

                    // make the request
                    HttpResponseMessage response = client.GetAsync(url).Result;


                    if (response.StatusCode.ToString().ToUpper()!="OK".ToUpper())
                    {
                        switch (response.StatusCode.ToString())
                        {
                        case "NotFound":
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result); 
                        }
                        case "BadRequest":
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result);
                        }
                        case "400":
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result);
                        }
                        case "401":
                        {
                            throw new Exception("Usuário não autorizado a acessar o sistema.");
                        }
                        case "403":
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result);
                        }
                        case "404":
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result); 
                        }
                        default:
                            {
                                throw new Exception("Erro ao solicitar autorização para acesso.");
                            }
                        }
                    }

                    // parse the response and return the data.
                    string jsonString = response.Content.ReadAsStringAsync().Result;

                    object responseData = JsonConvert.DeserializeObject(jsonString);

                    string strObj = JsonConvert.SerializeObject(responseData);

                    lstRetorno = JsonConvert.DeserializeObject<List<LogarUsuarioEmpresaPerfilTO>>(strObj);

                    return lstRetorno;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool validarusuario(HttpClient client, string usuario, string senha)
        {

            string _urlBase = ConfigurationManager.AppSettings["urlUsuario"];

            HttpResponseMessage response = client.GetAsync(
                _urlBase + "api/Usuario/items/perfil?login_Usuario=" + usuario + "&senha_Usuario=" + senha).Result;

            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;

        }

        public List<T> Listar<T>(Token pToken, string pUrlBase, string pUrl)
        {
            List<T> lstRetorno = new List<T>();
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(pUrlBase);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + pToken.access_token);

                    // make the request
                    HttpResponseMessage response = client.GetAsync(pUrl).Result;

                    if (response.StatusCode.ToString().ToUpper() != "OK".ToUpper())
                    {
                        switch (response.StatusCode.ToString())
                        {
                            case "NotFound":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "BadRequest":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "400":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "401":
                                {
                                    throw new Exception("Usuário não autorizado a acessar o sistema.");
                                }
                            case "403":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "404":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            default:
                                {
                                    throw new Exception("Erro ao solicitar autorização para acesso.");
                                }
                        }
                    }

                    // parse the response and return the data.
                    string jsonString = response.Content.ReadAsStringAsync().Result;

                    object responseData = JsonConvert.DeserializeObject(jsonString);

                    string strObj = JsonConvert.SerializeObject(responseData);

                    strObj.Replace(":null", ":\"\"");

                    lstRetorno = JsonConvert.DeserializeObject<List<T>>(strObj);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstRetorno;
        }

        public void Salvar<T>(Token pToken, string pUrlBase, string pUrl, T objContent, ref string id_Retorno)
        {
            List<T> lstRetorno = new List<T>();
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(pUrlBase);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + pToken.access_token);


                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, pUrl);
                    request.Content = new StringContent(JsonConvert.SerializeObject(objContent),
                                                        Encoding.UTF8,
                                                        "application/json");//CONTENT-TYPE header

                    // make the request
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        switch (response.StatusCode.ToString())
                        {
                            case "NotFound":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "BadRequest":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "400":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "401":
                                {
                                    throw new Exception("Usuário não autorizado a acessar o sistema.");
                                }
                            case "403":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "404":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            default:
                                {
                                    throw new Exception("Erro ao solicitar autorização para acesso.");
                                }
                        }
                    }

                    // parse the response and return the data.
                    string jsonString = response.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrWhiteSpace(jsonString))
                        id_Retorno = jsonString;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Excluir(Token pToken, string pUrlBase, string pUrl)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(pUrlBase);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + pToken.access_token);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, pUrl);

                    //// make the request
                    HttpResponseMessage response = client.SendAsync(request).Result;
                                        
                    if (!response.IsSuccessStatusCode)
                    {
                        switch (response.StatusCode.ToString())
                        {
                            case "NotFound":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "401":
                                {
                                    throw new Exception("Usuário não autorizado a acessar o sistema.");
                                }
                            case "403":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "404":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            default:
                                {
                                    throw new Exception("Erro ao solicitar autorização para acesso.");
                                }
                        }
                    }

                    // parse the response and return the data.
                    string jsonString = response.Content.ReadAsStringAsync().Result;

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Enviar(Token pToken, string pUrlBase, string pUrl, HttpMethod pMetodo)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(pUrlBase);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + pToken.access_token);


                    HttpRequestMessage request = new HttpRequestMessage(pMetodo, pUrl);
                    request.Content = new StringContent("",
                                                        Encoding.UTF8,
                                                        "application/json");//CONTENT-TYPE header

                    // make the request
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        switch (response.StatusCode.ToString())
                        {
                            case "NotFound":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "BadRequest":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "400":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "401":
                                {
                                    throw new Exception("Usuário não autorizado a acessar o sistema.");
                                }
                            case "403":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            case "404":
                                {
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                                }
                            default:
                                {
                                    throw new Exception("Erro ao solicitar autorização para acesso.");
                                }
                        }
                    }

                    // parse the response and return the data.
                    string jsonString = response.Content.ReadAsStringAsync().Result;

                    //if (!string.IsNullOrWhiteSpace(jsonString))
                    //    id_Retorno = jsonString;
                    return jsonString;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UfTO> RetornaUfs()
        {
            List<UfTO> lstUfs = new List<UfTO>();

            lstUfs.Add(new UfTO("AC","AC"));
            lstUfs.Add(new UfTO("AL","AL"));
            lstUfs.Add(new UfTO("AM","AM"));
            lstUfs.Add(new UfTO("AP","AP"));
            lstUfs.Add(new UfTO("BA","BA"));
            lstUfs.Add(new UfTO("CE","CE"));
            lstUfs.Add(new UfTO("DF","DF"));
            lstUfs.Add(new UfTO("ES","ES"));
            lstUfs.Add(new UfTO("GO","GO"));
            lstUfs.Add(new UfTO("MA","MA"));
            lstUfs.Add(new UfTO("MG","MG"));
            lstUfs.Add(new UfTO("MS","MS"));
            lstUfs.Add(new UfTO("MT","MT"));
            lstUfs.Add(new UfTO("PA","PA"));
            lstUfs.Add(new UfTO("PB","PB"));
            lstUfs.Add(new UfTO("PE","PE"));
            lstUfs.Add(new UfTO("PI","PI"));
            lstUfs.Add(new UfTO("PR","PR"));
            lstUfs.Add(new UfTO("RJ","RJ"));
            lstUfs.Add(new UfTO("RN","RN"));
            lstUfs.Add(new UfTO("RO","RO"));
            lstUfs.Add(new UfTO("RR","RR"));
            lstUfs.Add(new UfTO("RS","RS"));
            lstUfs.Add(new UfTO("SC","SC"));
            lstUfs.Add(new UfTO("SE","SE"));
            lstUfs.Add(new UfTO("SP","SP"));
            lstUfs.Add(new UfTO("TO","TO"));

            return lstUfs;
        }

        public List<TipoAcessoTO> RetornaTipoAcesso()
        {
            List<TipoAcessoTO> lstTipoAcessos = new List<TipoAcessoTO>();

            lstTipoAcessos.Add(new TipoAcessoTO("E", "Executar"));
            lstTipoAcessos.Add(new TipoAcessoTO("V", "Visualizar"));

            return lstTipoAcessos;
        }

        public List<TipoRespostaTO> ListarTipoResposta()
        {
            List<TipoRespostaTO> lstTipoAcessos = new List<TipoRespostaTO>();

            lstTipoAcessos.Add(new TipoRespostaTO("V", "Positiva"));
            lstTipoAcessos.Add(new TipoRespostaTO("F", "Negativa"));

            return lstTipoAcessos;
        }

        public List<TipoPermissaoFinalizacaoTotalTO> ListarTipoPermissaoFinalizacaoTotal()
        {
            List<TipoPermissaoFinalizacaoTotalTO> lstTipoAcessos = new List<TipoPermissaoFinalizacaoTotalTO>();

            lstTipoAcessos.Add(new TipoPermissaoFinalizacaoTotalTO("S", "Sim"));
            lstTipoAcessos.Add(new TipoPermissaoFinalizacaoTotalTO("N", "Não"));

            return lstTipoAcessos;
        }

        public List<TipoPerfilTO> RetornaTipoPerfil()
        {
            List<TipoPerfilTO> lstTipoPerfis = new List<TipoPerfilTO>();

            lstTipoPerfis.Add(new TipoPerfilTO("O", "Operacional"));
            lstTipoPerfis.Add(new TipoPerfilTO("G", "Gestão"));

            return lstTipoPerfis;
        }

        public List<TipoAtividadeAcomodacaoTO> ListarTipoAtividadeAcomodacao()
        {
            List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/TipoAtividadeAcomodacao/items";
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaTipoAtividadeAcomodacao = Listar<TipoAtividadeAcomodacaoTO>(accessToken, _urlBase, url);
                ListaTipoAtividadeAcomodacao = InMemoryCache.GetOrSet<List<TipoAtividadeAcomodacaoTO>>("ListaTipoAtividadeAcomodacaoTO", () => Listar<TipoAtividadeAcomodacaoTO>(accessToken, _urlBase, url), _TempoCache);
                //string imagem = "";
                string cor = "";
                foreach(TipoAtividadeAcomodacaoTO item in ListaTipoAtividadeAcomodacao)
                {

                    //1   MENSAGEIRO
                    //2   HIGIENIZAÇÃO
                    //3   MANUTENÇÃO
                    //4   ENGENHARIA
                    //5   RESERVA
                    //6   INTERDIÇÃO
                    //7   ENFERMAGEM
                    //8   CAMAREIRA
                    //9   CENTRAL DE LEITOS
                    switch (item.id_TipoAtividadeAcomodacao){
                        case "1":
                        {
                                //imagem = "images/icon_mensageiro.png";
                                cor = "#23CC72";
                                break;
                        }
                        case "2":
                            {
                                //imagem = "images/icon_higienizacao.png";
                                cor = "#FF4747";
                                break;
                            }
                        case "3":
                            {
                                //imagem = "images/icon_manutencao.png";
                                cor = "#FFE95D";
                                break;
                            }
                        case "4":
                            {
                                //imagem = "images/icon_engenharia_clinica.png";
                                cor = "#FFE95D";
                                break;
                            }
                        case "5":
                            {
                                //imagem = "images/icon_reserva.png";
                                cor = "#FFE95D";
                                break;
                            }
                        case "6":
                            {
                                //imagem = "images/icon_restrict.png";
                                cor = "#FF4747";
                                break;
                            }
                        case "7":
                            {
                                //imagem = "images/icon_enfermagem.png";
                                cor = "#FF4747";
                                break;
                            }
                        case "8":
                            {
                                //imagem = "images/icon_camareira.png";
                                cor = "#FF4747";
                                break;
                            }
                        case "9":
                            {
                                //imagem = "images/icon_central_leitos.png";
                                cor = "#FF4747";
                                break;
                            }
                        default:
                            {
                                //imagem = "";
                                cor = "";
                                break;
                            }
                    }
                    //item.imagem = imagem;
                    item.cor_TipoAtividadeAcomodacao = cor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaTipoAtividadeAcomodacao;
        }

        public List<TipoAcaoAcomodacaoTO> ListarTipoAcaoAcomodacao()
        {
            List<TipoAcaoAcomodacaoTO> ListaTipoAcaoAcomodacao = new List<TipoAcaoAcomodacaoTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/TipoAcaoAcomodacao/items";
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaTipoAcaoAcomodacao = Listar<TipoAcaoAcomodacaoTO>(accessToken, _urlBase, url);
                ListaTipoAcaoAcomodacao = InMemoryCache.GetOrSet<List<TipoAcaoAcomodacaoTO>>("ListaTipoAcaoAcomodacaoTO", () => Listar<TipoAcaoAcomodacaoTO>(accessToken, _urlBase, url), _TempoCache);

                //string imagem = "";
                string cor = "";
                int ordem=0;
                string nomeStatusLabel = "";
                foreach (TipoAcaoAcomodacaoTO item in ListaTipoAcaoAcomodacao)
                {

                    //1   ACEITAR                   ACEITE              //Aceita
                    //2   INICIAR                   CHECK-IN            //Iniciada
                    //3   FINALIZAR TOTALMENTE      CHECK - OUT         //Finalizada Total
                    //4   FINALIZAR PARCIALMENTE    SEMI CHECK-OUT      //Finalizada Parcial
                    //5   SOLICITAR                 SOLICITADO          //Solicitada
                    switch (item.id_TipoAcaoAcomodacao)
                    {
                        case "1":
                            {
                                //imagem = "images/icon_aceite.png";
                                cor = "#23CCB7";
                                ordem = 2;
                                nomeStatusLabel = "Aceita";
                                break;
                            }
                        case "2":
                            {
                                //imagem = "images/icon_iniciado.png";
                                cor = "#23CCB7";
                                ordem = 3;
                                nomeStatusLabel = "Iniciada";
                                break;
                            }
                        case "3":
                            {
                                //imagem = "images/icon_finalizado_total.png";
                                cor = "#23CC72";
                                ordem = 5;
                                nomeStatusLabel = "Finalizada Total";
                                break;
                            }
                        case "4":
                            {
                                //imagem = "images/icon_finalizado_parcial.png";
                                cor = "#FF4747";
                                ordem = 4;
                                nomeStatusLabel = "Finalizada Parcial";
                                break;
                            }
                        case "5":
                            {
                                //imagem = "images/icon_solicitado.png";
                                cor = "#23CCB7";
                                ordem = 1;
                                nomeStatusLabel = "Solicitada";
                                break;
                            }
                        default:
                            {
                                //imagem = "";
                                cor = "";
                                ordem = 0;
                                break;
                            }
                    }
                    //item.imagem = imagem;
                    item.cor_TipoAcaoAcomodacao = cor;
                    item.ordem = ordem;
                    item.nome_Status_Label = nomeStatusLabel;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaTipoAcaoAcomodacao;
        }

        public List<TipoSituacaoAcomodacaoTO> ListarTipoSituacaoAcomodacao()
        {
            List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/TipoSituacaoAcomodacao/items";
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaTipoSituacaoAcomodacao = Listar<TipoSituacaoAcomodacaoTO>(accessToken, _urlBase, url);
                ListaTipoSituacaoAcomodacao = InMemoryCache.GetOrSet<List<TipoSituacaoAcomodacaoTO>>("TipoSituacaoAcomodacaoTO", () => Listar<TipoSituacaoAcomodacaoTO>(accessToken, _urlBase, url), _TempoCache);

                //string imagem = "";
                string cor = "";
                foreach (TipoSituacaoAcomodacaoTO item in ListaTipoSituacaoAcomodacao)
                {

                    //1	ALTA MEDICA
                    //2 RESERVA
                    //3 OCUPADO
                    //4 LIBERADO
                    //5 INTERDITADO
                    //6 VAGO
                    switch (item.id_TipoSituacaoAcomodacao)
                    {
                        case "1":
                            {
                                //imagem = "images/icon_man.png";
                                cor = "#007ddb";
                                break;
                            }
                        case "2":
                            {
                                //imagem = "images/icon_ticket.png";
                                cor = "#ed008c";
                                break;
                            }
                        case "3":
                            {
                                //imagem = "images/icon_bed.png";
                                cor = "#ff1f30";
                                break;
                            }
                        case "4":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#f9a61a";
                                break;
                            }
                        case "5":
                            {
                                //imagem = "images/icon_restrict.png";
                                cor = "#ffde00";
                                break;
                            }
                        case "6":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#8cc63e";
                                break;
                            }
                        case "7":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#4f7484";
                                break;
                            }
                        case "8":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#00adef";
                                break;
                            }
                        case "9":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#27edf8";
                                break;
                            }
                        case "10":
                            {
                                //imagem = "images/icon_key.png";
                                cor = "#924bf9";
                                break;
                            }
                        default:
                            {
                                //imagem = "";
                                cor = "";
                                break;
                            }
                    }
                    //item.imagem = imagem;
                    item.cor_TipoSituacaoAcomodacao = cor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaTipoSituacaoAcomodacao; 
        }

        public List<ItemCheckListTO> ListarItemCheckList()
        {
            List<ItemCheckListTO> ListaItemCheckList = new List<ItemCheckListTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/ItemChecklist/items";
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaItemCheckList = Listar<ItemCheckListTO>(accessToken, _urlBase, url);
                ListaItemCheckList = InMemoryCache.GetOrSet<List<ItemCheckListTO>>("ItemCheckListTO", () => Listar<ItemCheckListTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaItemCheckList;
        }

        public List<CheckListTO> ListarCheckList()
        {
            List<CheckListTO> ListaCheckList = new List<CheckListTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/CheckList/items";
                ///int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                ListaCheckList = Listar<CheckListTO>(accessToken, _urlBase, url);
                ///ListaCheckList = InMemoryCache.GetOrSet<List<ItemCheckListTO>>("ItemCheckListTO", () => Listar<ItemCheckListTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaCheckList;
        }

        public List<EmpresaTO> ListarEmpresas()
        {
            List<EmpresaTO> ListaEmpresa = new List<EmpresaTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Empresa/items");

                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaEmpresa = Listar<EmpresaTO>(accessToken, _urlBase, url);
                ListaEmpresa = InMemoryCache.GetOrSet<List<EmpresaTO>>("EmpresaTO", () => Listar<EmpresaTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaEmpresa;
        }

        public List<TipoAcomodacaoTO> ListarTipoAcomodacao(string id_empresa)
        {
            List<TipoAcomodacaoTO> ListaTipoAcomodacao = new List<TipoAcomodacaoTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/TipoAcomodacao/items/empresa/{0}", id_empresa);

                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaTipoAcomodacao = Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url);
                ListaTipoAcomodacao = InMemoryCache.GetOrSet<List<TipoAcomodacaoTO>>("ItemTipoAcomodacaoTOCheckListTO", () => Listar<TipoAcomodacaoTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaTipoAcomodacao;
        }
        
        public List<SetorTO> ListarSetores(string id_empresa)
        {
            List<SetorTO> ListaSetor = new List<SetorTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];

                string url = string.Format("api/Setor/items/empresa/{0}", id_empresa);

                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                //ListaSetor = Listar<SetorTO>(accessToken, _urlBase, url);
                ListaSetor = InMemoryCache.GetOrSet<List<SetorTO>>("SetorTO", () => Listar<SetorTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaSetor;
        }

        public List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(string id_empresa)
        {
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao/items/empresa/{0}", id_empresa);

                //int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);

                ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = Listar<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>(accessToken, _urlBase, url);
                //ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = InMemoryCache.GetOrSet<List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>>("ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO", () => Listar<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>(accessToken, _urlBase, url), _TempoCache);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta;
        }

        public List<SLAEmpresaTO> ListarSLAsEmpresa()
        {
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);
                string url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);
                ListaSLAEmpresa = InMemoryCache.GetOrSet<List<SLAEmpresaTO>>("ListaSLAEmpresaTO", () => Listar<SLAEmpresaTO>(accessToken, _urlBase, url), _TempoCache);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaSLAEmpresa;
        }

        public List<AcessoEmpresaPerfilTsTaConsultaTO> ListarAcessoEmpresaPerfil(string pPerfil)
        {
            List<AcessoEmpresaPerfilTsTaConsultaTO> ListaAcessoEmpresaPerfil = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenAdministrativo"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloAdministrativo"];
                int _TempoCache = int.Parse(ConfigurationManager.AppSettings["tempoCache"]);
                string url = string.Format("api/AcessoEmpresaPerfilTSTA/items/empresa/{0}/perfil/{1}", Session["id_EmpresaSel"].ToString(), pPerfil);

                //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);
                ListaAcessoEmpresaPerfil = InMemoryCache.GetOrSet<List<AcessoEmpresaPerfilTsTaConsultaTO>>("ListaAcessoEmpresaPerfil" + pPerfil, () => Listar<AcessoEmpresaPerfilTsTaConsultaTO>(accessToken, _urlBase, url), _TempoCache);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListaAcessoEmpresaPerfil;
        }

        public string DefinirTipoAcessoPorSituacaoAtividade(string pPerfil, string pTipoSituacao, string pTipoAtividade)
        {
            List<AcessoEmpresaPerfilTsTaConsultaTO> ObjLstAcessoEmpresaPerfilTsTaConsultaTO = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            AcessoEmpresaPerfilTsTaConsultaTO ObjAcessoEmpresaPerfilTsTaConsultaTO = new AcessoEmpresaPerfilTsTaConsultaTO();
            string TipoAcessoRetorno = "V";
            try
            {
                ObjLstAcessoEmpresaPerfilTsTaConsultaTO = ListarAcessoEmpresaPerfil(pPerfil);
                if (ObjLstAcessoEmpresaPerfilTsTaConsultaTO != null && ObjLstAcessoEmpresaPerfilTsTaConsultaTO.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(pTipoSituacao) && pTipoSituacao != "")
                    {
                        if (!string.IsNullOrWhiteSpace(pTipoAtividade) && pTipoAtividade != "")
                        {
                            ObjAcessoEmpresaPerfilTsTaConsultaTO = ObjLstAcessoEmpresaPerfilTsTaConsultaTO.Where(m => m.id_TipoAtividadeAcomodacao == pTipoAtividade && m.id_TipoSituacaoAcomodacao == pTipoSituacao && m.cod_Tipo=="E").FirstOrDefault();
                            if (ObjAcessoEmpresaPerfilTsTaConsultaTO!=null)
                            {
                                TipoAcessoRetorno = ObjAcessoEmpresaPerfilTsTaConsultaTO.cod_Tipo;
                            }
                        }
                        else
                        {
                            ObjAcessoEmpresaPerfilTsTaConsultaTO = ObjLstAcessoEmpresaPerfilTsTaConsultaTO.Where(m =>  m.id_TipoSituacaoAcomodacao == pTipoSituacao && m.cod_Tipo == "E").FirstOrDefault();
                            if (ObjAcessoEmpresaPerfilTsTaConsultaTO != null)
                            {
                                TipoAcessoRetorno = ObjAcessoEmpresaPerfilTsTaConsultaTO.cod_Tipo;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao determinar o tipo de acesso. Erro: " + ex.Message);
            }
            return TipoAcessoRetorno;
        }

        #region "Definição de Imagens"

        public string DefineImagemSLA(decimal pTempoExecucao, string pTipoAtividadeAcomodacao, string pTipoSituacaoAcomodacao, string pTipoAcomodacao, ref string pSLA, ref string pCorSLA)
        {
            string strImagem = "";
            
            decimal decPercentSLA = 0;
            int pPercentualAtencao = int.Parse(ConfigurationManager.AppSettings["percentualAtencaoSLA"]);
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            List<SLAEmpresaTO> objSLAEmpresa = new List<SLAEmpresaTO>();
            decimal tempoSLAMedio = 0;
            decimal tempoSLATotal = 0;
            decimal tempoSLAItens = 0;
            try
            {
                ListaSLAEmpresa = ListarSLAsEmpresa();

                if (pTipoAtividadeAcomodacao!="0")
                    objSLAEmpresa = ListaSLAEmpresa.Where(m => m.id_TipoAtividadeAcomodacao == pTipoAtividadeAcomodacao && m.id_TipoSituacaoAcomodacao == pTipoSituacaoAcomodacao && m.id_TipoAcomodacao==pTipoAcomodacao).ToList();
                else
                    objSLAEmpresa = ListaSLAEmpresa.Where(m => m.id_TipoSituacaoAcomodacao == pTipoSituacaoAcomodacao && m.id_TipoAcomodacao == pTipoAcomodacao).ToList();

                foreach(SLAEmpresaTO item in objSLAEmpresa)
                {
                    tempoSLATotal += decimal.Parse(item.tempo_Minutos);
                    tempoSLAItens += 1;
                }
                if (tempoSLAItens > 0)
                {
                    tempoSLAMedio = tempoSLATotal / tempoSLAItens;
                }
                else
                {
                    tempoSLAMedio = tempoSLATotal / 10000;
                }

                if (objSLAEmpresa == null)
                {
                    throw new Exception("Não foi possível determinar o SLA");
                }

                pSLA = tempoSLAMedio.ToString(); //objSLAEmpresa.tempo_Minutos;

                decimal pTempoSLA = decimal.Parse(pSLA);

                decPercentSLA = pTempoSLA - ((pTempoSLA / pPercentualAtencao));

                if (pTempoExecucao > pTempoSLA)
                {
                    strImagem = "images/clock_red.png";
                    pCorSLA = "#FF4747"; // vermelho
                }
                else 
                if(pTempoExecucao<decPercentSLA)
                { 
                    strImagem = "images/clock_green.png";
                    if (pCorSLA != "#FF4747" && pCorSLA != "#FFE95D") // se não for a vermelho ou amarelo atribui a verde
                        pCorSLA = "#00cc00"; //verde
                }
                else 
                {
                    strImagem = "images/clock_yelow.png";
                    if (pCorSLA != "#FF4747") // se não for a vermelho atribui a amarela
                        pCorSLA = "#FFE95D"; //amarelo
                }




                //strImagem = "images/clock_green.png";
                //if (pCorSLA != "#FF4747" && pCorSLA != "#FFE95D") // se não for a vermelho ou amarelo atribui a verde
                //    pCorSLA = "#00cc00"; //verde
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemSLABodyDashBoard(string pFora, ref string pCorSLA)
        {
            string strImagem = "";
           
            try
            {
                if (pFora=="S")
                {
                    strImagem = "images/clock_red.png";
                    pCorSLA = "#FF4747"; // vermelho
                }else
                {
                    if (pFora == "N")
                    {
                        strImagem = "images/clock_green.png";
                        pCorSLA = "#00cc00"; //verde
                    }
                    else
                        {
                            strImagem = "images/clock_gray.png";
                            pCorSLA = "#cccccc"; //cinza
                        }
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemSLACalculado(decimal pTempoExecucao, string pSLA, ref string pCorSLA)
        {
            string strImagem = "";

            decimal decPercentSLA = 0;
            int pPercentualAtencao = int.Parse(ConfigurationManager.AppSettings["percentualAtencaoSLA"]);
            //List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            //List<SLAEmpresaTO> objSLAEmpresa = new List<SLAEmpresaTO>();

            decimal pTempoSLA = 0;
            try
            {
                //ListaSLAEmpresa = ListarSLAsEmpresa();


                if (!string.IsNullOrWhiteSpace(pSLA))
                {
                    pTempoSLA = decimal.Parse(pSLA);
                 
                }
                else
                {
                    pTempoSLA = 10000;
                }
                        

                decPercentSLA = pTempoSLA - ((pTempoSLA / pPercentualAtencao));

                if (pTempoExecucao > pTempoSLA)
                {
                    strImagem = "images/clock_red.png";
                    pCorSLA = "#FF4747"; // vermelho
                }
                else
                if (pTempoExecucao < decPercentSLA)
                {
                    strImagem = "images/clock_green.png";
                    if (pCorSLA != "#FF4747" && pCorSLA != "#FFE95D") // se não for a vermelho ou amarelo atribui a verde
                        pCorSLA = "#00cc00"; //verde
                }
                else
                if  (pTempoExecucao != 0)
                {
                    strImagem = "images/clock_yelow.png";
                    if (pCorSLA != "#FF4747") // se não for a vermelho atribui a amarela
                        pCorSLA = "#FFE95D"; //amarelo
                }
                else
                {
                    strImagem = "images/clock_gray.png";
                    pCorSLA = "#808080"; //cinza
                }




                //strImagem = "images/clock_green.png";
                //if (pCorSLA != "#FF4747" && pCorSLA != "#FFE95D") // se não for a vermelho ou amarelo atribui a verde
                //    pCorSLA = "#00cc00"; //verde
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemPrioridade(string pPrioridade)
        {
            string strImagem = "";
            try
            {
                

                if (pPrioridade == "S")
                {
                    strImagem = "images/icon_bell.png";
                }
                else
                {
                    strImagem = "images/icon_bell_off.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemIsolamento(string pIsolamento)
        {
            string strImagem = "";
            try
            {


                if (pIsolamento == "S")
                {
                    strImagem = "images/icon_alert.png";
                }
                else
                {
                    strImagem = "images/icon_alert_off.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemLimpezaPlus(string pLimpezaPlus)
        {
            string strImagem = "";
            try
            {


                if (pLimpezaPlus == "S")
                {
                    strImagem = "images/icon_plus.png";
                }
                else
                {
                    strImagem = "images/icon_plus_off.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string DefineImagemPendenciaFinanceira(string pPendenciaFinanceira)
        {
            string strImagem = "";
            try
            {

                if (pPendenciaFinanceira == "S")
                {
                    strImagem = "images/icon_pendencia_financeira_on.png";
                }
                else
                {
                    strImagem = "images/icon_pendencia_financeira_off.png";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strImagem;
        }

        public string FormataTempoExecucaoEmHoras(string pTempo)
        {
            string pRetorno = "";
            int totHoras = 0;
            int totMinutos = 0;
            int totGeral = 0;
            if (!string.IsNullOrWhiteSpace(pTempo))
            {
                if (int.TryParse(pTempo, out totGeral)==true)
                {
                    totHoras = totGeral / 60;
                    totMinutos = totGeral % 60;
                    pRetorno = totHoras.ToString().PadLeft(2, '0') + ":" + totMinutos.ToString().PadLeft(2, '0');
                }
                else
                {
                    pRetorno = "00:00";
                }
            }
            else
            {
                pRetorno = "00:00";
            }
            return pRetorno;

        }

        #endregion

        #region "Acessos"

        public List<LogAcessoTO> ListaAcessos()
        {
            List<LogAcessoTO> LstLogAcesso = new List<LogAcessoTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenOperacional"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloOperacional"];

                string url = string.Format("api/Gestao/items/logacesso");

                LstLogAcesso = Listar<LogAcessoTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LstLogAcesso;
        }

        #endregion

    }



}