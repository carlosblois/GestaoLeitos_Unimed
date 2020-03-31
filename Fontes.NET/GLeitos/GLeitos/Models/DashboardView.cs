using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class DashboardView: BaseView
    {
        public List<DashboardHeaderTO> HeaderSituacao { get; set; }
        public List<DashboardBodyAtividadeTO> BodyAtividade { get; set; }

        public DashboardView()
        {
            this.HeaderSituacao = new List<DashboardHeaderTO>();
            this.BodyAtividade = new List<DashboardBodyAtividadeTO>();
                
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}