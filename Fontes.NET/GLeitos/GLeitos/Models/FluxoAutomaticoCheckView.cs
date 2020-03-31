using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  FluxoAutomaticoCheckView : BaseView
    {

        public FluxoAutomaticoCheckTO FluxoAutomaticoCheck { get; set; }
        public List<FluxoAutomaticoCheckConsultaTO> ListaFluxoAutomaticoCheck { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao { get; set; }
        public List<CheckListTO> ListaCheckList { get; set; }
        public List<ChecklistItemChecklistConsultaTO> ListaItemCheckList { get; set; }
        public List<TipoPermissaoFinalizacaoTotalTO> ListaPermissaoFinalizacaoTotal { get; set; }
        public List<TipoRespostaTO> ListaTipoResposta { get; set; }
        public string dropDisabled { get; set; } 
        public string url_listaCheckList { get; set; }


        public FluxoAutomaticoCheckView()
        {
            this.FluxoAutomaticoCheck = new FluxoAutomaticoCheckTO();
            this.ListaFluxoAutomaticoCheck = new List<FluxoAutomaticoCheckConsultaTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            this.ListaCheckList = new List<CheckListTO>();
            this.ListaItemCheckList = new List<ChecklistItemChecklistConsultaTO>();
            this.ListaPermissaoFinalizacaoTotal = new List<TipoPermissaoFinalizacaoTotalTO>();
            this.ListaTipoResposta = new List<TipoRespostaTO>();
            this.erro = "";
            this.mensagem = "";
            this.dropDisabled = "";
        }

    }
    
}