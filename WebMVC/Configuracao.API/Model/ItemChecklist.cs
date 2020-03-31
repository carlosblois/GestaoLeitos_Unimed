
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class ItemChecklistItem
    {

        /// <summary>
        /// Identificador do Item de Checklist (Identity)
        /// </summary>
        [Required]
        public int Id_ItemChecklist { get; set; }

        /// <summary>
        /// Nome do Item de Checklist (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_ItemChecklist { get; set; }


        public ItemChecklistItem() { }

    }
}
