
using System;
using System.ComponentModel.DataAnnotations;

namespace Configuracao.API.Model
{
    public class FluxoAutomaticoCheckItem
    {
        /// <summary>
        /// Identificador do checklist
        /// </summary>
        [Required]
        public int Id_Checklist { get; set; }

        /// <summary>
        /// Identificador do Tipo de Situação de Acomodação 
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }


        /// <summary>
        /// Identificador do item de checklist
        /// </summary>
        [Required]
        public int Id_ItemChecklist { get; set; }


        /// <summary>
        /// Identificador do Tipo de Atividade de Acomodação 
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Codigo da resposta, indica V /F para indicar que o fluxo ocorrerá para resposta V ou F do item de checklist
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("V|F")]
        public string Cod_Resposta { get; set; }

        /// <summary>
        /// Codigo (S/N) que indica se pode se aplicar mesmo na FINALIZAR TOTAL
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string Cod_PermiteTotal { get; set; }

        public FluxoAutomaticoCheckItem() { }

    }
}
