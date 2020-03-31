using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class AtividadeAcomodacaoTO
    {

        [Display(Description = "Identificador da Atividade(Identity)")]
        public string id_AtividadeAcomodacao { get; set; }

        [Display(Description = "Identificador da situação")]
        public string id_SituacaoAcomodacao { get; set; }

        [Display(Description = "Identificador do tipo de situação")]
        public string id_TipoSituacaoAcomodacao { get; set; }

        [Display(Description = "Identificador do tipo de atividade")]
        public string id_TipoAtividadeAcomodacao { get; set; }

        [Display(Description = "Data de incicio da atividade")]
        public string dt_InicioAtividadeAcomodacao { get; set; }

        [Display(Description = "Data fim da atividade")]
        public string dt_FimAtividadeAcomodacao { get; set; }

        [Display(Description = "Identificador do usuario solicitante")]
        public string id_UsuarioSolicitante { get; set; }

        public string Cod_Prioritario { get; set; }

        public string Cod_Plus { get; set; }

        [Display(Description = "Lista de ações para a atividade na acomodação")]
        public List<AcaoAtividadeAcomodacaoTO> acaoItems { get; set; }

        public AtividadeAcomodacaoTO()
        {
            this.id_AtividadeAcomodacao = "";
            this.id_SituacaoAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.id_TipoAtividadeAcomodacao = "";
            this.dt_InicioAtividadeAcomodacao = "";
            this.dt_FimAtividadeAcomodacao = "";
            this.id_UsuarioSolicitante = "";
            this.acaoItems = new List<AcaoAtividadeAcomodacaoTO>();
        }
    }
}
