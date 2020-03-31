using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class DashboardSituacaoView: BaseView
    {
        public List<DashboardSituacao_AtividadeTO> DashboardSituacao { get; set; }

        public List<DashboardSituacao_AtividadeTO> DashboardSituacaoBacklog { get; set; }

        public List<TipoAtividadeAcomodacaoTO> LstTipoAtividade { get; set; }
        public List<SetorTO> LstSetor { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string id_Setor { get; set; }
        public string id_TipoAtividade { get; set; }

        public DashboardSituacaoView()
        {
            this.DashboardSituacao = new List<DashboardSituacao_AtividadeTO>();
            this.DashboardSituacaoBacklog = new List<DashboardSituacao_AtividadeTO>();
            this.LstTipoAtividade = new List<TipoAtividadeAcomodacaoTO>();
            this.LstSetor = new List<SetorTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}