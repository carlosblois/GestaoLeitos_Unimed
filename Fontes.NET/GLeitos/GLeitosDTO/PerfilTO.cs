using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class PerfilTO
    {
     
        public string id_Perfil { get;set;}
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        public string nome_Perfil { get; set; }

        public PerfilTO()
        {
            this.id_Perfil = "";
            this.nome_Perfil = "";

        }
    }
}
