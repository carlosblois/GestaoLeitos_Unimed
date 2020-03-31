using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class DashboardSituacaoTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string slA_ATIVIDADE { get; set; }
        public string tempo_Utilizado_Atividade { get; set; }
        public string id_Acomodacao { get; set; }
        public string nome_Acomodacao { get; set; }
        public string nome_Setor { get; set; }
        public string id_Setor { get; set; }
        public string nome_TipoAcaoAcomodacao { get; set; }
        public string nome_Status { get; set; }
        public string nome_Status_Label { get; set; }
        public string slA_ACAO { get; set; }
        public string tempo_Utilizado_Acao { get; set; }
        public string cod_Prioritario { get; set; }
        public string imagem { get; set; }
        public string cor{ get; set; }
        public string cod_Isolamento { get; set; }
        public string cod_Plus { get; set; }
        public string prioritarioAtividade { get; set; }
        public int ordem { get; set; }
        public string pendenciaFinanceira { get; set;}

        public DashboardSituacaoTO()
        {

            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.slA_ATIVIDADE = "";
            this.tempo_Utilizado_Atividade = "";
            this.id_Acomodacao = "";
            this.nome_Acomodacao = "";
            this.nome_Setor = "";
            this.nome_TipoAcaoAcomodacao = "";
            this.nome_Status = "";
            this.nome_Status_Label = "";
            this.slA_ACAO = "";
            this.tempo_Utilizado_Acao = "";
            this.cod_Prioritario = "";
            this.imagem = "";
            this.cor = "";

        }
    }

    public class DashboardSituacao_AtividadeTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string imagem { get; set; }
        public string cor { get; set; }
        public int ordem { get; set; }
        public List<DashboardSituacao_AcomodacaoTO> Acomodacoes { get; set; }

        public DashboardSituacao_AtividadeTO()
        {

            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.imagem = "";
            this.cor = "";
            this.Acomodacoes = new List<DashboardSituacao_AcomodacaoTO>();

        }
    }

    public class DashboardSituacao_AcomodacaoTO
    {
        public string slA_ATIVIDADE { get; set; }
        public string tempo_Utilizado_Atividade { get; set; }
        public string nome_Acomodacao { get; set; }
        public string nome_Setor { get; set; }
        public string nome_TipoAcaoAcomodacao { get; set; }
        public string nome_Status { get; set; }
        public string nome_Status_Label { get; set; }
        public string slA_ACAO { get; set; }
        public string tempo_Utilizado_Acao { get; set; }
        public string cod_Prioritario { get; set; }
        public string id_Acomodacao { get; set; }
        public string imagem_sla { get; set; }
        public string imagem_prioridade { get; set; }
        public string imagem_isolamento { get; set; }
        public string imagem_limpezaplus { get; set; }
        public string imagem_sla_backlog { get; set; }
        public string imagem_prioridade_backlog { get; set; }
        public string imagem_isolamento_backlog { get; set; }
        public string imagem_limpezaplus_backlog { get; set; }
        public string cod_Isolamento { get; set; }
        public string cod_Plus { get; set; }
        public string prioritarioAtividade { get; set; }
        public string id_PerfilUsuario { get; set; }
        public string id_PerfilAdministrador { get; set; }
        public string cod_Acesso { get; set; }
        public string pendenciaFinanceira { get; set; }
        public string imagem_pendencia_financeira { get; set; }

        public DashboardSituacao_AcomodacaoTO()
        {

            this.slA_ATIVIDADE = "";
            this.tempo_Utilizado_Atividade = "";
            this.nome_Acomodacao = "";
            this.nome_Setor = "";
            this.nome_TipoAcaoAcomodacao = "";
            this.nome_Status = "";
            this.nome_Status_Label = "";
            this.slA_ACAO = "";
            this.tempo_Utilizado_Acao = "";
            this.cod_Prioritario = "";
            this.id_Acomodacao = "";

            this.imagem_sla = "";
            this.imagem_prioridade = "";
            this.imagem_isolamento = "";
            this.imagem_limpezaplus = "";

        }
    }
}
