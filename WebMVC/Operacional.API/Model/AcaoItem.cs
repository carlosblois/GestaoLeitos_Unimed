using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.Model
{
    public class AcaoItem
    {
                 
        /// <summary>
        /// Identificador da Acao de uma atividade (Identity)
        /// </summary>
        [Required]
        public int Id_AcaoAtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador da atividade
        /// </summary>
        [Required]
        public int Id_AtividadeAcomodacao { get; set; }

        /// <summary>
        /// Identificador do tipo de ação
        /// </summary>
        [Required]
        public int Id_TipoAcaoAcomodacao { get; set; }


        /// <summary>
        /// Data de incicio da ação
        /// </summary>
        [Required]
        public DateTime dt_InicioAcaoAtividade { get; set; }

        /// <summary>
        /// Data fim da ação
        /// </summary>
        public DateTime? dt_FimAcaoAtividade { get; set; }

        /// <summary>
        /// SLA de execução da ação
        /// </summary>
        public int? Id_SLA { get; set; }

        /// <summary>
        /// Identificador do usuario executor da ação
        /// </summary>
        public string Id_UsuarioExecutor { get; set; }

        [JsonIgnore]
        public AtividadeItem AtividadeItem { get; set; }

        public AcaoItem() { }

    }
}
