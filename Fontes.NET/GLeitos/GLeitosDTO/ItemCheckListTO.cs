using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class ItemCheckListTO
    {
     
        public string id_ItemChecklist { get;set;}
        [Required(ErrorMessage = "O nome do item de check-list é obrigatório.")]
        public string nome_ItemChecklist { get; set; }

        public ItemCheckListTO()
        {
            this.id_ItemChecklist = "";
            this.nome_ItemChecklist = "";

        }
    }
}
