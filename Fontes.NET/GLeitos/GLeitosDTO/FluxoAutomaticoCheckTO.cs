using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class FluxoAutomaticoCheckTO
    {
       
        [Required(ErrorMessage = "O CheckList é obrigatório.")]
        public string id_Checklist { get; set; }
        [Required(ErrorMessage = "A Situação é obrigatória.")]
        public string id_TipoSituacaoAcomodacao { get; set; }
        [Required(ErrorMessage = "O Item de CheckList é obrigatório.")]
        public string id_ItemChecklist { get; set; }
        [Required(ErrorMessage = "A Atividade é obrigatória.")]
        public string id_TipoAtividadeAcomodacao { get; set; }
        [Required(ErrorMessage = "O Tipo de Resposta é obrigatório.")]
        public string cod_Resposta { get; set; }
        [Required(ErrorMessage = "A permissão para finalização total é obrigatória.")]
        public string cod_PermiteTotal { get; set; }
        
        public FluxoAutomaticoCheckTO()
        {
            this.id_Checklist = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_ItemChecklist = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.cod_Resposta = "";
            this.cod_PermiteTotal = "";
        }
    }
}
