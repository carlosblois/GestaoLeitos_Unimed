using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class FluxoAutomaticoTO
    {
        [Required(ErrorMessage = "A Ação de origem é obrigatória.")]
        public string id_TipoAcaoAcomodacaoOrigem { get; set; }
        [Required(ErrorMessage = "A Atividade de origem é obrigatória.")]
        public string id_TipoAtividadeAcomodacaoOrigem { get; set; }
        [Required(ErrorMessage = "A Situação de origem é obrigatória.")]
        public string id_TipoSituacaoAcomodacaoOrigem { get; set; }
        [Required(ErrorMessage = "A Atividade de destino é obrigatória.")]
        public string id_TipoAtividadeAcomodacaoDestino { get; set; }
        [Required(ErrorMessage = "A Situação de destio é obrigatória.")]
        public string id_TipoSituacaoAcomodacaoDestino { get; set; }
        public string id_Empresa { get; set; }

        public FluxoAutomaticoTO()
        {
            this.id_TipoAcaoAcomodacaoOrigem = "";
            this.id_TipoAtividadeAcomodacaoOrigem = "";
            this.id_TipoSituacaoAcomodacaoOrigem = "";
            this.id_TipoAtividadeAcomodacaoDestino = "";
            this.id_TipoSituacaoAcomodacaoDestino = "";
            this.id_Empresa = "";
        }
    }
}
