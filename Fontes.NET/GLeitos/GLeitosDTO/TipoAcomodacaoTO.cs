using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class TipoAcomodacaoTO
    {

        public string id_TipoAcomodacao { get; set; }
        [Required(ErrorMessage = "O código da empresa do tipo de acomodação é obrigatório.")]
        public string id_Empresa { get;set;}
        [Required(ErrorMessage = "O nome do tipo de acomodação é obrigatório.")]
        public string nome_TipoAcomodacao { get; set; }
        [Required(ErrorMessage = "O Código externo do tipo de acomodação é obrigatório.")]
        public string codExterno_TipoAcomodacao { get; set; }
        [Required(ErrorMessage = "O identificador da característica da acomodação é obrigatório.")]
        public string id_CaracteristicaAcomodacao { get; set; }
        public string nome_CaracteristicaAcomodacao { get; set; }

        public TipoAcomodacaoTO()
        {
            this.id_TipoAcomodacao = "";
            this.id_Empresa = "";
            this.nome_TipoAcomodacao = "";
            this.codExterno_TipoAcomodacao = "";
            this.id_CaracteristicaAcomodacao = "";
            this.nome_CaracteristicaAcomodacao = "";

        }
    }
}
