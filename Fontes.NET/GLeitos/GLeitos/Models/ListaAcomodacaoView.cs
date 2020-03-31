using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaAcomodacaoView: BaseView
    {

        public List<AcomodacaoTO> ListaAcomodacoes { get; set; }

        public ListaAcomodacaoView()
        {
            this.ListaAcomodacoes = new List<AcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}