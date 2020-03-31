using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  CheckListView : BaseView
    {

        public CheckListTO CheckList { get; set; }
        public List<ItemCheckListTO> ListaItemCheckList { get; set; }
        public List<ItemCheckListTO> ListaItemCheckListSel { get; set; }
        public string ItensCheckListSel { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }
        public List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao { get; set; }
        public List<TipoAcomodacaoTO> ListaTipoAcomodacao { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }


        public CheckListView()
        {
            this.ListaItemCheckList = new List<ItemCheckListTO>();
            this.CheckList = new CheckListTO();
            this.ListaItemCheckListSel = new List<ItemCheckListTO>();
            this.ItensCheckListSel = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.id_TipoAcomodacao = "";
            this.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            this.ListaTipoAcomodacao = new List<TipoAcomodacaoTO>();
            this.ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}