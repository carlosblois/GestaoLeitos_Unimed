using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class ChecklistItemChecklistConsultaTO
    {
        public string id_Checklist { get;set;}
        public string nome_Checklist { get; set; }
        public string id_ItemChecklist { get; set; }
        public string nome_ItemChecklist { get; set; }


        public ChecklistItemChecklistConsultaTO()
        {
            this.id_Checklist = "";
            this.nome_Checklist = "";
            this.id_ItemChecklist = "";
            this.nome_ItemChecklist = "";

        }
    }
}
