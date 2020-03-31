using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class UsuarioTO
    {

        public string id_Usuario { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nome_Usuario { get; set; }
        [Required(ErrorMessage = "O login é obrigatório.")]
        public string login_Usuario { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string senha_Usuario { get; set; }
        public byte ativo { get; set; }
        [Required(ErrorMessage = "O Perfil é obrigatório.")]
        public List<PerfilItemsTO> usuarioEmpresaPerfilItems { get; set; }

        public UsuarioTO()
        {
            this.id_Usuario = "";
            this.nome_Usuario = "";
            this.login_Usuario = "";
            this.senha_Usuario = "";
            this.ativo = 1;
            this.usuarioEmpresaPerfilItems = new List<PerfilItemsTO>();
        }
    }

    public class PerfilItemsTO
    {
        public string id_Usuario { get; set; }
        public string id_Empresa { get; set; }
        public string id_Perfil { get; set; }
    }


    public class ConsultarPerfisPorIdUsuarioTO
    {
        public int id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
        public int id_Perfil { get; set; }
        public string nome_Perfil { get; set; }
        public int id_Usuario { get; set; }
        public string nome_Usuario { get; set; }
        public string cod_Tipo { get; set; }
        public string cod_Tipo_Descricao { get; set; }
    }

}
