using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class DashboardSituacaoAtividadeView : BaseView
    {
        //public List<DashboardAtividadeTO> DashboardAtividade { get; set; }

        public List<DashboardSituacaoAtividade_AcaoTO> DashboardSituacaoAtividadeAcao { get; set; }
        public List<TipoAtividadeAcomodacaoTO> LstTipoAtividade { get; set; }
        public List<TipoAcaoAcomodacaoTO> LstTipoAcao { get; set; }
        public List<SetorTO> LstSetor { get; set; }
        public string id_Setor { get; set; }
        public string id_TipoAtividade { get; set; }
        public string id_TipoSituacao { get; set; }

        public DashboardSituacaoAtividadeView()
        {
            this.DashboardSituacaoAtividadeAcao = new List<DashboardSituacaoAtividade_AcaoTO>();
            this.LstTipoAtividade = new List<TipoAtividadeAcomodacaoTO>();
            this.LstSetor = new List<SetorTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}