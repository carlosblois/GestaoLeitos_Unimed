
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class TipoAcaoAcomodacaoItem
    {

        /// <summary>
        /// Identificador do Tipo de Ação de Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_TipoAcaoAcomodacao { get; set; }

        /// <summary>
        /// Nome do Tipo de Ação de Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_TipoAcaoAcomodacao { get; set; }

        /// <summary>
        /// Nome do status do Tipo de Ação de Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_Status { get; set; }

        /// <summary>
        /// Caminho da imagem
        /// </summary>
        [StringLength(100)]
        public string imagem { get; set; }


        public TipoAcaoAcomodacaoItem() { }

    }
}
