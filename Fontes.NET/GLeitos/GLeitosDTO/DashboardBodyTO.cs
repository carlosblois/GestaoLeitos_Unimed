using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class DashboardBodyTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string qtD_POR_ATV { get; set; }
        public string peR_POR_ATV { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string qtD_POR_SIT { get; set; }
        public string tempO_Utilizado { get; set; }
        public string tempO_UtilizadoAt { get; set; }
        public string FORASLA { get; set; }
        public string MaiorTempo { get; set; }

        public DashboardBodyTO()
        {
            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.qtD_POR_ATV = "";
            this.peR_POR_ATV = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.qtD_POR_SIT = "";
            this.tempO_Utilizado = "";
            this.tempO_UtilizadoAt = "";
        }
    }

    public class DashboardBodyAtividadeTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string qtD_POR_ATV { get; set; }
        public string peR_POR_ATV { get; set; }
        public string tempO_Utilizado { get; set; }
        public string imagem { get; set; }
        public string cor { get; set; }
        public int ordem { get; set; }
        public string corsla { get; set; }
        public string totalAcessos { get; set; }
        public List<DashboardBodySituacaoTO> DashboardSituacao { get; set; }

        public DashboardBodyAtividadeTO()
        {
            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.qtD_POR_ATV = "";
            this.peR_POR_ATV = "";
            this.tempO_Utilizado = "";
            this.imagem = "";
            this.cor = "";
            this.corsla = "";
            this.totalAcessos = "0";
            this.DashboardSituacao = new List<DashboardBodySituacaoTO>();
        }
    }

    public class DashboardBodySituacaoTO
    {

        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string qtD_POR_SIT { get; set; }
        public string tempO_Utilizado_SIT { get; set; }
        public string sla_SIT { get; set; }
        public string imagem { get; set; }
        public string cor { get; set; }
        public int ordem { get; set; }
        public string tempO_UtilizadoAt { get; set; }
        public string FORASLA { get; set; }
        public string MaiorTempo { get; set; }

        public DashboardBodySituacaoTO()
        {
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.qtD_POR_SIT = "";
            this.tempO_Utilizado_SIT = "";
            this.sla_SIT = "";
            this.imagem = "";
            this.cor = "";
            this.tempO_UtilizadoAt = "";
            this.FORASLA = "";
            this.MaiorTempo = "";
        }
    }

}
