using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarSLATO
    {
        string m_sql;
        #region Atributos
        private int? m_Id_SLA;

        private int m_Id_Empresa;

        private string m_Nome_Empresa;

        private int m_Id_TipoSituacaoAcomodacao;

        private string m_Nome_TipoSituacaoAcomodacao;

        private int m_Id_TipoAtividadeAcomodacao;

        private string m_Nome_TipoAtividadeAcomodacao;

        private int m_Id_TipoAcaoAcomodacao;

        private string m_Nome_TipoAcaoAcomodacao;

        private int m_Versao_SLA;

        private int m_Tempo_Minutos;

        private int m_Id_TipoAcomodacao;

        private string m_Nome_TipoAcomodacao;

        private string m_cod_enabled;

        #endregion

        #region Propriedades

        public int? Id_SLA { get => m_Id_SLA; set => m_Id_SLA = value; }
        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Versao_SLA { get => m_Versao_SLA; set => m_Versao_SLA = value; }
        public int Tempo_Minutos { get => m_Tempo_Minutos; set => m_Tempo_Minutos = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int Id_TipoAcaoAcomodacao { get => m_Id_TipoAcaoAcomodacao; set => m_Id_TipoAcaoAcomodacao = value; }
        public string Nome_TipoAcaoAcomodacao { get => m_Nome_TipoAcaoAcomodacao; set => m_Nome_TipoAcaoAcomodacao = value; }
        public int Id_TipoAcomodacao { get => m_Id_TipoAcomodacao; set => m_Id_TipoAcomodacao = value; }
        public string Nome_TipoAcomodacao { get => m_Nome_TipoAcomodacao; set => m_Nome_TipoAcomodacao = value; }
        public string Cod_enabled { get => m_cod_enabled; set => m_cod_enabled = value; }
        #endregion

        #region Construtor

        public ConsultarSLATO()
        {
            m_Nome_Empresa = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_TipoAcomodacao= string.Empty;
            m_cod_enabled = string.Empty;
        }

        #endregion
        #region SQL



        public void ConsultarSLAPorIdEmpresaTOCommand(int Id_Empresa, string connection, ref List<ConsultarSLATO> l_ListSLATO)
        {

            m_sql = " SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, E.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao, TA.Nome_TipoAcaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TA_1.Nome_TipoAcomodacao, t2.cod_enabled ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLA ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLA AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA AND t1.Id_TipoAtividadeAcomodacao = t2.Id_TipoAtividadeAcomodacao AND t1.Id_TipoAcaoAcomodacao = t2.Id_TipoAcaoAcomodacao AND t1.Id_TipoAcomodacao = t2.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "         Empresa AS E ON t2.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TS ON t2.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TA ON t2.Id_TipoAcaoAcomodacao = TA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAA ON t2.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao AS TA_1 ON t2.Id_Empresa = TA_1.Id_Empresa AND t2.Id_TipoAcomodacao = TA_1.Id_TipoAcomodacao            ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", Id_Empresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }

        public void ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoTOCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividade, int IdTipoAcao, string connection, ref List<ConsultarSLATO> l_ListSLATO)
        {

            m_sql = " SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, E.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao, TA.Nome_TipoAcaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TA_1.Nome_TipoAcomodacao, t2.cod_enabled ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLA ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLA AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA AND t1.Id_TipoAtividadeAcomodacao = t2.Id_TipoAtividadeAcomodacao AND t1.Id_TipoAcaoAcomodacao = t2.Id_TipoAcaoAcomodacao AND t1.Id_TipoAcomodacao = t2.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "         Empresa AS E ON t2.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TS ON t2.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TA ON t2.Id_TipoAcaoAcomodacao = TA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAA ON t2.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao AS TA_1 ON t2.Id_Empresa = TA_1.Id_Empresa AND t2.Id_TipoAcomodacao = TA_1.Id_TipoAcomodacao            ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND (t2.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAcaoAcomodacao = @Id_TipoAcaoAcomodacao) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividade);
            command.Parameters.AddWithValue("Id_TipoAcaoAcomodacao", IdTipoAcao);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }

        public void ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividade, int IdTipoAcao,int IdTipoAcomodacao, string connection, ref List<ConsultarSLATO> l_ListSLATO)
        {

            m_sql = " SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, E.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao, TA.Nome_TipoAcaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TA_1.Nome_TipoAcomodacao, t2.cod_enabled ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLA ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLA AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA AND t1.Id_TipoAtividadeAcomodacao = t2.Id_TipoAtividadeAcomodacao AND t1.Id_TipoAcaoAcomodacao = t2.Id_TipoAcaoAcomodacao AND t1.Id_TipoAcomodacao = t2.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "         Empresa AS E ON t2.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TS ON t2.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TA ON t2.Id_TipoAcaoAcomodacao = TA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAA ON t2.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao AS TA_1 ON t2.Id_Empresa = TA_1.Id_Empresa AND t2.Id_TipoAcomodacao = TA_1.Id_TipoAcomodacao            ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND (t2.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAcaoAcomodacao = @Id_TipoAcaoAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAcomodacao = @Id_TipoAcomodacao) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividade);
            command.Parameters.AddWithValue("Id_TipoAcaoAcomodacao", IdTipoAcao);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_TipoAcomodacao", IdTipoAcomodacao);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }

        public void ListarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcaoIdTipoAcomodacaoTOCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividade, int IdTipoAcao, int IdTipoAcomodacao, string connection, ref List<ConsultarSLATO> l_ListSLATO)
        {

            m_sql = " SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, E.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao, TA.Nome_TipoAcaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TA_1.Nome_TipoAcomodacao, t2.cod_enabled ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLA ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLA AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA AND t1.Id_TipoAtividadeAcomodacao = t2.Id_TipoAtividadeAcomodacao AND t1.Id_TipoAcaoAcomodacao = t2.Id_TipoAcaoAcomodacao AND t1.Id_TipoAcomodacao = t2.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "         Empresa AS E ON t2.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TS ON t2.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TA ON t2.Id_TipoAcaoAcomodacao = TA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAA ON t2.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao AS TA_1 ON t2.Id_Empresa = TA_1.Id_Empresa AND t2.Id_TipoAcomodacao = TA_1.Id_TipoAcomodacao            ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND (t2.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND (t2.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            if (IdTipoAcao > 0)
            {
                m_sql += " AND (t2.Id_TipoAcaoAcomodacao = @Id_TipoAcaoAcomodacao) ";
            }
            if (IdTipoAcomodacao > 0)
            {
                m_sql += " AND (t2.Id_TipoAcomodacao = @Id_TipoAcomodacao) ";
            }
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividade);
            if (IdTipoAcao > 0)
            {
                command.Parameters.AddWithValue("Id_TipoAcaoAcomodacao", IdTipoAcao);
            }
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            if (IdTipoAcomodacao > 0)
            {
                command.Parameters.AddWithValue("Id_TipoAcomodacao", IdTipoAcomodacao);
            }
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }

        public void ConsultarSLAPorIdTOCommand(int IdEmpresa, int IdSla, string connection, ref List<ConsultarSLATO> l_ListSLATO)
        {

            m_sql = " SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, E.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, t2.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, t2.Id_TipoAcaoAcomodacao, TA.Nome_TipoAcaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TA_1.Nome_TipoAcomodacao, t2.cod_enabled ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLA ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao, Id_TipoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLA AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA AND t1.Id_TipoAtividadeAcomodacao = t2.Id_TipoAtividadeAcomodacao AND t1.Id_TipoAcaoAcomodacao = t2.Id_TipoAcaoAcomodacao AND t1.Id_TipoAcomodacao = t2.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "         Empresa AS E ON t2.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TS ON t2.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TA ON t2.Id_TipoAcaoAcomodacao = TA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAA ON t2.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao AS TA_1 ON t2.Id_Empresa = TA_1.Id_Empresa AND t2.Id_TipoAcomodacao = TA_1.Id_TipoAcomodacao            ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND (t2.Id_SLA = @Id_SLA) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_SLA ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_SLA", IdSla);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }

        #endregion

    }
}
