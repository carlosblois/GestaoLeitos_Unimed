
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class FluxoAutomaticoAcaoItem
    {

        /// <summary>
        /// Identificador do Tipo de Ação de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoAcaoAcomodacaoOrigem { get; set; }

        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacaoOrigem { get; set; }


        /// <summary>
        /// Identificador do Tipo de Situação de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacaoOrigem { get; set; }

        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacaoDestino { get; set; }


        /// <summary>
        /// Identificador do Tipo de Situação de Acomodação
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacaoDestino { get; set; }


        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }

        public FluxoAutomaticoAcaoItem() { }

    }
}
