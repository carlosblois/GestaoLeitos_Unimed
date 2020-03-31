using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarDashBoardSATO
    {
        string m_sql;


        #region Atributos
        private int m_Id_Acomodacao;
        private string m_Nome_Acomodacao;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private DateTime m_dt_InicioSituacaoAcomodacao;
        private DateTime m_dt_InicioAtividadeAcomodacao;
        private DateTime m_dt_InicioAcaoAtividade;
        private int m_Id_TipoAcaoAcomodacao;
        private string m_Nome_TipoAcaoAcomodacao;
        private string m_Nome_Status;
        private int m_SLAAcao;
        private int m_SLAAtividade;
        private long m_Tempo_Utilizado_Atividade;
        private long m_Tempo_Utilizado_Acao;
        private int m_TempoPercorrido;
        private int m_Id_Empresa;
        private string m_CodExterno_Acomodacao;
        private string m_Cod_Isolamento;
        private string m_PrioritarioAtividade;
        private string m_Cod_Plus;
        private int m_Id_Setor;
        private string m_Nome_Setor;
        private string m_PendenciaFinanceira;
        #endregion

        #region Propriedades
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public string Nome_Acomodacao { get => m_Nome_Acomodacao; set => m_Nome_Acomodacao = value; }
        public string Nome_TipoAcaoAcomodacao { get => m_Nome_TipoAcaoAcomodacao; set => m_Nome_TipoAcaoAcomodacao = value; }
        public string Nome_Status { get => m_Nome_Status; set => m_Nome_Status = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public DateTime Dt_InicioSituacaoAcomodacao { get => m_dt_InicioSituacaoAcomodacao; set => m_dt_InicioSituacaoAcomodacao = value; }
        public DateTime Dt_InicioAtividadeAcomodacao { get => m_dt_InicioAtividadeAcomodacao; set => m_dt_InicioAtividadeAcomodacao = value; }
        public DateTime Dt_InicioAcaoAtividade { get => m_dt_InicioAcaoAtividade; set => m_dt_InicioAcaoAtividade = value; }
        public int Id_TipoAcaoAcomodacao { get => m_Id_TipoAcaoAcomodacao; set => m_Id_TipoAcaoAcomodacao = value; }
        public int SLAAcao { get => m_SLAAcao; set => m_SLAAcao = value; }
        public int SLAAtividade { get => m_SLAAtividade; set => m_SLAAtividade = value; }
        public int TempoPercorrido { get => m_TempoPercorrido; set => m_TempoPercorrido = value; }
        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string CodExterno_Acomodacao { get => m_CodExterno_Acomodacao; set => m_CodExterno_Acomodacao = value; }
        public string Cod_Isolamento { get => m_Cod_Isolamento; set => m_Cod_Isolamento = value; }
        public string PrioritarioAtividade { get => m_PrioritarioAtividade; set => m_PrioritarioAtividade = value; }
        public string Cod_Plus { get => m_Cod_Plus; set => m_Cod_Plus = value; }
        public int Id_Setor { get => m_Id_Setor; set => m_Id_Setor = value; }
        public string Nome_Setor { get => m_Nome_Setor; set => m_Nome_Setor = value; }
        public string PendenciaFinanceira { get => m_PendenciaFinanceira; set => m_PendenciaFinanceira = value; }
        public long Tempo_Utilizado_Acao { get => m_Tempo_Utilizado_Acao; set => m_Tempo_Utilizado_Acao = value; }
        public long Tempo_Utilizado_Atividade { get => m_Tempo_Utilizado_Atividade; set => m_Tempo_Utilizado_Atividade = value; }
        #endregion

        #region Construtor

        public ConsultarDashBoardSATO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_Acomodacao = string.Empty;
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;
            m_CodExterno_Acomodacao = string.Empty;
            m_Cod_Isolamento = string.Empty;
            m_PrioritarioAtividade = string.Empty;
            m_Cod_Plus = string.Empty;
            m_Nome_Setor = string.Empty;
            m_PendenciaFinanceira = string.Empty;
        }



        #endregion

        #region SQL
        public void ConsultarDashBoardSATOCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao,int IdTipoAtividadeAcomodacao, string connection, ref List<ConsultarDashBoardSATO> l_ListTO)
        {

            m_sql = " SELECT * ";
            m_sql += " FROM [vw_ConsultarDashBoardSA] ";
            m_sql += " WHERE (Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";

            if (IdTipoAtividadeAcomodacao != 0)
            {
                m_sql += " AND (Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            }

            m_sql += " AND(Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY Nome_TipoSituacaoAcomodacao,Nome_TipoAtividadeAcomodacao  ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            if (IdTipoAtividadeAcomodacao != 0)
            {
                command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividadeAcomodacao);
            }
            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        #endregion
    }
}
