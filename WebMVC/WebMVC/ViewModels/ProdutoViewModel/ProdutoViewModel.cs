using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.ViewModels.Pagination;

namespace WebMVC.ViewModels.ProdutoViewModel
{

    public class ProdutoViewModel
    {
        public IEnumerable<ProdutoItem> ProdutosItems { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
    
}
