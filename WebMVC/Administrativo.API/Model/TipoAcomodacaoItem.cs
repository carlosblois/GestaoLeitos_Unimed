
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class TipoAcomodacaoItem
    {
        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }

        /// <summary>
        /// Identificador do Tipo de Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_TipoAcomodacao { get; set; }

        /// <summary>
        /// Nome do Tipo de Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_TipoAcomodacao { get; set; }

        /// <summary>
        /// Codigo externo do Tipo de Acomodação, com proposito de integração. (Max 2)
        /// </summary>
        [Required]
        [StringLength(5)]
        public string CodExterno_TipoAcomodacao { get; set; }

        /// <summary>
        /// Identificador da Caracteristica da Acomodação
        /// </summary>
        [Required]
        public int Id_CaracteristicaAcomodacao { get; set; }

        public TipoAcomodacaoItem() { }

    }
}
