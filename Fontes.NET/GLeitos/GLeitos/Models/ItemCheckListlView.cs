using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  ItemCheckListView : BaseView
    {

        public ItemCheckListTO ItemCheckList { get; set; }
        public List<ItemCheckListTO> ListaItemCheckList { get; set; } 

        public ItemCheckListView()
        {
            this.ListaItemCheckList = new List<ItemCheckListTO>();
            this.ItemCheckList = new ItemCheckListTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}