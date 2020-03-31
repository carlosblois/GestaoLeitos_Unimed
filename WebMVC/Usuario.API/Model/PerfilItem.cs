
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class PerfilItem
    {
        /// <summary>
        /// Identificador do Perfil (Identity)
        /// </summary>
        [Required]
        public int id_Perfil { get; set; }

        /// <summary>
        /// Nome do Perfil - (Min 5 Max 20)
        /// </summary>
        [Required]
        [StringLength(20), MinLength(5)]
        public string nome_Perfil { get; set; }


        public List<EmpresaPerfilItem> EmpresaPerfilItems { get; set; }

        public PerfilItem() { }

    }
}
