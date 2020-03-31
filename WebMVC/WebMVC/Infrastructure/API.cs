using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure
{
    public class API
    {
        public static class Produto
        {
            public static string GetProdutos(string baseUri) => $"{baseUri}/produto";
            public static string CreateOrUpdateProduto(string baseUri) => $"{baseUri}/produto";            
        }
    }
}
