
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class SetorItem
    {

        /// <summary>
        /// Identificador do Setor (Identity)
        /// </summary>
        [Required]
        public int id_Setor { get; set; }


        /// <summary>
        /// Identificador da Empresa 
        /// </summary>
        [Required]
        public int id_Empresa { get; set; }


        /// <summary>
        /// Nome do Setor (Max 50)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string nome_Setor { get; set; }

        /// <summary>
        /// Codigo externo do Setor, com proposito de integração. (Max 4)
        /// </summary>
        [Required]
        [StringLength(4)]
        public string CodExterno_Setor { get; set; }

        public SetorItem() { }

    }
}
