using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarCaracteristicaAcomodacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_CaracteristicaAcomodacao;

        private string m_Nome_CaracteristicaAcomodacao;


        #endregion

        #region Propriedades

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

        public ConsultarCaracteristicaAcomodacaoTO()
        {
            m_Nome_CaracteristicaAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarCaracteristicaAcomodacaoTOCommand(string connection, ref List<ConsultarCaracteristicaAcomodacaoTO> l_ListCaracteristicaAcomodacaoTO)
        {

            m_sql = " SELECT Id_CaracteristicaAcomodacao, Nome_CaracteristicaAcomodacao ";
            m_sql += " FROM CaracteristicaAcomodacao AS C ";
            m_sql += " ORDER BY Nome_CaracteristicaAcomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListCaracteristicaAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
