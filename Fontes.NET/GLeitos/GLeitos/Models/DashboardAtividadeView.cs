using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class DashboardAtividadeView : BaseView
    {
        //public List<DashboardAtividadeTO> DashboardAtividade { get; set; }

        public List<DashboardAtividade_SituacaoTO> DashboardAtividade { get; set; }
        public List<TipoAtividadeAcomodacaoTO> LstTipoAtividade { get; set; }
        public List<SetorTO> LstSetor { get; set; }
        public string id_Setor { get; set; }
        public string id_TipoAtividade { get; set; }

        public DashboardAtividadeView()
        {
            this.DashboardAtividade = new List<DashboardAtividade_SituacaoTO>();
            this.LstTipoAtividade = new List<TipoAtividadeAcomodacaoTO>();
            this.LstSetor = new List<SetorTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}