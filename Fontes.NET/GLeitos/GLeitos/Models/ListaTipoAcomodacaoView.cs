using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaTipoAcomodacaoView: BaseView
    {

        public List<TipoAcomodacaoTO> ListaTipoAcomodacoes { get; set; }

        public ListaTipoAcomodacaoView()
        {
            this.ListaTipoAcomodacoes = new List<TipoAcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}