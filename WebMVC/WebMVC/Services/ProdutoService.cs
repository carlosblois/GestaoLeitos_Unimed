using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMVC.Model;

namespace WebMVC.Services
{
    public class ProdutoService:IProdutoService 
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProdutoService> _logger;

        private readonly string _remoteServiceBaseUrl;

        public ProdutoService(IOptions<AppSettings> settings, HttpClient httpClient, ILogger<ProdutoService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.ProdutoUrl}/api/v1/m/produtos/";
        }

        public async Task CreateOrUpdateProduto(ProdutoDTO produto)
        {

            var uri = API.Produto.CreateOrUpdateProduto(_remoteServiceBaseUrl);
            var produtoContent = new StringContent(JsonConvert.SerializeObject(produto), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, produtoContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<SelectListItem>> GetProdutos()
        {
            var uri = API.Produto.GetProdutos(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var items = new List<SelectListItem>();


            var produtos = JArray.Parse(responseString);

            foreach (var produto in produtos.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = produto.Value<string>("id"),
                    Text = produto.Value<string>("name")
                });
            }

            return items;
        }

    }

}
