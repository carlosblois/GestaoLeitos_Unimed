using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class MensagemItem
    {
                 
        /// <summary>
        /// Identificador da Mensagem (Identity)
        /// </summary>
        [Required]
        public int Id_Mensagem { get; set; }

        /// <summary>
        /// Identificador da atividade
        /// </summary>
        public int? Id_AtividadeAcomodacao { get; set; }

        /// <summary>
        /// Data de envio da mensagem
        /// </summary>
        [Required]
        public DateTime dt_EnvioMensagem { get; set; }

        /// <summary>
        /// Data em que o destinatário recebeu a mensagem
        /// </summary>
        public DateTime? dt_RecebimentoMensagem { get; set; }

        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }

        /// <summary>
        /// Id do usuário emissor da mensagem
        /// </summary>
        [Required]
        public int Id_Usuario_Emissor { get; set; }

        /// <summary>
        /// Id do usuário destinatário da mensagem
        /// </summary>
        public int? Id_Usuario_Destinatario { get; set; }

        /// <summary>
        /// Texto da mensagem enviada
        /// </summary>
        [Required]
        public string TextoMensagem { get; set; }


        [JsonIgnore]
        public AtividadeItem AtividadeItem { get; set; }

        public MensagemItem() { }

    }
}
