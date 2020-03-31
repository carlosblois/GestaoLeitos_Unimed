using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class AcomodacaoConsultaSituacaoTO
    {
        public string id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
        public string id_Setor { get; set; }
        public string nome_Setor { get; set; }
        public string id_Acomodacao { get; set; }
        public string nome_Acomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }
        public string nome_TipoAcomodacao { get; set; }
        public string id_CaracteristicaAcomodacao { get; set; }
        public string codExterno_Acomodacao { get; set; }
        public string nome_CaracteristicaAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string id_SituacaoAcomodacao { get; set; }
        public string id_PerfilUsuario { get; set; }
        public string id_PerfilAdministrador { get; set; }
        public string cod_Acesso { get; set; }


        public AcomodacaoConsultaSituacaoTO()
        {
            this.id_Empresa = "";
            this.nome_Empresa = "";
            this.id_Setor = "";
            this.nome_Setor = "";
            this.id_Acomodacao = "";
            this.nome_Acomodacao = "";
            this.id_TipoAcomodacao = "";
            this.nome_TipoAcomodacao = "";
            this.id_CaracteristicaAcomodacao = "";
            this.nome_CaracteristicaAcomodacao = "";
            this.codExterno_Acomodacao = "";
            this.id_SituacaoAcomodacao = "";
        }
    }
}
