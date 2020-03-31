using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebMVC.Model;
using WebMVC.Services;
using WebMVC.ViewModels;
using WebMVC.ViewModels.ProdutoViewModel;

namespace WebMVC.Controllers
{
    public class ProdutoController: Controller
    {

        private readonly IProdutoService _produtoService;
        private readonly AppSettings _settings;

        public ProdutoController(IProdutoService produtoService, IOptionsSnapshot<AppSettings> settings)
        {
            _produtoService = produtoService;
            _settings = settings.Value;
        }

        public async Task<IActionResult> ChamaAPIGetProdutos()
        {
                    var produtos = await _produtoService.GetProdutos();

                    //Redirect to historic list.
                    return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChamaAPICreateOrUpdateProdutos(ProdutoViewModel model)  
        {
            
            if (ModelState.IsValid)
            {

                List<ProdutoItem> list = model.ProdutosItems.ToList();
                foreach (ProdutoItem value in list)
                {
                    var produto = new ProdutoDTO();                  
                    produto.Id = value.Id;
                    produto.Name = value.Name;
                    produto.PictureUri  = value.PictureUri ;
                    produto.Price  = value.Price ;
                    produto.Description = value.Description;

                    await _produtoService.CreateOrUpdateProduto(produto);
                };
                //return RedirectToAction("");
            }

            return View();
        }
    }
}
