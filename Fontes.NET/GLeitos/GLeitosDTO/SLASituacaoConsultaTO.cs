using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class SLASituacaoConsultaTO
    {

        public string id_SLA { get; set; }
        public string id_Empresa { get; set; }
        public string nome_Empresa { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAcomodacao { get; set; }
        public string nome_TipoAcomodacao { get; set; }
        public string versao_SLA { get; set; }
        public string tempo_Minutos { get; set; }
    
        public SLASituacaoConsultaTO()
        {
            this.id_SLA = "";
            this.id_Empresa = "";
            this.nome_Empresa = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.versao_SLA = "";
            this.tempo_Minutos = "";
    
    
        }
    }
}
