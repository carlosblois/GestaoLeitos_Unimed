
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem
    {

        /// <summary>
        /// Identificador (identity)
        /// </summary>
        [Required]
        public int Id_CheckTSTAT{ get; set; }

        /// <summary>
        /// Identificador do Checklist
        /// </summary>
        [Required]
        public int Id_Checklist { get; set; }

        /// <summary>
        /// Identificador do Tipo de Situacao de Acomodacao.
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodacao.
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador do Tipo de Acomodacao.
        /// </summary>
        [Required]
        public int Id_TipoAcomodacao { get; set; }

        /// <summary>
        /// Identificador da Empresa.
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }


        public ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem() { }

    }
}
