using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class AcaoAtividadeAcomodacaoTO
    {

        [Display(Description = "Identificador da Acao de uma atividade(Identity)")]
        public string id_AcaoAtividadeAcomodacao { get; set; }

        [Display(Description = "Identificador da atividade")]
        public string id_AtividadeAcomodacao { get; set; }

        [Display(Description = "Identificador do tipo de ação")]
        public string id_TipoAcaoAcomodacao { get; set; }

        [Display(Description = "Data de incicio da ação")]
        public string dt_InicioAcaoAtividade { get; set; }

        [Display(Description = "Data fim da ação")]
        public string dt_FimAcaoAtividade { get; set; }

        [Display(Description = "SLA de execução da ação")]
        public string id_SLA { get; set; }

        [Display(Description = "Identificador do usuario executor da ação")]
        public string id_UsuarioExecutor { get; set; }


        public AcaoAtividadeAcomodacaoTO()
        {
            this.id_AcaoAtividadeAcomodacao = "";
            this.id_AtividadeAcomodacao = "";
            this.id_TipoAcaoAcomodacao = "";
            this.dt_InicioAcaoAtividade = "";
            this.dt_FimAcaoAtividade = "";
            this.id_SLA = "";
            this.id_UsuarioExecutor = "";

        }
    }
}
