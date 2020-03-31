using Newtonsoft.Json;
using Usuario.API.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;

namespace Usuario.FunctionalTests
{
    public class UsuarioScenarios
       : UsuarioScenariosBase
    {
        [Fact]
        public async Task Inc_SaveUsuario()
        {
 
                //using (var server = CreateServer())
                //{
                    for (int i = 0; i < 1000; i++)
                    {
                        //var myTestClient = server.CreateClient();
                        IEnumerable<string> Ltoken = await GetAsync();
                        using (var myTestClient = new HttpClient())
                        {
                            UsuarioItem usuarioItem = new UsuarioItem();
                            usuarioItem.nome_Usuario = "TESTE"+ i.ToString(); //+ Guid.NewGuid().ToString();
                            var content = new StringContent(JsonConvert.SerializeObject(usuarioItem), System.Text.Encoding.UTF8, "application/json");
                            myTestClient.SetBearerToken(((string[])Ltoken)[0]);
                            var response = await myTestClient.PostAsync(Post.SaveUsuario(), content);
                        }
                    }
                //}
        }

        public async Task<IEnumerable<string>> GetAsync()
        {

            using(var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                requestData.Add(new KeyValuePair<string, string>("client_id", "clientusuario"));
                requestData.Add(new KeyValuePair<string, string>("client_secret", "secret"));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", "http://localhost:5000"));
                requestData.Add(new KeyValuePair<string, string>("response_type", "token"));
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", "usuario_api"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("http://localhost:5001/connect/token", requestBody);
                var response = request.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<AccessToken>(response.Result.ToString());
                return new string[] { token.access_token.ToString(), token.expires_in.ToString() };
            }
        }

        public class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
        }


    }
}
