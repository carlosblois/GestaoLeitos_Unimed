using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class AtividadeItem
    {
        /// <summary>
        /// Identificador da Atividade (Identity)
        /// </summary>
        [Required]
        public int Id_AtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador da situação
        /// </summary>
        [Required]
        public int Id_SituacaoAcomodacao { get; set; }

        /// <summary>
        /// Identificador do tipo de situação
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }

        /// <summary>
        /// Identificador do tipo de atividade
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Data de incicio da atividade
        /// </summary>
        [Required]
        public DateTime dt_InicioAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Data fim da atividade
        /// </summary>
        public DateTime? dt_FimAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador do usuario solicitante
        /// </summary>
        [Required]
        public int Id_UsuarioSolicitante { get; set; }

        /// <summary>
        /// Indicador se é uma atividade prioritária N/S
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string Cod_Prioritario { get; set; }

        /// <summary>
        /// Indicador se é uma atividade plus (se aplica a higienização) N/S
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string Cod_Plus { get; set; }

        [JsonIgnore]
        public SituacaoItem SituacaoItem { get; set; }

        public AtividadeItem() { }

        [JsonIgnore]
        public List<AcaoItem> AcaoItems { get; set; }

        [JsonIgnore]
        public List<CheckRespostaAtividadeItem> CheckRespostaAtividadeItems { get; set; }

        [JsonIgnore]
        public List<MensagemItem> MensagemItems { get; set; }

    }
}
