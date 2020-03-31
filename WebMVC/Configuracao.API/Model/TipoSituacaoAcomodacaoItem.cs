
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class TipoSituacaoAcomodacaoItem
    {

        /// <summary>
        /// Identificador do Tipo de Situação de Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }

        /// <summary>
        /// Nome do Tipo de situação de Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_TipoSituacaoAcomodacao { get; set; }

        /// <summary>
        /// Ordem de apresentacao
        /// </summary>
        public int ordem { get; set; }

        /// <summary>
        /// Caminho da imagem
        /// </summary>
        [StringLength(100)]
        public string imagem { get; set; }


        public TipoSituacaoAcomodacaoItem() { }

    }
}
