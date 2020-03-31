
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class UsuarioEmpresaPerfilItem
    {
        /// <summary>
        /// Identificador do Usuario
        /// </summary>
        [Required]
        public int id_Usuario { get; set; }
        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int id_Empresa { get; set; }
        /// <summary>
        /// Identificador do Perfil
        /// </summary>
        [Required]
        public int id_Perfil { get; set; }

        [JsonIgnore]
        public UsuarioItem UsuarioItem { get; set; }

        public UsuarioEmpresaPerfilItem() { }

    }
}
