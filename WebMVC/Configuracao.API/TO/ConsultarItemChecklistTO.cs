using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarItemChecklistTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_ItemChecklist;

        private string m_Nome_ItemChecklist;


        #endregion

        #region Propriedades

        public int Id_ItemChecklist
        {
            get
            {
                return this.m_Id_ItemChecklist;
            }
            set
            {
                this.m_Id_ItemChecklist = value;
            }
        }
        public string Nome_ItemChecklist
        {
            get
            {
                return this.m_Nome_ItemChecklist;
            }
            set
            {
                this.m_Nome_ItemChecklist = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarItemChecklistTO()
        {
            m_Nome_ItemChecklist = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarItemChecklistTOCommand(string connection, ref List<ConsultarItemChecklistTO> l_ListItemChecklistTO)
        {
            m_sql = " SELECT Id_ItemChecklist, Nome_ItemChecklist ";
            m_sql += " FROM ItensChecklist AS C ";
            m_sql += " ORDER BY Nome_ItemChecklist ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListItemChecklistTO);
            Dados = null;
        }

        #endregion
    }
}
