using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarSLASituacaoTO
    {
        string m_sql;
        #region Atributos
                
        private int m_Id_SLA;

        private int m_Id_Empresa;

        private string m_Nome_Empresa;

        private int m_Id_TipoSituacaoAcomodacao;

        private string m_Nome_TipoSituacaoAcomodacao;

        private int m_Versao_SLA;

        private int m_Tempo_Minutos;

        private int m_Id_TipoAcomodacao;

        private string m_Nome_TipoAcomodacao;

        #endregion

        #region Propriedades

        public int Id_SLA { get => m_Id_SLA; set => m_Id_SLA = value; }
        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Versao_SLA { get => m_Versao_SLA; set => m_Versao_SLA = value; }
        public int Tempo_Minutos { get => m_Tempo_Minutos; set => m_Tempo_Minutos = value; }
        public int Id_TipoAcomodacao { get => m_Id_TipoAcomodacao; set => m_Id_TipoAcomodacao = value; }
        public string Nome_TipoAcomodacao { get => m_Nome_TipoAcomodacao; set => m_Nome_TipoAcomodacao = value; }
        #endregion

        #region Construtor

        public ConsultarSLASituacaoTO()
        {
            m_Nome_Empresa = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarSLASituacaoTOCommand(string connection, ref List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO)
        {

            m_sql = "  SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, Empresa.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TipoSituacaoAcomodacao.Nome_TipoSituacaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TipoAcomodacao.Nome_TipoAcomodacao ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  vw_SLASituacao ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLASituacao AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA INNER JOIN ";
            m_sql += "         Empresa ON t2.Id_Empresa = Empresa.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao ON t2.Id_TipoSituacaoAcomodacao = TipoSituacaoAcomodacao.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao ON t2.Id_TipoAcomodacao = TipoAcomodacao.Id_TipoAcomodacao AND t2.Id_Empresa = TipoAcomodacao.Id_Empresa ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLASituacaoTO);
            Dados = null;
        }

        public void ConsultarSLASituacaoPorIdEmpresaTOCommand(int Id_Empresa, string connection, ref List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO)
        {

            m_sql = "  SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, Empresa.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TipoSituacaoAcomodacao.Nome_TipoSituacaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TipoAcomodacao.Nome_TipoAcomodacao ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  SLASituacao ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLASituacao AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA INNER JOIN ";
            m_sql += "         Empresa ON t2.Id_Empresa = Empresa.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao ON t2.Id_TipoSituacaoAcomodacao = TipoSituacaoAcomodacao.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao ON t2.Id_TipoAcomodacao = TipoAcomodacao.Id_TipoAcomodacao AND t2.Id_Empresa = TipoAcomodacao.Id_Empresa ";
            m_sql += " WHERE(t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", Id_Empresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLASituacaoTO);
            Dados = null;
        }

        public void ConsultarSLASituacaoPorIdTipoSituacaoTOCommand(int Id_Empresa, int Id_TipoSituacaoAcomodacao, string connection, ref List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO)
        {

            m_sql = "  SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, Empresa.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TipoSituacaoAcomodacao.Nome_TipoSituacaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TipoAcomodacao.Nome_TipoAcomodacao ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  vw_SLASituacao ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLASituacao AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA INNER JOIN ";
            m_sql += "         Empresa ON t2.Id_Empresa = Empresa.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao ON t2.Id_TipoSituacaoAcomodacao = TipoSituacaoAcomodacao.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao ON t2.Id_TipoAcomodacao = TipoAcomodacao.Id_TipoAcomodacao AND t2.Id_Empresa = TipoAcomodacao.Id_Empresa ";
            m_sql += " WHERE(t2.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND  (t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", Id_TipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_Empresa", Id_Empresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLASituacaoTO);
            Dados = null;
        }

        public void ConsultarSLASituacaoPorIdTipoSituacaoIdTipoAcomodacaoTOCommand(int Id_Empresa, int Id_TipoSituacaoAcomodacao,int Id_TipoAcomodacao, string connection, ref List<ConsultarSLASituacaoTO> l_ListSLASituacaoTO)
        {

            m_sql = "  SELECT DISTINCT t2.Id_SLA, t2.Id_Empresa, Empresa.Nome_Empresa, t2.Id_TipoSituacaoAcomodacao, TipoSituacaoAcomodacao.Nome_TipoSituacaoAcomodacao, t2.Versao_SLA, t2.Tempo_Minutos, t2.Id_TipoAcomodacao, TipoAcomodacao.Nome_TipoAcomodacao ";
            m_sql += " FROM(SELECT Id_Empresa, Id_TipoSituacaoAcomodacao, MAX(Versao_SLA) AS Version ";
            m_sql += "         FROM  vw_SLASituacao ";
            m_sql += "         GROUP BY Id_Empresa, Id_TipoSituacaoAcomodacao) AS t1 INNER JOIN ";
            m_sql += "         SLASituacao AS t2 ON t1.Id_Empresa = t2.Id_Empresa AND t1.Id_TipoSituacaoAcomodacao = t2.Id_TipoSituacaoAcomodacao AND t1.Version = t2.Versao_SLA INNER JOIN ";
            m_sql += "         Empresa ON t2.Id_Empresa = Empresa.Id_Empresa INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao ON t2.Id_TipoSituacaoAcomodacao = TipoSituacaoAcomodacao.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcomodacao ON t2.Id_TipoAcomodacao = TipoAcomodacao.Id_TipoAcomodacao AND t2.Id_Empresa = TipoAcomodacao.Id_Empresa ";
            m_sql += " WHERE(t2.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND  (t2.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND  (t2.Id_TipoAcomodacao = @Id_TipoAcomodacao) ";
            m_sql += " ORDER BY t2.Id_Empresa, t2.Id_TipoSituacaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAcomodacao", Id_TipoAcomodacao);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", Id_TipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_Empresa", Id_Empresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLASituacaoTO);
            Dados = null;
        }

        #endregion
    }
}
