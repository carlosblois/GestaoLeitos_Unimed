using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaEmpresaView: BaseView
    {

        public List<EmpresaTO> ListaEmpresas { get; set; }

        public ListaEmpresaView()
        {
            this.ListaEmpresas = new List<EmpresaTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}