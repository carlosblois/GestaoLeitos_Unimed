using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  SLASituacaoView : BaseView
    {

        public SlaSituacaoTO SlaSituacao { get; set; }
        public List<SLASituacaoConsultaTO> ListaSLASituacao { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }
        public List<TipoAcomodacaoTO> ListaTipoAcomodacao { get; set; }
        public string dropDisabled { get; set; } 


        public SLASituacaoView()
        {
            this.SlaSituacao = new SlaSituacaoTO();
            this.ListaSLASituacao = new List<SLASituacaoConsultaTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAcomodacao = new List<TipoAcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
            this.dropDisabled = "";
        }

    }
    
}