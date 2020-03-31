using Newtonsoft.Json;
using Empresa.API.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Empresa.FunctionalTests
{
    public class EmpresaScenarios
       : EmpresaScenariosBase
    {


        [Fact]
        public async Task Post_IncluirSetor()
        {
            using (var server = CreateServer())
            {
                //var response = await server.CreateClient()
                //    .DeleteAsync(Del.ExcluirSetor(Guid.Parse("4C8D935A-E673-47D3-A8F5-9A3934DF72E7")));

                for (int i = 0; i < 1000; i++)
                {
                    EmpresaItem empresaItem = new EmpresaItem();
                    empresaItem.Nome_Empresa = "AAA" + i.ToString();

                    var content = new StringContent(JsonConvert.SerializeObject(empresaItem), System.Text.Encoding.UTF8, "application/json");
                    var responsex = await server.CreateClient()
                    .PostAsync(Post.IncluirEmpresa(), content);
                }

            }
        }


    }
}
