using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class AcomodacaoView : BaseView
    {

        public AcomodacaoTO Acomodacao { get; set; }
        public List<EmpresaTO> ListaEmpresas { get; set; }
        public List<TipoAcomodacaoTO> ListaTiposAcomodacoes { get; set; }
        public List<SetorTO> ListaSetores { get; set; }

        public AcomodacaoView()
        {
            this.ListaEmpresas = new List<EmpresaTO>();
            this.ListaTiposAcomodacoes = new List<TipoAcomodacaoTO>();
            this.ListaSetores = new List<SetorTO>();
            this.Acomodacao = new AcomodacaoTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}