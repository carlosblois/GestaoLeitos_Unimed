using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class FluxoAutomaticoCheckConsultaTO
    {
        public string id_Checklist { get; set; }
        public string nome_Checklist { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_ItemChecklist { get; set; }
        public string nome_ItemChecklist { get; set; }
        public string cod_Resposta { get; set; }
        public string cod_PermiteTotal { get; set; }
        public string nome_TipoAtividadeAcomodacaoDestino { get; set; }

        public FluxoAutomaticoCheckConsultaTO()
        {
            this.id_Checklist = "";
            this.nome_Checklist = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_ItemChecklist = "";
            this.nome_ItemChecklist = "";
            this.cod_Resposta = "";
            this.cod_PermiteTotal = "";
            this.nome_TipoAtividadeAcomodacaoDestino = "";

        }
    }
}
