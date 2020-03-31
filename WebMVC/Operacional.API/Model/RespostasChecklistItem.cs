using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class RespostasChecklistItem
    {
      
        /// <summary>
        /// Identificador das Respostas (Identity)
        /// </summary>
        [Required]
        public int Id_RespostasChecklist { get; set; }

        /// <summary>
        /// Identificador da Atividade das Respostas
        /// </summary>
        [Required]
        public int Id_AtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador do da composicao das respostas Tipos Situacao / Tipo Atividade / Tipo Acomodacao
        /// </summary>
        [Required]
        public int Id_CheckTSTAT { get; set; }

        /// <summary>
        /// Identificador do checklist
        /// </summary>
        [Required]
        public int Id_Checklist { get; set; }

        /// <summary>
        /// Identificador do item do checklist
        /// </summary>
        [Required]
        public int Id_ItemChecklist { get; set; }


        /// <summary>
        /// Valor preenchido como a resposta do checklist
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Valor { get; set; }

        [JsonIgnore]
        public List<CheckRespostaAtividadeItem> CheckRespostaAtividadeItems { get; set; }


        public RespostasChecklistItem() { }

    }
}
