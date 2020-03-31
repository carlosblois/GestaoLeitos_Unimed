using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class ListaSlaTO
    {
        
        [Required(ErrorMessage = "O tipo de situação é obrigatório.")]
        public string id_TipoSituacaoAcomodacao { get; set; }
        [Required(ErrorMessage = "O tipo de atividade é obrigatório.")]
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoAcaoAcomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }          
        public string id_Empresa { get; set; }
        public string cod_enabled { get; set; }

        public ListaSlaTO()
        {
           
            this.id_Empresa = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.id_TipoAcaoAcomodacao = "";           
            this.id_TipoAcomodacao = "";
            this.cod_enabled = "S";
        }
    }
}
