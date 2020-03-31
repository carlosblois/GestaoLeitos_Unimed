
using Configuracao.API.TO;
using EventBus.Events;
using Newtonsoft.Json;
using Operacional.API.Infrastructure;
using Operacional.API.IntegrationEvents.Events;
using Operacional.API.Model;
using Operacional.API.TO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using static Operacional.API.Enum.ExpoEnum;
using Administrativo.API.TO;

namespace Operacional.API.Utilitarios
{
    public class csttk
    {
        internal string token;
        internal DateTime  expires;
        internal csttk(string token, DateTime expires)
        {
            this.token = token;
            this.expires = expires;
        }

    }

    public class LogCheck
    {
        internal int Log;
        internal TipoAtividade  TipoAtv;
        internal LogCheck(int log, TipoAtividade tipoAtv)
        {
            this.Log = log;
            this.TipoAtv = tipoAtv;
        }
    }

    public class LogAtividade
    {
        internal List<TipoAtividade> LstAt;
        internal List<LogCheck> LstCheck;

        public LogAtividade()
        {
        }
        public LogAtividade(List<TipoAtividade> lstAt, List<LogCheck> lstCheck)
        {
            this.LstAt = lstAt;
            this.LstCheck = lstCheck;
        }
    }

    public class Util
    {
        static Hashtable lsthub;
        static Util()
        {
            lsthub = new Hashtable();
            lsthub.Add("clientoperacional", new csttk("A",DateTime.Now));
            lsthub.Add("clientconfiguracao", new csttk("A", DateTime.Now));
            lsthub.Add("clientadministrativo", new csttk("A", DateTime.Now));
            lsthub.Add("clientusuario", new csttk("A", DateTime.Now));
            lsthub.Add("clientmodulo", new csttk("A", DateTime.Now));
        }

        internal static async Task<AcaoItem> IncluiAcao(string ConfiguracaoURL, string tokenURL,
                                                    int idEmpresa, int idTipoSituacaoAcomodacao,
                                                    int idTipoAtividadeAcomodacao,
                                                    int Id_AtividadeAcomodacao, int Id_TipoAcaoAcomodacao,
                                                    DateTime dt_InicioAcaoAtividade, DateTime? dt_FimAcaoAtividade,
                                                    string Id_UsuarioExecutor,int IdTipoAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarSLATO> SLAItem = await ConsultaSLAAsync(ConfiguracaoURL,
                                                                                    tokenURL,
                                                                                    idEmpresa,
                                                                                    idTipoSituacaoAcomodacao,
                                                                                    idTipoAtividadeAcomodacao,
                                                                                    Id_TipoAcaoAcomodacao,
                                                                                    IdTipoAcomodacao);

            AcaoItem acaoToSave = new AcaoItem();
            acaoToSave.Id_AtividadeAcomodacao = Id_AtividadeAcomodacao;
            acaoToSave.Id_TipoAcaoAcomodacao = Id_TipoAcaoAcomodacao;
            acaoToSave.dt_InicioAcaoAtividade = dt_InicioAcaoAtividade;
            acaoToSave.dt_FimAcaoAtividade = dt_FimAcaoAtividade;
            if ((SLAItem!=null) && (SLAItem.Count>0)) { acaoToSave.Id_SLA = SLAItem[0].Id_SLA; }
            acaoToSave.Id_UsuarioExecutor = Id_UsuarioExecutor;

            return acaoToSave;
        }

        internal static async Task<bool> ValidaRespostasChecklist(string ConfiguracaoURL, string tokenURL,
                                            int idTipoSituacaoAcomodacao,
                                            List<RespostasChecklistItem>lstRespostas)
        {

            List<Configuracao.API.TO.ConsultarChecklistDetalheTO> lstChecklistItem = await ConsultaCheckListDetalheAsync(ConfiguracaoURL,
                                                                                    tokenURL,
                                                                                    idTipoSituacaoAcomodacao);

            if (lstChecklistItem != null  )
            {
                foreach (ConsultarChecklistDetalheTO CheckListItem in lstChecklistItem)
                {
                    bool result = lstRespostas.Exists(x => (x.Id_ItemChecklist == CheckListItem.Id_ItemChecklist) && !string.IsNullOrEmpty(x.Valor));
                    if (!result)
                    { return false; }
                }
            }

            return true;
        }

