using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class EmpresaView: BaseView
    {

        public EmpresaTO Empresa { get; set; }
        public List<UfTO> ListaUfs { get; set; } 

        public EmpresaView()
        {
            this.ListaUfs = new List<UfTO>();
            this.Empresa = new EmpresaTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}