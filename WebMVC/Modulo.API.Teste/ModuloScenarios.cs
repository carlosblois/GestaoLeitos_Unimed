using Newtonsoft.Json;
using Modulo.API.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;

namespace Modulo.FunctionalTests
{
    public class ModuloScenarios
       : ModuloScenariosBase
    {
        [Fact]
        public async Task Inc_SaveModulo()
        {
            //using (var server = CreateServer())
            //{
            IEnumerable<string> Ltoken = await GetAsync();
            for (int i = 6000; i < 7000; i++)
                    {
                        //var myTestClient = server.CreateClient();
                        
                        using (var myTestClient = new HttpClient())
                        {
                            ModuloItem moduloItem = new ModuloItem();
                            moduloItem.Nome_Modulo = "TESTE"+ i.ToString(); //+ Guid.NewGuid().ToString();
                            var content = new StringContent(JsonConvert.SerializeObject(moduloItem), System.Text.Encoding.UTF8, "application/json");
                            myTestClient.SetBearerToken(((string[])Ltoken)[0]);
                            var response = await myTestClient.PostAsync(Post.SaveModulo(), content);
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
                requestData.Add(new KeyValuePair<string, string>("client_id", "clientmodulo"));
                requestData.Add(new KeyValuePair<string, string>("client_secret", "secret"));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", "http://localhost:5000"));
                requestData.Add(new KeyValuePair<string, string>("response_type", "token"));
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", "modulo_api"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("https://identityhubapi.azurewebsites.net/connect/token", requestBody);
                //var request = await client.PostAsync("http://localhost:5001/connect/token", requestBody);
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
