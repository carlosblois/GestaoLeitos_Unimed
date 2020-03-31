using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarSetorPorIdEmpresaTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Empresa;

        private int m_Id_Setor;

        private string m_Nome_Setor;

        private string m_CodExterno_Setor;

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
        public int Id_Setor
        {
            get
            {
                return this.m_Id_Setor;
            }
            set
            {
                this.m_Id_Setor = value;
            }
        }
        public string Nome_Setor
        {
            get
            {
                return this.m_Nome_Setor;
            }
            set
            {
                this.m_Nome_Setor = value;
            }
        }
        public string CodExterno_Setor
        {
            get
            {
                return this.m_CodExterno_Setor;
            }
            set
            {
                this.m_CodExterno_Setor = value;
            }

        }


        #endregion

        #region Construtor

        public ConsultarSetorPorIdEmpresaTO()
        {
            m_Nome_Setor = string.Empty;
            m_CodExterno_Setor = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarSetorPorIdEmpresaTOCommand(int IdEmpresa, string connection, ref List<ConsultarSetorPorIdEmpresaTO> l_ListSetorTO)
        {

            m_sql = " SELECT Id_Setor, Nome_Setor, CodExterno_Setor,Id_Empresa ";
            m_sql += " FROM Setor AS S ";
            m_sql += " WHERE(Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY Nome_Setor ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSetorTO);
            Dados = null;
        }
        #endregion
    }
}
