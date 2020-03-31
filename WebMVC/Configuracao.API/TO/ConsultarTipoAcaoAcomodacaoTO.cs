using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarTipoAcaoAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_TipoAcaoAcomodacao;

        private string m_Nome_TipoAcaoAcomodacao;

        private string m_Nome_Status;

        private string m_imagem;

        #endregion

        #region Propriedades

        public int Id_TipoAcaoAcomodacao
        {
            get
            {
                return this.m_Id_TipoAcaoAcomodacao;
            }
            set
            {
                this.m_Id_TipoAcaoAcomodacao = value;
            }
        }
        public string Nome_TipoAcaoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAcaoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAcaoAcomodacao = value;
            }
        }
        public string Nome_Status
        {
            get
            {
                return this.m_Nome_Status;
            }
            set
            {
                this.m_Nome_Status = value;
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

        public ConsultarTipoAcaoAcomodacaoTO()
        {
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarTipoAcaoAcomodacaoTOCommand(string connection, ref List<ConsultarTipoAcaoAcomodacaoTO> l_ListTipoAcaoAcomodacaoTO)
        {
            m_sql = " SELECT Id_TipoAcaoAcomodacao, Nome_TipoAcaoAcomodacao, Nome_Status, imagem ";
            m_sql += " FROM TipoAcaoAcomodacao AS AA ";
            m_sql += " ORDER BY Nome_TipoAcaoAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoAcaoAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
