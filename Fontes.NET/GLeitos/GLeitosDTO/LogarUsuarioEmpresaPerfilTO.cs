using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLeitos.GLeitosTO
{
    public class LogarUsuarioEmpresaPerfilTO
    {

        public int id_Usuario { get; set; }
        public string nome_Usuario { get; set; }
        public int id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
        public int id_Perfil { get; set; }
        public string nome_Perfil { get; set; }
        public string cod_Tipo { get; set; }
        public string cod_Tipo_Descricao { get; set; }

    }

    public class LogarUsuarioEmpresaTO
    {
        public int id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
    }

    public class LogarUsuarioPerfilTO
    {
        public int id_Perfil { get; set; }
        public string nome_Perfil { get; set; }
    }

}
