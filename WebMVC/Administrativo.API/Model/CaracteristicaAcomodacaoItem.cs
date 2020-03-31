
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class CaracteristicaAcomodacaoItem
    {
        /// <summary>
        /// Identificador de Caracteristica da Acomodação (Identity)
        /// </summary>
        [Required]
        public int id_CaracteristicaAcomodacao { get; set; }

        /// <summary>
        /// Nome da Caracteristica da Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string nome_CaracteristicaAcomodacao { get; set; }

        public CaracteristicaAcomodacaoItem() { }

    }
}
