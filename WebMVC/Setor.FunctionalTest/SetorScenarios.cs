using Newtonsoft.Json;
using Setor.API.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Setor.FunctionalTests
{
    public class SetorScenarios
       : SetorScenariosBase
    {

        [Fact]
        public async Task Get_ConsultarPorId_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.ConsultarPorId(Guid.Parse("2F676B7B-516B-479A-8590-09EE4569672A")));
            }
        }

        [Fact]
        public async Task Get_ConsultarPorId_and_response_not_found_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.ConsultarPorId(Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730")));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_ConsultarPorIdEmpresa_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.ConsultarPorIdEmpresa(Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A")));

                var responseBody = await response.Content.ReadAsStringAsync();  
                List<Setor.API.Model.SetorItem> setorItems = JsonConvert.DeserializeObject<List<Setor.API.Model.SetorItem>>(responseBody);

                Assert.Equal(6, setorItems.Count);
            }
        }

        [Fact]
        public async Task Get_ConsultarPorIdEmpresa_and_response_not_found_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.ConsultarPorIdEmpresa(Guid.Parse("A898A236-C34F-46F8-93AA-636EC4E90D20")));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Del_ExcluirSetor()
        {
            using (var server = CreateServer())
            {
                //var response = await server.CreateClient()
                //    .DeleteAsync(Del.ExcluirSetor(Guid.Parse("4C8D935A-E673-47D3-A8F5-9A3934DF72E7")));

                for (int i = 0; i < 1000; i++)
                { 
                    SetorItem setorItem = new SetorItem();
                    setorItem.id_Empresa = Guid.Parse("B8AC98BA-50B3-4947-B042-09DD09A76C21");
                    setorItem.id_Setor = Guid.NewGuid();
                    setorItem.nome_Setor = "AAA" + i.ToString();
                    
                    var content = new StringContent(JsonConvert.SerializeObject(setorItem), System.Text.Encoding.UTF8, "application/json");
                    var responsex = await server.CreateClient()
                    .PostAsync(Post.IncluirSetor(), content);
                }

            }
        }

        
    }
}
