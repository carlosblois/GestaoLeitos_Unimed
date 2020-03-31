using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class CaracteristicaAcomodacaoTO
    {
     
        public string id_CaracteristicaAcomodacao { get;set;}
        [Required(ErrorMessage = "O nome da característica da acomodação é obrigatório.")]
        public string nome_CaracteristicaAcomodacao { get; set; }

        public CaracteristicaAcomodacaoTO()
        {
            this.id_CaracteristicaAcomodacao = "";
            this.nome_CaracteristicaAcomodacao = "";

        }
    }
}
