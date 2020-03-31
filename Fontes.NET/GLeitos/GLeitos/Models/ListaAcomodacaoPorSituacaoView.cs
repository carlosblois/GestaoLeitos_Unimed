using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaAcomodacaoPorSituacaoView: BaseView
    {

        public List<AcomodacaoConsultaSituacaoTO> ListaAcomodacoesPorSituacao { get; set; }
        public List<TipoSituacaoAcomodacaoTO> ListaTipoSituacaoAcomodacaoTO { get; set; }
        public List<TipoAtividadeAcomodacaoTO> ListaTipoAtividadeAcomodacaoTO { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string id_TipoAtividadeAcomodacao { get; set; }

        public ListaAcomodacaoPorSituacaoView()
        {
            this.ListaAcomodacoesPorSituacao = new List<AcomodacaoConsultaSituacaoTO>();
            this.ListaTipoSituacaoAcomodacaoTO = new List<TipoSituacaoAcomodacaoTO>();
            this.ListaTipoAtividadeAcomodacaoTO = new List<TipoAtividadeAcomodacaoTO>();
            this.id_TipoSituacaoAcomodacao = string.Empty;
            this.id_TipoAtividadeAcomodacao = string.Empty;
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}