using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class CheckRespostaAtividadeItem
    {

        /// <summary>
        /// Identificador da atividade
        /// </summary>
        [Required]
        public int Id_AtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador da Resposta
        /// </summary>
        [Required]
        public int Id_RespostasChecklist { get; set; }

        [JsonIgnore]
        public AtividadeItem AtividadeItem { get; set; }

        [JsonIgnore]
        public RespostasChecklistItem RespostasChecklistItem { get; set; }

        public CheckRespostaAtividadeItem() { }

    }
}
