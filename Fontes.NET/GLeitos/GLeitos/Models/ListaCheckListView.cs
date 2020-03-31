using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaCheckListView: BaseView
    {

        public List<CheckListTO> ListaCheckList { get; set; }

        public ListaCheckListView()
        {
            this.ListaCheckList = new List<CheckListTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}