
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class AcomodacaoItem
    {

        /// <summary>
        /// Identificador da Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_Acomodacao { get; set; }

        /// <summary>
        /// Nome da Acomodação
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_Acomodacao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoAcomodacao { get; set; }

        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }


        /// <summary>
        /// Identificador do Setor.
        /// </summary>
        [Required]
        public int Id_Setor { get; set; }


        /// <summary>
        /// Codigo externo da Acomodação, com proposito de integração. (Max 5)
        /// </summary>
        [Required]
        [StringLength(5)]
        public string CodExterno_Acomodacao { get; set; }

        /// <summary>
        /// Codigo do isolamento da Acomodação. (Max 1) S/N
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string Cod_Isolamento { get; set; }

        public AcomodacaoItem() { }

    }
}
