using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MyAPI.Controllers
{
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
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
                requestData.Add(new KeyValuePair<string, string>("RequestedScopes", "dame_api"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request =  await client.PostAsync("http://localhost:5001/connect/token", requestBody);
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