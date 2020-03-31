using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarTipoAtividadeAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_TipoAtividadeAcomodacao;

        private string m_Nome_TipoAtividadeAcomodacao;

        private int m_ordem;

        private string m_imagem;

        private string m_qrcode;



        #endregion

        #region Propriedades

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

        public string qrcode
        {
            get
            {
                return this.m_qrcode;
            }
            set
            {
                this.m_qrcode = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarTipoAtividadeAcomodacaoTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarTipoAtividadeAcomodacaoTOCommand(string connection, ref List<ConsultarTipoAtividadeAcomodacaoTO> l_ListTipoAtividadeAcomodacaoTO)
        {

            m_sql = " SELECT Id_TipoAtividadeAcomodacao, Nome_TipoAtividadeAcomodacao , imagem, ordem , qrcode";
            m_sql += " FROM TipoAtividadeAcomodacao AS AA ";
            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoAtividadeAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
