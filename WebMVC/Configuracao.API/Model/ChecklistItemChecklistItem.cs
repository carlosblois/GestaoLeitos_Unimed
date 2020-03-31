
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class ChecklistItemChecklistItem
    {

        /// <summary>
        /// Identificador do Checklist
        /// </summary>
        [Required]
        public int Id_Checklist { get; set; }

        /// <summary>
        /// Identificador do Item de Checklist
        /// </summary>
        [Required]
        public int Id_ItemChecklist { get; set; }


        public ChecklistItemChecklistItem() { }

    }
}
