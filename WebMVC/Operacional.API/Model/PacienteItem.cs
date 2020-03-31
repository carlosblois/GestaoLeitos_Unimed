using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class PacienteItem
    {
                        

        /// <summary>
        /// Identificador do Paciente (Identity)
        /// </summary>
        [Required]
        public int Id_Paciente { get; set; }

        /// <summary>
        /// Nome do paciente
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome_Paciente { get; set; }

        /// <summary>
        /// Identificador externo do paciente para finalidade de integração
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Cod_Externo { get; set; }


        /// <summary>
        /// Data de Nascimento do Paciente
        /// </summary>
        [Required]
        public DateTime Dt_NascimentoPaciente { get; set; }

        /// <summary>
        /// Genero do Paciente
        /// </summary>
        [Required]
        public string GeneroPaciente { get; set; }

        /// <summary>
        /// Pendencia Financeira
        /// </summary>
        [Required]
        [RegularExpression(@"[S,N]", ErrorMessage = "As opções válidas são S - Com Pendencia ou N - Sem Pendencia")]
        [StringLength(1)]
        public string PendenciaFinanceira { get; set; }

        public List<PacienteAcomodacaoItem> PacienteAcomodacaoItems { get; set; }

        public List<SituacaoItem> SituacaoAcomodacaoItems { get; set; }

        public PacienteItem()
        {
            this.PendenciaFinanceira = "N";
        }

    }
}
