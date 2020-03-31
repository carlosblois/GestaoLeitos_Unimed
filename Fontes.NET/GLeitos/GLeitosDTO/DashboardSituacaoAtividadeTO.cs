using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class DashboardSituacaoAtividadeTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string nome_Acomodacao { get; set; }
        public string nome_TipoAcaoAcomodacao { get; set; }
        public string id_TipoAcaoAcomodacao { get; set; }
        public string nome_Status { get; set; }
        public string nome_status_Label { get; set; }
        public string id_Acomodacao { get; set; }
        public string nome_Setor { get; set; }
        public string Id_Setor { get; set; }
        public string dt_InicioSituacaoAcomodacao { get; set; }
        public string dt_InicioAtividadeAcomodacao { get; set; }
        public string dt_InicioAcaoAtividade { get; set; }
        
        public string slaAcao { get; set; }
        public string slaAtividade { get; set; }
        public string tempo_Utilizado_Acao { get; set; }
        public string tempo_Utilizado_Atividade { get; set; }
        public string id_Empresa { get; set; }
        public string codExterno_Acomodacao { get; set; }
        public string cod_Isolamento { get; set; }
        public string prioritarioAtividade { get; set; }
        public string cod_Plus { get; set; }
        public string imagem { get; set; }
        public string cor { get; set; }
        public int ordemAcao { get; set; }
        public string pendenciaFinanceira { get; set; }

        public DashboardSituacaoAtividadeTO()
        {

            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.nome_Acomodacao = "";
            this.nome_TipoAcaoAcomodacao = "";
            this.nome_Status = "";
            this.nome_status_Label = "";
            this.id_Acomodacao = "";
            this.nome_Setor = "";
            this.Id_Setor = "";
            this.dt_InicioSituacaoAcomodacao = "";
            this.dt_InicioAtividadeAcomodacao = "";
            this.dt_InicioAcaoAtividade = "";
            this.id_TipoAcaoAcomodacao = "";
            this.slaAcao = "";
            this.slaAtividade = "";
            this.tempo_Utilizado_Acao = "";
            this.tempo_Utilizado_Atividade = "";
            this.id_Empresa = "";
            this.codExterno_Acomodacao = "";
            this.cod_Isolamento = "";
            this.prioritarioAtividade = "";
            this.cod_Plus = "";
            this.imagem = "";
            this.cor = "";

        }
    }

    public class DashboardSituacaoAtividade_AcaoTO
    {
        public string id_TipoAtividadeAcomodacao { get; set; }
        public string nome_TipoAtividadeAcomodacao { get; set; }
        public string id_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoSituacaoAcomodacao { get; set; }
        public string nome_TipoAcaoAcomodacao { get; set; }
        public string id_TipoAcaoAcomodacao { get; set; }
        public string nome_Status_Label { get; set; }
        public string imagem { get; set; }
        public string cor { get; set; }
        public int ordemAcao { get; set; }
        public List<DashboardSituacaoAtividade_AcomodacaoTO> Acomodacoes { get; set; }
        
        public DashboardSituacaoAtividade_AcaoTO()
        {

            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.nome_TipoAcaoAcomodacao = "";
            this.id_TipoAcaoAcomodacao = "";
            this.nome_Status_Label = "";
            this.imagem = "";
            this.cor = "";
            this.ordemAcao = 0;
            this.Acomodacoes = new List<DashboardSituacaoAtividade_AcomodacaoTO>();

        }
    }

    public class DashboardSituacaoAtividade_AcomodacaoTO
    {
        public string slA_ATIVIDADE { get; set; }
        public string tempo_Utilizado_Atividade { get; set; }
        public string nome_Acomodacao { get; set; }
        public string id_Setor { get; set; }
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
        public string cod_Isolamento { get; set; }
        public string cod_Plus { get; set; }
        public string prioritarioAtividade { get; set; }
        public string id_PerfilUsuario { get; set; }
        public string id_PerfilAdministrador { get; set; }
        public string cod_Acesso { get; set; }
        public string pendenciaFinanceira { get; set; }
        public string imagem_pendencia_financeira { get; set; }

        public DashboardSituacaoAtividade_AcomodacaoTO()
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
