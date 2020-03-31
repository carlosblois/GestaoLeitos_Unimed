using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class ChecklistItemChecklistTO
    {
        public string id_Checklist { get;set;}
        public string id_ItemChecklist { get; set; }

        public ChecklistItemChecklistTO()
        {
            this.id_Checklist = "";
            this.id_ItemChecklist = "";
        }
    }
}
