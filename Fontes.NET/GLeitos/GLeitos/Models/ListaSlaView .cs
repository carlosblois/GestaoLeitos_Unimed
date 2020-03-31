using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  ListaSLAView : BaseView
    {
        public ListaSlaTO ListaSla { get; set; }
        public List<SLAEmpresaTO> ListaSLAEmpresa { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao { get; set; }
        public List<TipoAcaoAcomodacaoTO> ListaTipoAcaoAcomodacao { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }
        public List<TipoAcomodacaoTO> ListaTipoAcomodacao { get; set; }
        public string dropDisabled { get; set; } 


        public ListaSLAView()
        {
            this.ListaSla = new ListaSlaTO();
            this.ListaSLAEmpresa = new List<SLAEmpresaTO>();
            this.ListaTipoAcaoAcomodacao = new List<TipoAcaoAcomodacaoTO>();
            this.ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAcomodacao = new List<TipoAcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
            this.dropDisabled = "";
        }

    }
    
}