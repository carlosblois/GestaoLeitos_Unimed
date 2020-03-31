using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarLogAcessoPorAtividadeTO
    {
        string m_sql;

        #region Atributos
        private int m_TotalUsuarios;
        private int m_Id_TipoAtividadeAcomodacao;


        #endregion

        #region Propriedades
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int TotalUsuarios { get => m_TotalUsuarios; set => m_TotalUsuarios = value; }
        #endregion

        #region Construtor
        public ConsultarLogAcessoPorAtividadeTO()
        {

        }

        #endregion

        #region SQL


        public void ConsultarLogAcessoPorAtividadeTOCommand(string connection, ref List<ConsultarLogAcessoPorAtividadeTO> l_ListTO)
        {

            m_sql = "  SELECT * ";
            m_sql += " FROM vw_ListarLogAcessoPorAtividade ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }
        #endregion
    }
}
