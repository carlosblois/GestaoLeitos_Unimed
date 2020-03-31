using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class AcessoEmpresaPerfilTsTaConsultaTO
    {
        public string id_Empresa { get; set; }
        public string id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade { get; set; }
        public string id_Perfil { get; set; }
        public string nome_Perfil { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string cod_Tipo { get; set; }
        

        public AcessoEmpresaPerfilTsTaConsultaTO()
        {
            this.id_Empresa = "";
            this.id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade = "";
            this.id_Perfil = "";
            this.nome_Perfil = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.cod_Tipo = "";

        }
    }

    public class AcessoEmpresaPerfilTsTaTO
    {
        public string id_Empresa { get; set; }
        public string id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade { get; set; }
        public string id_Perfil { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string cod_Tipo { get; set; }


        public AcessoEmpresaPerfilTsTaTO()
        {
            this.id_Empresa = "";
            this.id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade = "";
            this.id_Perfil = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.cod_Tipo = "";

        }
    }


}
