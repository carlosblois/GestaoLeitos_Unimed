
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class TipoSituacaoTipoAtividadeAcomodacaoItem
    {

        /// <summary>
        /// Identificador do Tipo de Situação de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }


        public TipoSituacaoTipoAtividadeAcomodacaoItem() { }

    }
}
