using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class LogAcessoTO
    {
     
        public string id_TipoAtividadeAcomodacao { get;set;}
        public string totalUsuarios { get; set; }
        
        public LogAcessoTO()
        {

            this.id_TipoAtividadeAcomodacao = "";
            this.totalUsuarios = "";

        }
    }
}
