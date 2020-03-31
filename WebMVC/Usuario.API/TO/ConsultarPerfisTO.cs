using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class ConsultarPerfisTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Perfil;

        private String m_Nome_Perfil;

        #endregion

        #region Propriedades
        public int Id_Perfil
        {
            get
            {
                return this.m_Id_Perfil;
            }
            set
            {
                this.m_Id_Perfil = value;
            }
        }
        public String Nome_Perfil
        {
            get
            {
                return this.m_Nome_Perfil;
            }
            set
            {
                this.m_Nome_Perfil = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarPerfisTO()
        {
            m_Nome_Perfil = String.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarPerfisTOCommand(string connection, ref List<ConsultarPerfisTO> l_ListPerfilTO)
        {

            m_sql = " SELECT P.Id_Perfil, P.Nome_Perfil ";
            m_sql += " FROM   Perfil AS P ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListPerfilTO);
            Dados = null;
        }
       
        #endregion
    }
}
