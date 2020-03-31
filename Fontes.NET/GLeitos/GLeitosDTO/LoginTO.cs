using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class LoginTO
    {
        [Required(ErrorMessage="Login de preenchimento obrigatório.")]
        public string login { get; set; }
        [Required(ErrorMessage = "Senha de preenchimento obrigatório.")]
        public string senha { get; set; }
        public string id_empresa { get; set; }
        public string id_perfil { get; set; }

    }
}
