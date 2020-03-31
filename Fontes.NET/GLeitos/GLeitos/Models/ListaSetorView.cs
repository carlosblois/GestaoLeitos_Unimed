using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaSetorView: BaseView
    {

        public List<SetorTO> ListaSetores { get; set; }

        public ListaSetorView()
        {
            this.ListaSetores = new List<SetorTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}