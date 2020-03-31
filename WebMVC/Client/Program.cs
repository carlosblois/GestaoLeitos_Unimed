using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
        }
        private static async Task<string> GetToken()
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
                requestData.Add(new KeyValuePair<string, string>("client_id", "client"));
                requestData.Add(new KeyValuePair<string, string>("client_secret", "secret"));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", "http://localhost:59820"));
                requestData.Add(new KeyValuePair<string, string>("response_type", "token"));
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", "modulo_api"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("http://localhost:5001/connect/token", requestBody);
                var response = request.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<AccessToken>(response.Result.ToString());

                client.SetBearerToken(token.access_token);
                var responseA = await client.GetAsync("http://localhost:59820/api/Home");
           
                if (!responseA.IsSuccessStatusCode)
                {
                    Console.WriteLine(responseA.StatusCode);
                }
                else
                {
                    var content = await responseA.Content.ReadAsStringAsync();
                    Console.WriteLine(JArray.Parse(content));
                }


            }



            string clientId = "dameswaggerui";
            string clientSecret = "SEGREDO";
            string credentials = String.Format("{0}", clientId);

            //var tokenClient = new TokenClient("http://localhost:5001/connect/token/", "dameswaggerui","SEGREDO");
            // var tokenResponse = await tokenClient.RequestClientCredentialsAsync("dame_api");

            // var token = await HttpContext.GetTokenAsync("access_token");

            var disco = await DiscoveryClient.GetAsync("http://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                //return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("dame_api");

            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                requestData.Add(new KeyValuePair<string, string>("client_id", "dameswaggerui"));
                requestData.Add(new KeyValuePair<string, string>("redirect_uri", "http://localhost:59820"));
                requestData.Add(new KeyValuePair<string, string>("response_type", "token"));
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", "dame_api"));
                requestData.Add(new KeyValuePair<string, string>("Username", "bob"));
                requestData.Add(new KeyValuePair<string, string>("Password", "password"));




                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request =  await client.PostAsync("http://localhost:5001/connect/token", requestBody);
                var response =  request.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<AccessToken>(response.ToString());
                return "A";
            }
        }
        private static async Task MainAsync()
        {

            Console.WriteLine("Spotify API");
            var token = GetToken().Result;
            //Console.WriteLine(String.Format("Access Token: {0}", token.access_token));
        


        // discover endpoints from metadata
        var disco = await DiscoveryClient.GetAsync("http://localhost:5001");
            

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5000/api/Modulo");
            //var response = await client.GetAsync("https://localhost:44322/api/Modulo");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            //// request token
            //var tokenClientx = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            //var tokenResponsex = await tokenClientx.RequestResourceOwnerPasswordAsync("alice", "password", "api1");
            //if (tokenResponsex.IsError)
            //{
            //    Console.WriteLine(tokenResponsex.Error);
            //    return;
            //}
            //Console.WriteLine(tokenResponsex.Json);
            //Console.WriteLine("\n\n");

            //// call api
            //var clientx = new HttpClient();
            //clientx.SetBearerToken(tokenResponsex.AccessToken);

            //var responsex = await clientx.GetAsync("http://localhost:5001/identity");
            //if (!responsex.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(responsex.StatusCode);
            //}
            //else
            //{
            //    var contentx = await responsex.Content.ReadAsStringAsync();
            //    Console.WriteLine(JArray.Parse(contentx));
            //}


        }
    }
}
