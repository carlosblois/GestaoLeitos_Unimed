
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class UsuarioPerfilItem
    {
        [Required]
        public UsuarioItem  usuario { get; set; }
        [Required]
        public UsuarioEmpresaPerfilItem perfil { get; set; }
    }
}
