
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class TipoAtividadeAcomodacaoItem
    {

        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Nome do Tipo de Atividade de Acomodação (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nome_TipoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Ordem de apresentacao
        /// </summary>
        public int ordem { get; set; }

        /// <summary>
        /// Caminho da imagem
        /// </summary>
        [StringLength(100)]
        public string imagem { get; set; }

        /// <summary>
        /// Caminho da imagem
        /// </summary>
        [StringLength(1)]
        public string qrcode { get; set; }


        public TipoAtividadeAcomodacaoItem() { }

    }
}
