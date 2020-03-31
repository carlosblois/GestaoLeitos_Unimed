using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class FluxoAutomaticoConsultaTO
    {
        public string id_TipoAtividadeAcomodacaoOrigem { get; set; }
        public string id_TipoSituacaoAcomodacaoOrigem { get; set; }
        public string id_TipoAcaoAcomodacaoOrigem { get; set; }
        public string id_TipoAtividadeAcomodacaoDestino { get; set; }
        public string id_TipoSituacaoAcomodacaoDestino { get; set; }
        public string id_Empresa { get; set; }
        public string nome_TipoSituacaoAcomodacaoOrigem { get; set; }
        public string nome_TipoAtividadeAcomodacaoOrigem { get; set; }
        public string nome_TipoAcaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacaoDestino { get; set; }
        public string nome_TipoAtividadeAcomodacaoDestino { get; set; }

        public FluxoAutomaticoConsultaTO()
        {
            this.id_TipoAtividadeAcomodacaoOrigem = "";
            this.id_TipoSituacaoAcomodacaoOrigem = "";
            this.id_TipoAtividadeAcomodacaoDestino = "";
            this.id_TipoSituacaoAcomodacaoDestino = "";
            this.id_Empresa = "";
            this.nome_TipoSituacaoAcomodacaoOrigem = "";
            this.nome_TipoAtividadeAcomodacaoOrigem = "";
            this.nome_TipoSituacaoAcomodacaoDestino = "";
            this.nome_TipoAtividadeAcomodacaoDestino = "";
            this.id_TipoAtividadeAcomodacaoDestino = "";
            this.nome_TipoAcaoAcomodacao = "";

        }
    }
}