        private static async Task<IEnumerable<string>> TokenAsync(string pathAPI, string pathToken, string client_id, string RequestedScopes)
        {
            csttk mykey = (csttk)lsthub[client_id];
            if (mykey.token == "A")
            {
                IEnumerable<string> token = await TokenAsyncReal(pathAPI, pathToken, client_id, RequestedScopes);
                return token;
            }
            else
            {
                if ((mykey.expires) <= DateTime.Now)
                {
                    IEnumerable<string> token = await TokenAsyncReal(pathAPI, pathToken, client_id, RequestedScopes);
                    return token;
                }
                else
                {
                    return new string[] { mykey.token, mykey.expires.ToString() };
                }
                
            }
        }

        private static async Task<IEnumerable<string>> TokenAsyncReal(string pathAPI, string pathToken, string client_id, string RequestedScopes)
        {
            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                requestData.Add(new KeyValuePair<string, string>("client_id", client_id));
                requestData.Add(new KeyValuePair<string, string>("client_secret", "secret"));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", pathAPI));
                requestData.Add(new KeyValuePair<string, string>("response_type", "token"));
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", RequestedScopes));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync(pathToken, requestBody);
                var response = request.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<AccessToken>(response.Result.ToString());

                lsthub[client_id] = new csttk(token.access_token.ToString(), DateTime.Now.AddSeconds( token.expires_in));

                return new string[] { token.access_token.ToString(), token.expires_in.ToString() };
            }
        }

        internal static async Task<List<Configuracao.API.TO.ConsultarSLASituacaoTO>> ConsultaSLASituacaoAsync(string ConfiguracaoURL, string tokenURL,
                                                                              int idEmpresa,
                                                                              int idTipoSituacaoAcomodacao,
                                                                              int idTipoAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarSLASituacaoTO> SLAItem = null;
            using (var client = new HttpClient())
            {
    
                string path = string.Format("{0}/api/SLASituacao/items/empresa/{1}/tiposituacao/{2}/tipoacomodacao/{3}", ConfiguracaoURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    SLAItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarSLASituacaoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }
            }
            return SLAItem;
        }

