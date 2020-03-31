
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class SLAItem
    {
        /// <summary>
        /// Identificador do SLA (Identity)
        /// </summary>
        [Required]
        public int Id_SLA { get; set; }

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

        /// <summary>
        /// Identificador do Tipo de Ação de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoAcaoAcomodacao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoAcomodacao { get; set; }

        /// <summary>
        /// Tempo em minutos referente ao SLA
        /// </summary>
        [Required]
        public int Tempo_Minutos { get; set; }

        /// <summary>
        /// Versao do SLA
        /// </summary>
        [Required]
        public int Versao_SLA { get; set; }

        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }

        /// <summary>
        /// Codigo para habilitar e desabilitar SLA N/S
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string cod_enabled { get; set; }


        public SLAItem() { }

    }
}
