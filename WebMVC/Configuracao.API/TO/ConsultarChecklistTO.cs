using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarChecklistTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Checklist;

        private string m_Nome_Checklist;


        #endregion

        #region Propriedades

        public int Id_Checklist
        {
            get
            {
                return this.m_Id_Checklist;
            }
            set
            {
                this.m_Id_Checklist = value;
            }
        }
        public string Nome_Checklist
        {
            get
            {
                return this.m_Nome_Checklist;
            }
            set
            {
                this.m_Nome_Checklist = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarChecklistTO()
        {
            m_Nome_Checklist = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarChecklistTOCommand(string connection, ref List<ConsultarChecklistTO> l_ListChecklistTO)
        {
            m_sql = " SELECT Id_Checklist, Nome_Checklist ";
            m_sql += " FROM Checklist AS C ";
            m_sql += " ORDER BY Nome_Checklist ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListChecklistTO);
            Dados = null;
        }
       
        #endregion
    }
}
