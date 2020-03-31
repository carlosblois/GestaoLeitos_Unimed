using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  FluxoAutomaticoView : BaseView
    {

        public FluxoAutomaticoTO FluxoAutomatico { get; set; }
        public List<FluxoAutomaticoConsultaTO> ListaFluxoAutomatico { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao { get; set; }
        public List<TipoAcaoAcomodacaoTO> ListaTipoAcaoAcomodacao { get; set; }
        public string dropDisabled { get; set; } 


        public FluxoAutomaticoView()
        {
            this.FluxoAutomatico = new FluxoAutomaticoTO();
            this.ListaFluxoAutomatico = new List<FluxoAutomaticoConsultaTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            this.ListaTipoAcaoAcomodacao = new List<TipoAcaoAcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
            this.dropDisabled = "";
        }

    }
    
}