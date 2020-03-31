using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaPerfilView: BaseView
    {

        public List<PerfilTO> ListaPerfis { get; set; }

        public ListaPerfilView()
        {
            this.ListaPerfis = new List<PerfilTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}