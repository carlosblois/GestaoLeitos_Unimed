
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Model;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IProdutoService
    {
        Task<IEnumerable<SelectListItem>> GetProdutos();
        Task CreateOrUpdateProduto(ProdutoDTO produto);
    }
}
