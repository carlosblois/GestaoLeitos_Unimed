
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class UsuarioItem 
    {
        /// <summary>
        /// Identificador do Usuario (Identity)
        /// </summary>
        [Required]
        public int id_Usuario { get; set; }

        /// <summary>
        /// Nome do Usuario - (Min 5 Max 50)
        /// </summary>
        [Required]
        [StringLength(50),MinLength(5)]
        public string nome_Usuario { get; set; }

        /// <summary>
        /// Login do Usuario - (Min 5 Max 20)
        /// </summary>
       
        [Required]
        [StringLength(20), MinLength(5)]
        public string login_Usuario { get; set; }

        /// <summary>
        /// Senha do Usuario - (Min 5 Max 10)
        /// </summary>
        [Required]
        [StringLength(10), MinLength(5)]
        public string senha_Usuario { get; set; }

        /// <summary>
        /// Ativo ()
        /// </summary>
        public byte Ativo { get; set; }

        public  List<UsuarioEmpresaPerfilItem> UsuarioEmpresaPerfilItems { get; set; }

        public UsuarioItem() { }

    }
}
