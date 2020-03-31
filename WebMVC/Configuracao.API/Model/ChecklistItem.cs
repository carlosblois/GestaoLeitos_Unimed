
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class ChecklistItem
    {

        /// <summary>
        /// Identificador do Checklist (Identity)
        /// </summary>
        [Required]
        public int Id_Checklist { get; set; }

        /// <summary>
        /// Nome do Checklist (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_Checklist { get; set; }


        public ChecklistItem() { }

    }
}
