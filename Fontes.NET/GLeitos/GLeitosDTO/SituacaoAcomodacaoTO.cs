using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class SituacaoAcomodacaoTO
    {

        public string id_SituacaoAcomodacao { get; set; }
        public string id_Acomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string dt_InicioSituacaoAcomodacao { get; set; }
        public string dt_FimSituacaoAcomodacao { get; set; }
        public string cod_NumAtendimento { get; set; }
        public string id_SLA { get; set; }
        public string cod_Prioritario { get; set; }
        //public List<AtividadeAcomodacaoTO> atividadeItems { get; set; }

        public SituacaoAcomodacaoTO()
        {
            this.id_SituacaoAcomodacao = "";
            this.id_Acomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.dt_InicioSituacaoAcomodacao = "";
            this.dt_FimSituacaoAcomodacao = "";
            this.cod_NumAtendimento = "";
            this.id_SLA = "";
            this.cod_Prioritario = "";
           // this.atividadeItems = new List<AtividadeAcomodacaoTO>();
        }
    }
}
