using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class  PerfilView : BaseView
    {

        public PerfilTO Perfil { get; set; }
        public List<PerfilTO> ListaPerfil { get; set; }
        public List<AcessoEmpresaPerfilTsTaConsultaTO> ListaAcessoEmpresaPerfilTsTa { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacao { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacao { get; set; }
        public List<TipoAcessoTO> ListaTipoAcesso { get; set; }
        public List<TipoPerfilTO> ListaTipoPerfil { get; set; }
        public string codTipoPerfil { get; set; }

        public PerfilView()
        {
            this.ListaPerfil = new List<PerfilTO>();
            this.ListaAcessoEmpresaPerfilTsTa = new List<AcessoEmpresaPerfilTsTaConsultaTO>();
            this.ListaTipoAtividadeAcomodacao = new List<TipoAtividadeAcomodacaoTO>();
            this.ListaTipoSituacaoAcomodacao = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAcesso = new List<TipoAcessoTO>();
            this.codTipoPerfil = "0";

            this.Perfil = new PerfilTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}