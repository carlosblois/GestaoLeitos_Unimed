using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarTipoSituacaoAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_TipoSituacaoAcomodacao;

        private string m_Nome_TipoSituacaoAcomodacao;

        private int m_ordem;

        private string m_imagem;

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
        public int ordem
        {
            get
            {
                return this.m_ordem;
            }
            set
            {
                this.m_ordem = value;
            }
        }

        public string imagem
        {
            get
            {
                return this.m_imagem;
            }
            set
            {
                this.m_imagem = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarTipoSituacaoAcomodacaoTO()
        {
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarTipoSituacaoAcomodacaoTOCommand( string connection, ref List<ConsultarTipoSituacaoAcomodacaoTO> l_ListTipoSituacaoAcomodacaoTO)
        {
            m_sql = " SELECT Id_TipoSituacaoAcomodacao, Nome_TipoSituacaoAcomodacao, imagem, ordem ";
            m_sql += " FROM TipoSituacaoAcomodacao AS SA ";
            m_sql += " ORDER BY Nome_TipoSituacaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoSituacaoAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
