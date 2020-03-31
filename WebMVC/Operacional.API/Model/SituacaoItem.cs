using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class SituacaoItem
    {

        /// <summary>
        /// Identificador da situação
        /// </summary>
        [Required]
        public int Id_SituacaoAcomodacao { get; set; }
        /// <summary>
        /// Identificador da acomodação
        /// </summary>
        [Required]
        public int Id_Acomodacao { get; set; }
        /// <summary>
        /// Identificador do tipo de situação
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }
        /// <summary>
        /// Data inicio da situação
        /// </summary>
        [Required]
        public DateTime dt_InicioSituacaoAcomodacao { get; set; }
        /// <summary>
        /// Data Fim da situação
        /// </summary>
        public DateTime? dt_FimSituacaoAcomodacao { get; set; }
        /// <summary>
        /// Numero de identificação do atendimento (para integração)
        /// </summary>
        [Required]
        public string cod_NumAtendimento { get; set; }
        /// <summary>
        /// Identificador do SLA
        /// </summary>
        public int? Id_SLA { get; set; }
        /// <summary>
        /// Indicador se é uma situação prioritária
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string Cod_Prioritario { get; set; }
        /// <summary>
        /// Indicador se ocorreu a alta administrativa
        /// </summary>
        public string Alta_Administrativa { get; set; }

        /// <summary>
        /// Identificador do Paciente 
        /// </summary>
        public int? Id_Paciente { get; set; }

        /// <summary>
        /// Id do paciente que está relacionado a Acomodação para armazenar 
        /// </summary>
        [JsonIgnore]
        public PacienteItem PacienteItem { get; set; }

        public SituacaoItem()
        {
        }

        [JsonIgnore]
        public List<AtividadeItem> AtividadeItems { get; set; }

    }
}
