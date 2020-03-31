using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_TipoSituacaoAcomodacao;

        private int m_Id_TipoAtividadeAcomodacao;

        private string m_Nome_TipoSituacaoAcomodacao;

        private string m_Nome_TipoAtividadeAcomodacao;

        #endregion

        #region Propriedades

        public int Id_TipoSituacaoAcomodacao
        {
            get
            {
                return this.m_Id_TipoSituacaoAcomodacao;
            }
            set
            {
                this.m_Id_TipoSituacaoAcomodacao = value;
            }
        }
        public string Nome_TipoSituacaoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoSituacaoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoSituacaoAcomodacao = value;
            }
        }
        public int Id_TipoAtividadeAcomodacao
        {
            get
            {
                return this.m_Id_TipoAtividadeAcomodacao;
            }
            set
            {
                this.m_Id_TipoAtividadeAcomodacao = value;
            }
        }
        public string Nome_TipoAtividadeAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAtividadeAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAtividadeAcomodacao = value;
            }
        }
        #endregion

        #region Construtor

        public ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO()
        {
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarTipoSituacaoTipoAtividadeAcomodacaoTOCommand(string connection, ref List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO)
        {

            m_sql = " SELECT TS.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao,  ";
            m_sql += " TA.Id_TipoAtividadeAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";
            m_sql += " FROM   TipoAtividadeAcomodacao AS TA INNER JOIN ";
            m_sql += " TipoSituacao_TipoAtividadeAcomodacao AS TT ON TA.Id_TipoAtividadeAcomodacao = TT.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += " TipoSituacaoAcomodacao AS TS ON TT.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += " ORDER BY TS.Nome_TipoSituacaoAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            Dados = null;
        }

        public void ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorSituacaoTOCommand(int Id_TipoSituacaoAcomodacao,string connection, ref List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO)
        {
            m_sql = " SELECT TS.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao,  ";
            m_sql += " TA.Id_TipoAtividadeAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";
            m_sql += " FROM   TipoAtividadeAcomodacao AS TA INNER JOIN ";
            m_sql += " TipoSituacao_TipoAtividadeAcomodacao AS TT ON TA.Id_TipoAtividadeAcomodacao = TT.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += " TipoSituacaoAcomodacao AS TS ON TT.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += " WHERE(TT.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " ORDER BY TS.Nome_TipoSituacaoAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", Id_TipoSituacaoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            Dados = null;
        }

        public void ConsultarTipoSituacaoTipoAtividadeAcomodacaoPorAtividadeTOCommand(int Id_TipoAtividadeAcomodacao, string connection, ref List<ConsultarTipoSituacaoTipoAtividadeAcomodacaoTO> l_ListTipoSituacaoTipoAtividadeAcomodacaoTO)
        {
            m_sql = " SELECT TS.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao,  ";
            m_sql += " TA.Id_TipoAtividadeAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";
            m_sql += " FROM   TipoAtividadeAcomodacao AS TA INNER JOIN ";
            m_sql += " TipoSituacao_TipoAtividadeAcomodacao AS TT ON TA.Id_TipoAtividadeAcomodacao = TT.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += " TipoSituacaoAcomodacao AS TS ON TT.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += " WHERE(TT.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            m_sql += " ORDER BY TS.Nome_TipoSituacaoAcomodacao, TA.Nome_TipoAtividadeAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", Id_TipoAtividadeAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoSituacaoTipoAtividadeAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
