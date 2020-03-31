using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class EmpresaPerfilTO
    {
       
        public string id_Empresa {get;set;}
        public string id_Perfil { get; set; }
        public string cod_Tipo { get; set; }

        public EmpresaPerfilTO()
        {

            this.id_Empresa = "";
            this.id_Perfil = "";
            this.cod_Tipo = "";

        }
    }

    public class EmpresaPerfilConsultaTO
    {

        public string id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
        public string id_Perfil { get; set; }
        public string nome_Perfil { get; set; }
        public string cod_Tipo_Perfil { get; set; }
        public string cod_Tipo_Descricao_Perfil { get; set; }

        public EmpresaPerfilConsultaTO()
        {

            this.id_Empresa = "";
            this.id_Perfil = "";
            this.cod_Tipo_Perfil = "";

        }
    }
}
