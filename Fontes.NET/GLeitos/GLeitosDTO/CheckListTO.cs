using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class CheckListTO
    {
     
        public string id_Checklist { get;set;}
        [Required(ErrorMessage = "O nome do CheckList é obrigatório.")]
        public string nome_Checklist { get; set; }

        public CheckListTO()
        {
            this.id_Checklist = "";
            this.nome_Checklist = "";

        }
    }
}
