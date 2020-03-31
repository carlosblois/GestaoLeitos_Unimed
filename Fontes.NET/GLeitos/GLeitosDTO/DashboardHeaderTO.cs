using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class DashboardHeaderTO
    {
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string imagem { get; set; }
        public int ordem { get; set; }
        public string corsituacao { get; set; }
        public string qtd { get; set; }
        public string perc { get; set; }

        public DashboardHeaderTO()
        {
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.imagem = "";
            this.corsituacao = "";
            this.qtd = "";
            this.perc = "";
        }
    }
}
