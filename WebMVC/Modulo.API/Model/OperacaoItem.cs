
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Modulo.API.Model
{
    public class OperacaoItem
    {
        /// <summary>
        /// Identificador da Operação (Identity)
        /// </summary>
        [Required]
        public int Id_Operacao { get; set; }

        [Required]
        public int Id_Modulo { get; set; }

        [Required]
        [StringLength(50), MinLength(5)]
        public string Nome_Operacao { get; set; }

        [JsonIgnore]
        public ModuloItem ModuloItem { get; set; }

        public OperacaoItem() { }

    }
}
