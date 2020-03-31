using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarDashBoardAtividadeTO
    {
        string m_sql;

        #region Atributos
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private long m_SLA_ATIVIDADE;
        private long m_Tempo_Utilizado_Atividade;
        private string m_Nome_Acomodacao;
        private string m_Nome_Setor;
        private string m_Nome_TipoAcaoAcomodacao;
        private string m_Nome_Status;
        private long m_SLA_ACAO;
        private long m_Tempo_Utilizado_Acao;
        private string m_Cod_Prioritario;
        private int m_Id_TipoSituacaoAcomodacao;
        private string  m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_Acomodacao;
        private string m_CodExterno_Acomodacao;
        private string m_Cod_Isolamento;
        private string m_PrioritarioAtividade;
        private string m_Cod_Plus;
        private string m_PendenciaFinanceira;
        #endregion

        #region Propriedades
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public long SLA_ATIVIDADE { get => m_SLA_ATIVIDADE; set => m_SLA_ATIVIDADE = value; }
        public long Tempo_Utilizado_Atividade { get => m_Tempo_Utilizado_Atividade; set => m_Tempo_Utilizado_Atividade = value; }
        public string Nome_Acomodacao { get => m_Nome_Acomodacao; set => m_Nome_Acomodacao = value; }
        public string Nome_Setor { get => m_Nome_Setor; set => m_Nome_Setor = value; }
        public string Nome_TipoAcaoAcomodacao { get => m_Nome_TipoAcaoAcomodacao; set => m_Nome_TipoAcaoAcomodacao = value; }
        public string Nome_Status { get => m_Nome_Status; set => m_Nome_Status = value; }
        public long SLA_ACAO { get => m_SLA_ACAO; set => m_SLA_ACAO = value; }
        public long Tempo_Utilizado_Acao { get => m_Tempo_Utilizado_Acao; set => m_Tempo_Utilizado_Acao = value; }
        public string Cod_Prioritario { get => m_Cod_Prioritario; set => m_Cod_Prioritario = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public string CodExterno_Acomodacao { get => m_CodExterno_Acomodacao; set => m_CodExterno_Acomodacao = value; }
        public string Cod_Isolamento { get => m_Cod_Isolamento; set => m_Cod_Isolamento = value; }
        public string PrioritarioAtividade { get => m_PrioritarioAtividade; set => m_PrioritarioAtividade = value; }
        public string Cod_Plus { get => m_Cod_Plus; set => m_Cod_Plus = value; }
        public string PendenciaFinanceira { get => m_PendenciaFinanceira; set => m_PendenciaFinanceira = value; }
        #endregion

        #region Construtor

        public ConsultarDashBoardAtividadeTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_Acomodacao = string.Empty;
            m_Nome_Setor = string.Empty;
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;
            m_Cod_Prioritario = string.Empty;
            m_CodExterno_Acomodacao = string.Empty;
            m_Cod_Isolamento = string.Empty;
            m_PrioritarioAtividade = string.Empty;
            m_Cod_Plus = string.Empty;
              m_PendenciaFinanceira = string.Empty;
    }



        #endregion

        #region SQL
        public void ConsultarDashBoardAtividadeTOCommand(int IdEmpresa, int IdTipoAtividadeAcomodacao, string connection, ref List<ConsultarDashBoardAtividadeTO> l_ListTO)
        {

            m_sql = " SELECT * ";
            m_sql += " FROM vw_ConsultarDashBoardAtividade ";
            m_sql += "WHERE (Id_Empresa = @Id_Empresa) AND(Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            m_sql += "ORDER BY Nome_TipoAtividadeAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividadeAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        #endregion
    }
}