        internal static async Task<List<Administrativo.API.TO.ConsultarTipoAcomodacaoPorIdAcomodacaoTO>> ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(string ConfiguracaoURL, string tokenURL,
                                                                      int idEmpresa,
                                                                      int idAcomodacao)
        {

            List<Administrativo.API.TO.ConsultarTipoAcomodacaoPorIdAcomodacaoTO> TipoAcomodacaoItem = null;
            using (var client = new HttpClient())
            {

                string path = string.Format("{0}/api/TipoAcomodacao/items/empresa/{1}/acomodacao/{2}", ConfiguracaoURL, idEmpresa, idAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientadministrativo", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    TipoAcomodacaoItem = await response.Content.ReadAsAsync<List<Administrativo.API.TO.ConsultarTipoAcomodacaoPorIdAcomodacaoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }
            }
            return TipoAcomodacaoItem;
        }

        internal static async Task<List<Configuracao.API.TO.ConsultarSLAAtividadeTO>> ConsultaSLAAtividadeAsync(string ConfiguracaoURL, string tokenURL,
                                                                              int idEmpresa,
                                                                              int idTipoSituacaoAcomodacao,
                                                                              int idTipoAtividadeAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarSLAAtividadeTO> SLAItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/SLA/items/empresa/{1}/tiposituacao/{2}/tipoatividade/{3}/tipoacomodacao/{4}", ConfiguracaoURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idTipoAtividadeAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    SLAItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarSLAAtividadeTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }
            }
            return SLAItem;
        }

    
        internal static async Task< List<AtividadeItem>> TrataFluxoSituacao(string ConfiguracaoURL,string tokenURL,int idEmpresa, int idTipoSituacaoAcomodacao, int idTipoAcomodacao)
        {
            //TRATA FLUXO DE TRANSICAO DE SITUACAO

            List<AtividadeItem> lstAt = new List<AtividadeItem>();

            List<ConsultarFluxoAutomaticoSitTO> lsFluxoItem = await ConsultaFluxoSituacaoAsync(ConfiguracaoURL,
                                                                               tokenURL,
                                                                               idTipoSituacaoAcomodacao,
                                                                               idEmpresa);
            if (lsFluxoItem != null && lsFluxoItem.Count() > 0)
            {
                foreach (ConsultarFluxoAutomaticoSitTO Fluxo in lsFluxoItem)
                {
                    AtividadeItem atividadeToSave = new AtividadeItem();
                    atividadeToSave.Id_TipoSituacaoAcomodacao = Fluxo.Id_TipoSituacaoAcomodacaoDestino;
                    atividadeToSave.Id_TipoAtividadeAcomodacao = Fluxo.Id_TipoAtividadeAcomodacaoDestino;
                    atividadeToSave.Id_UsuarioSolicitante = 0;
                    atividadeToSave.dt_InicioAtividadeAcomodacao = DateTime.Now;
                    atividadeToSave.Cod_Prioritario = "N";
                    atividadeToSave.Cod_Plus  = "N";
                    

                    List<Configuracao.API.TO.ConsultarSLATO> SLAItem = await ConsultaSLAAsync(ConfiguracaoURL,
                                                                                tokenURL,
                                                                                idEmpresa,
                                                                                Fluxo.Id_TipoSituacaoAcomodacaoDestino,
                                                                                Fluxo.Id_TipoAtividadeAcomodacaoDestino,
                                                                                (int)TipoAcao.SOLICITAR,idTipoAcomodacao);

                    AcaoItem acaoToSave = new AcaoItem();
                    acaoToSave.Id_TipoAcaoAcomodacao = (int)TipoAcao.SOLICITAR;
                    acaoToSave.dt_InicioAcaoAtividade = DateTime.Now;
                    acaoToSave.Id_UsuarioExecutor = "0";
                    if (SLAItem!=null && SLAItem.Count>0) { acaoToSave.Id_SLA = SLAItem[0].Id_SLA; }

                    List<AcaoItem> lstAcao = new List<AcaoItem>();
                    atividadeToSave.AcaoItems = lstAcao;
                    atividadeToSave.AcaoItems.Add(acaoToSave);

                    lstAt.Add(atividadeToSave);
                }          
            }
            return lstAt;


        }
        internal static async Task<List<Configuracao.API.TO.ConsultarSLATO>> ConsultaSLAAsync(string ConfiguracaoURL, string tokenURL,
                                                                                      int idEmpresa,
                                                                                      int idTipoSituacaoAcomodacao,
                                                                                      int idTipoAtividadeAcomodacao,
                                                                                      int idTipoAcaoAcomodacao,
                                                                                      int idTipoAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarSLATO> SLAItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/SLA/items/empresa/{1}/tiposituacao/{2}/tipoatividade/{3}/tipoacao/{4}/tipoacomodacao/{5}", ConfiguracaoURL, idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idTipoAcaoAcomodacao, idTipoAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL,"clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    SLAItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarSLATO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }
            }
            return SLAItem;
        }

        private static async Task<List<Configuracao.API.TO.ConsultarChecklistDetalheTO>> ConsultaCheckListDetalheAsync(string ConfiguracaoURL, string tokenURL,
                                                                                                                        int idTipoSituacaoAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarChecklistDetalheTO> ChecklistItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/CheckList/items/tiposituacao/{1}", ConfiguracaoURL,  idTipoSituacaoAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    ChecklistItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarChecklistDetalheTO>>();
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    
                }
            }
            return ChecklistItem;
        }

        internal static async Task<List<Administrativo.API.TO.ConsultarAcomodacaoPorIdEmpresaCodExternoTO>> ConsultaAcomodacaoAsync(string AdministrativoURL, string tokenURL,
                                                                      int idEmpresa,
                                                                      string CodExterno)
        {

            List<Administrativo.API.TO.ConsultarAcomodacaoPorIdEmpresaCodExternoTO> AcomodacaoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/Acomodacao/items/empresa/{1}/codexterno/{2}", AdministrativoURL, idEmpresa, CodExterno);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(AdministrativoURL, tokenURL, "clientadministrativo", "administrativo_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    AcomodacaoItem = await response.Content.ReadAsAsync<List<Administrativo.API.TO.ConsultarAcomodacaoPorIdEmpresaCodExternoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return AcomodacaoItem;
        }

        internal static async Task<List<ConsultarAcessoAtividadeEmpresaPerfilTO>> ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(string AdministrativoURL, string tokenURL,
                                                                      int idEmpresa,
                                                                      int idTipoSituacaoAcomodacao,
                                                                      int idTipoAtividadeAcomodacao)
        {
            
            List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/AcessoEmpresaPerfilTSTA/items/empresa/{1}/tiposituacao/{2}/tiposituacao/{3}", AdministrativoURL, idEmpresa, idTipoSituacaoAcomodacao,idTipoAtividadeAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(AdministrativoURL, tokenURL, "clientadministrativo", "administrativo_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    Perfis = await response.Content.ReadAsAsync<List<ConsultarAcessoAtividadeEmpresaPerfilTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return Perfis;
        }

        internal static async Task<List<Administrativo.API.TO.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>> ConsultaAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoAsync(string AdministrativoURL, string tokenURL,
                                                                      int idAcomodacao)
        {

            List<Administrativo.API.TO.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO> AcomodacaoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/Acomodacao/items/acomodacaodetalhe/{1}", AdministrativoURL, idAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(AdministrativoURL, tokenURL, "clientadministrativo", "administrativo_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    AcomodacaoItem = await response.Content.ReadAsAsync<List<Administrativo.API.TO.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return AcomodacaoItem;
        }

        internal static async Task<List<Administrativo.API.TO.ConsultarAcomodacaoDetalhePorIdAcomodacaoTO>> ConsultaAcomodacaoDetalhePorIdAcomodacaoAsync(string AdministrativoURL, string tokenURL,
                                                                      int idAcomodacao)
        {

            List<Administrativo.API.TO.ConsultarAcomodacaoDetalhePorIdAcomodacaoTO> AcomodacaoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/Acomodacao/items/acomodacaodetalhe/{1}", AdministrativoURL, idAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(AdministrativoURL, tokenURL, "clientadministrativo", "administrativo_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    AcomodacaoItem = await response.Content.ReadAsAsync<List<Administrativo.API.TO.ConsultarAcomodacaoDetalhePorIdAcomodacaoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return AcomodacaoItem;
        }


        internal static async Task<List<Configuracao.API.TO.ConsultarFluxoAutomaticoSitTO>> ConsultaFluxoSituacaoAsync(string ConfiguracaoURL, string tokenURL,
                                                                      int idTipoSituacaoAcomodacao,
                                                                      int idEmpresa)
        {

            List<Configuracao.API.TO.ConsultarFluxoAutomaticoSitTO> FluxoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/FluxoAutomaticoSituacao/items/fluxoautomaticosituacao/tiposituacao/{1}/empresa/{2}", ConfiguracaoURL, idTipoSituacaoAcomodacao, idEmpresa);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    FluxoItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarFluxoAutomaticoSitTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }
                
            }
            return FluxoItem;
        }

        internal  static async Task<List<Configuracao.API.TO.ConsultarFluxoAutomaticoCheckTO>> ConsultaFluxoCheckAsync(string ConfiguracaoURL, string tokenURL,
                                                                      int idTipoSituacaoAcomodacao)
        {

            List<Configuracao.API.TO.ConsultarFluxoAutomaticoCheckTO> FluxoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/FluxoAutomaticoCheck/items/fluxoautomaticocheck/tiposituacao/{1}", ConfiguracaoURL, idTipoSituacaoAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    FluxoItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarFluxoAutomaticoCheckTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return FluxoItem;
        }

        internal static async Task<List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>> ConsultaTipoSituacaoTipoAtividadeAsync(string ConfiguracaoURL, string tokenURL,
                                                                              int idTipoSituacaoAcomodacao)
        {

            List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> TSTAItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/TipoSituacaoTipoAtividade/items/tiposituacao/{1}", ConfiguracaoURL, idTipoSituacaoAcomodacao);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    TSTAItem = await response.Content.ReadAsAsync<List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return TSTAItem;
        }


        private static async Task<List<Configuracao.API.TO.ConsultarFluxoAutomaticoTO>> ConsultaFluxoAsync(string ConfiguracaoURL, string tokenURL,
                                                                              int idTipoSituacaoAcomodacao,
                                                                              int idTipoAtividadeAcomodacao,
                                                                              int idTipoAcaoAcomodacao,
                                                                              int idEmpresa)
        {

            List<Configuracao.API.TO.ConsultarFluxoAutomaticoTO> FluxoItem = null;
            using (var client = new HttpClient())
            {
                string path = string.Format("{0}/api/FluxoAutomaticoAcao/items/fluxoautomatico/tiposituacao/{1}/tipoatividade/{2}/tipoacao/{3}/empresa/{4}", ConfiguracaoURL, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, idTipoAcaoAcomodacao,idEmpresa);

                //BUSCA TOKEN
                IEnumerable<string> token = await TokenAsync(ConfiguracaoURL, tokenURL, "clientconfiguracao", "configuracao_api");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ((string[])token)[0]);

                //CHAMA A API
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    FluxoItem = await response.Content.ReadAsAsync<List<Configuracao.API.TO.ConsultarFluxoAutomaticoTO>>();
                }
                else
                {
                    throw new Exception(response.ToString());
                }

            }
            return FluxoItem;
        }


        internal static async Task<List<ConsultarFluxoAutomaticoTO>> ConsultaValidaFluxoAutomatico(string ConfiguracaoURL, string tokenURL,
                                    int idTipoSituacaoAcomodacao,
                                    int idTipoAtividadeAcomodacao,
                                    int idTipoAcaoAcomodacao,
                                    int idEmpresa,
                                    List<AtividadeItem> lstEncaminhamento,
                                    List<AtividadeItem> lstAtividadesExistentes)
        {

            List<ConsultarFluxoAutomaticoTO> lsFluxoItem = await ConsultaFluxoAsync(ConfiguracaoURL,
                                                                                    tokenURL,
                                                                                    idTipoSituacaoAcomodacao, 
                                                                                    idTipoAtividadeAcomodacao, 
                                                                                    idTipoAcaoAcomodacao,
                                                                                    idEmpresa);
            if (lsFluxoItem != null)
            {
                //VERIFICA SE TEM COLISAO DO FLUXO AUTOMATICO COM OS ITENS ENCAMINHADOS
                List<ConsultarFluxoAutomaticoTO> cloneFluxoItem =new List<ConsultarFluxoAutomaticoTO>() ;
                cloneFluxoItem = lsFluxoItem.ToList();
                foreach (ConsultarFluxoAutomaticoTO FluxoItem in cloneFluxoItem)
                {
                    bool resultE = lstEncaminhamento.Exists(x => (x.Id_TipoSituacaoAcomodacao == FluxoItem.Id_TipoSituacaoAcomodacaoDestino)
                                                                 && x.Id_TipoAtividadeAcomodacao == FluxoItem.Id_TipoAtividadeAcomodacaoDestino);
                    if (resultE)
                    {
                        lsFluxoItem.Remove(FluxoItem);
                    }
                    else
                    {
                        bool resultA = lstAtividadesExistentes.Exists(x => (x.Id_TipoSituacaoAcomodacao == FluxoItem.Id_TipoSituacaoAcomodacaoDestino)
                                                     && x.Id_TipoAtividadeAcomodacao == FluxoItem.Id_TipoAtividadeAcomodacaoDestino
                                                     && x.dt_FimAtividadeAcomodacao == null);

                        if (resultA)
                        { lsFluxoItem.Remove(FluxoItem); }
                    }
                }
            }

            return lsFluxoItem;
        }

        internal static async Task<List<TipoAtividade>> TrataAssociacaoCheckAtividade(
                                                        List<TipoAtividade> lstEncaminhamento,
                                                        List<AtividadeItem> lstAtividadesExistentes)

            {
            List<TipoAtividade> cloneEncaminhamentoItem = new List<TipoAtividade>();
            return cloneEncaminhamentoItem;
        }

        internal static async Task<LogAtividade> ConsultaValidaEncaminhamento(
                                                                                    List<TipoAtividade> lstEncaminhamento,
                                                                                    List<AtividadeItem> lstAtividadesExistentes,
                                                                                    List<TipoAtividade> lstCheck,
                                                                                    List<LogCheck> lstLogCheck)
        { 


            if (lstEncaminhamento != null)
            {
                //VERIFICA SE TEM COLISAO COM ATIVIDADES
                List<TipoAtividade> cloneEncaminhamentoItem = new List<TipoAtividade>();
                cloneEncaminhamentoItem = lstEncaminhamento.ToList();
                foreach (TipoAtividade EncaminhamentoItem in cloneEncaminhamentoItem)
                {
 
                    bool resultA = lstAtividadesExistentes.Exists(x => (x.Id_TipoAtividadeAcomodacao == (int)EncaminhamentoItem
                                                                        && x.dt_FimAtividadeAcomodacao ==null));

                    if (resultA)
                    {
                        lstEncaminhamento.Remove(EncaminhamentoItem);
                        //var resultLog = lstLogCheck.Find(item => item.TipoAtv == EncaminhamentoItem);
                        //lstLogCheck.Remove(resultLog);
                    }
                    
                }
            }

            foreach (TipoAtividade checkItem in lstCheck)
            {
                var result = lstEncaminhamento.Exists(item => item == checkItem);
                if (result == false)
                {
                    bool resultA = lstAtividadesExistentes.Exists(x => (x.Id_TipoAtividadeAcomodacao == (int)checkItem
                                                    && x.dt_FimAtividadeAcomodacao == null));
                    if (resultA == false)
                    {
                        lstEncaminhamento.Add(checkItem);

                    }
                    else
                    {
                        var resultLog = lstLogCheck.Find(item => item.TipoAtv == checkItem);
                        lstLogCheck.Remove(resultLog);
                    }
                }
            }


            return new LogAtividade(lstEncaminhamento,lstLogCheck);
        }
        internal static async Task<EncaminharOut> Encaminhar(string ConfiguracaoURL, string AdministrativoURL, string tokenURL, int idUsuario, int idEmpresa, AtividadeItem atividadeToOrigemEncaminhar, List<TipoAtividade> LstTipoAtividadeToSave,int idTipoAcomodacao, int idAcomodacao)
        {


            List<IntegrationEvent> lstEvt = new List<IntegrationEvent>();
            List<AtividadeItem> lstItem = new List<AtividadeItem>();
            foreach (TipoAtividade idTipoAtividade in LstTipoAtividadeToSave)
            {
                AtividadeItem AtividadeToSave = new AtividadeItem();
                AtividadeToSave.Id_SituacaoAcomodacao = atividadeToOrigemEncaminhar.Id_SituacaoAcomodacao;
                AtividadeToSave.Id_TipoSituacaoAcomodacao = atividadeToOrigemEncaminhar.Id_TipoSituacaoAcomodacao;
                AtividadeToSave.Id_TipoAtividadeAcomodacao = (int)idTipoAtividade;
                AtividadeToSave.dt_InicioAtividadeAcomodacao = DateTime.Now;
                AtividadeToSave.dt_FimAtividadeAcomodacao = null;
                AtividadeToSave.Id_UsuarioSolicitante = idUsuario;
                AtividadeToSave.Cod_Plus = "N";
                AtividadeToSave.Cod_Prioritario = "N";

                List<AcaoItem> lstAcaoSave = new List<AcaoItem>();
                AcaoItem AcaoToSave = await IncluiAcao(ConfiguracaoURL, tokenURL, idEmpresa, atividadeToOrigemEncaminhar.Id_TipoSituacaoAcomodacao, (int)idTipoAtividade, 0, (int)TipoAcao.SOLICITAR, DateTime.Now, null, idUsuario.ToString(), idTipoAcomodacao);
                lstAcaoSave.Add(AcaoToSave);

                AtividadeToSave.AcaoItems = lstAcaoSave;

                lstItem.Add(AtividadeToSave);

                List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, AtividadeToSave.Id_TipoSituacaoAcomodacao, AtividadeToSave.Id_TipoAtividadeAcomodacao);

                //Create Integration Event to be published through the Event Bus
                var atividadeSaveEvent = new AtividadeSaveIE(AtividadeToSave.Id_AtividadeAcomodacao,
                                                                            AtividadeToSave.Id_SituacaoAcomodacao,
                                                                            AtividadeToSave.Id_TipoSituacaoAcomodacao,
                                                                            AtividadeToSave.Id_TipoAtividadeAcomodacao,
                                                                            AtividadeToSave.dt_InicioAtividadeAcomodacao,
                                                                            AtividadeToSave.dt_FimAtividadeAcomodacao,
                                                                            AtividadeToSave.Id_UsuarioSolicitante,
                                                                            lstAcaoSave,
                                                                            AtividadeToSave.Cod_Prioritario,
                                                                            AtividadeToSave.Cod_Plus,
                                                                            Perfis,
                                                                            idAcomodacao);

                lstEvt.Add(atividadeSaveEvent);

            }

            EncaminharOut objReturn = new EncaminharOut();
            objReturn.lstItem = lstItem;
            objReturn.lstEvt = lstEvt;

            return objReturn;

        }

        public class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
        }

        public class resultLiberacao
        {
            public string Result { get; set; }
            public string Message { get; set; }
        }

        public class EncaminharOut
        {
            public List<IntegrationEvent> lstEvt { get; set; }
            public List<AtividadeItem> lstItem { get; set; }

            public EncaminharOut()
            {
                lstEvt = new List<IntegrationEvent>();
                lstItem = new List<AtividadeItem>();
            }
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static AtividadeSaveIE CriaEventoAtividade(AtividadeItem atividadeToSave, List<ConsultarAcessoAtividadeEmpresaPerfilTO> perfis, int idAcomodacao)
        {

            //Create Integration Event to be published through the Event Bus
            var atividadeSaveEvent = new AtividadeSaveIE(atividadeToSave.Id_AtividadeAcomodacao,
                                                                        atividadeToSave.Id_SituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoSituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoAtividadeAcomodacao,
                                                                        atividadeToSave.dt_InicioAtividadeAcomodacao,
                                                                        atividadeToSave.dt_FimAtividadeAcomodacao,
                                                                        atividadeToSave.Id_UsuarioSolicitante,
                                                                        atividadeToSave.AcaoItems,
                                                                        atividadeToSave.Cod_Prioritario,
                                                                        atividadeToSave.Cod_Plus,
                                                                        perfis, 
                                                                        idAcomodacao);

            return atividadeSaveEvent;
        }
        internal static async Task<string> LiberarAcomodacao(string urlApiLiberacao , string rotaApiLiberacao, string userApiLiberacao, string passwordApiLiberacao, string hostApiLiberacao, string codExterno_Acomodacao)
        {
            try
            {
                urlApiLiberacao = string.Format(urlApiLiberacao, rotaApiLiberacao, codExterno_Acomodacao);
                string PassWordhash;

                using (MD5 md5Hash = MD5.Create())
                {
                    PassWordhash = GetMd5Hash(md5Hash, passwordApiLiberacao + DateTime.Now.ToString("ddMMyyyyHHmm"));
                }

                using (var client = new HttpClient())
                {
                    //Define Headers
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userApiLiberacao,PassWordhash);

                    //Prepare Request Body
                    List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                    requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                    requestData.Add(new KeyValuePair<string, string>("Domain", hostApiLiberacao));
                    requestData.Add(new KeyValuePair<string, string>("response_type", "resultLiberacao"));
                    
                    FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);
                
                    var request = await client.PostAsync(urlApiLiberacao, requestBody);
                    var response = request.Content.ReadAsStringAsync();
                    var resultLiberacao = JsonConvert.DeserializeObject<resultLiberacao>(response.Result.ToString());
                    if (resultLiberacao.Result == "200")
                        return "OK";
                    else
                        return resultLiberacao.Message;
                }
            }
            catch
            {
                return "Erro";
            }

        }


    }
}
