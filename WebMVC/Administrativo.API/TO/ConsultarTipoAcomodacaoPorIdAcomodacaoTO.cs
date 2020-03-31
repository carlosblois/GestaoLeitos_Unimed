using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarTipoAcomodacaoPorIdAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Empresa;

        private int m_Id_TipoAcomodacao;

        private string m_Nome_TipoAcomodacao;

        private string m_CodExterno_TipoAcomodacao;

        private int m_Id_CaracteristicaAcomodacao;

        private string m_Nome_CaracteristicaAcomodacao;

        #endregion

        #region Propriedades

        public int Id_Empresa
        {
            get
            {
                return this.m_Id_Empresa;
            }
            set
            {
                this.m_Id_Empresa = value;
            }
        }
        public int Id_TipoAcomodacao
        {
            get
            {
                return this.m_Id_TipoAcomodacao;
            }
            set
            {
                this.m_Id_TipoAcomodacao = value;
            }
        }
        public string Nome_TipoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAcomodacao = value;
            }
        }
        public string CodExterno_TipoAcomodacao
        {
            get
            {
                return this.m_CodExterno_TipoAcomodacao;
            }
            set
            {
                this.m_CodExterno_TipoAcomodacao = value;
            }

        }
        public int Id_CaracteristicaAcomodacao
        {
            get
            {
                return this.m_Id_CaracteristicaAcomodacao;
            }
            set
            {
                this.m_Id_CaracteristicaAcomodacao = value;
            }

        }
        public string Nome_CaracteristicaAcomodacao
        {
            get
            {
                return this.m_Nome_CaracteristicaAcomodacao;
            }
            set
            {
                this.m_Nome_CaracteristicaAcomodacao = value;
            }

        }


        #endregion

        #region Construtor

        public ConsultarTipoAcomodacaoPorIdAcomodacaoTO()
        {
            m_Nome_TipoAcomodacao = string.Empty;
            m_CodExterno_TipoAcomodacao = string.Empty;
            m_Nome_CaracteristicaAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarTipoAcomodacaoPorIdAcomodacaoTOCommand(int IdEmpresa,int IdAcomodacao, string connection, ref List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> l_ListTipoAcomodacaoTO)
        {

            m_sql = " SELECT TA.Id_Empresa, TA.Id_TipoAcomodacao, TA.Nome_TipoAcomodacao, ";
            m_sql += " TA.CodExterno_TipoAcomodacao, TA.Id_CaracteristicaAcomodacao,  ";
            m_sql += " CA.Nome_CaracteristicaAcomodacao ";
            m_sql += " FROM   TipoAcomodacao AS TA INNER JOIN ";
            m_sql += "        CaracteristicaAcomodacao AS CA ON TA.Id_CaracteristicaAcomodacao = CA.Id_CaracteristicaAcomodacao INNER JOIN ";
            m_sql += "        Acomodacao AS A ON TA.Id_Empresa = A.Id_Empresa AND TA.Id_TipoAcomodacao = A.Id_TipoAcomodacao  ";
            m_sql += " WHERE (TA.Id_Empresa = @Id_Empresa) ";
            m_sql += " AND (A.Id_Acomodacao = @Id_Acomodacao) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_Acomodacao", IdAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTipoAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
