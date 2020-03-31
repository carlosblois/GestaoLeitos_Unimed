
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Modulo.API.Model
{
    public class EmpresaPerfilModuloItem
    {
        [Required]
        public int Id_Empresa { get; set; }

        [Required]
        public int Id_Perfil { get; set; }

        [Required]
        public int Id_Modulo { get; set; }

        [JsonIgnore]
        public ModuloItem ModuloItem { get; set; }

        public EmpresaPerfilModuloItem() { }

    }
}
