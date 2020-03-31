
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class LogAcessoItem
    {

        /// <summary>
        /// Identificador do Log de acesso (Identity)
        /// </summary>
        [Required]
        public int id_Log { get; set; }

        /// <summary>
        /// Identificador do Usuario
        /// </summary>
        [Required]
        public int id_Usuario { get; set; }

        /// <summary>
        /// Data / hora da entrada do usuario
        /// </summary>
        [Required]
        public DateTime dt_Entrada { get; set; }

        /// <summary>
        /// Data / hora da saida do usuario
        /// </summary>
        [Required]
        public DateTime? dt_Saida { get; set; }


        /// <summary>
        /// Codigo de indicacao de acesso valido ou nao S/N
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("N|S")]
        public string cod_sucesso { get; set; }
        
        public LogAcessoItem() { }

    }
}
