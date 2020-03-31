using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class SetorView : BaseView
    {

        public SetorTO Setor { get; set; }
        public List<EmpresaTO> ListaEmpresas { get; set; } 

        public SetorView()
        {
            this.ListaEmpresas = new List<EmpresaTO>();
            this.Setor = new SetorTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}