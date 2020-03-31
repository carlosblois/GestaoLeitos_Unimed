using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemTO
    {
        public string id_CheckTSTAT { get; set; }
        public string id_Checklist { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }
        public string id_Empresa { get; set; }

        public ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemTO()
        {
            this.id_CheckTSTAT = "";
            this.id_Checklist = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.id_TipoAcomodacao = "";
            this.id_Empresa = "";

        }
    }

    public class ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO
    {
        public string id_CheckTSTAT { get; set; }
        public string id_Checklist { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }
        public string nome_TipoAcomodacao { get; set; }
        public string id_Empresa { get; set; }

        public ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO()
        {
            this.id_CheckTSTAT = "";
            this.id_Checklist = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_TipoAcomodacao = "";
            this.nome_TipoAcomodacao = "";
            this.id_Empresa = "";

        }
    }

}
