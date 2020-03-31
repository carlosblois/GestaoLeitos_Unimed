using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class PacienteAcomodacaoItem
    {

        /// <summary>
        /// Identificador do Paciente Acomodação (Identity)
        /// </summary>
        [Required]
        public int Id_PacienteAcomodacao { get; set; }

        /// <summary>
        /// Identificador do Paciente 
        /// </summary>
        [Required]
        public int Id_Paciente { get; set; }

        /// <summary>
        /// Identificador da Acomodação
        /// </summary>
        [Required]
        public int Id_Acomodacao { get; set; }


        /// <summary>
        /// Data de entrada na acomodação
        /// </summary>
        [Required]
        public DateTime Dt_Entrada { get; set; }

        /// <summary>
        /// Data de saída da acomodação
        /// </summary>
        public DateTime? Dt_Saida { get; set; }


        /// <summary>
        /// Numero de Atendimento do Paciente
        /// </summary>
        [Required]
        public string NumAtendimento { get; set; }


        [JsonIgnore]
        public PacienteItem PacienteItem { get; set; }


        public PacienteAcomodacaoItem() { }

    }
}
