using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarDashBoardHeaderTO
    {
        string m_sql;

        #region Atributos
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private long m_Qtd;
        private long m_PERC;


        #endregion

        #region Propriedades
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public long Qtd { get => m_Qtd; set => m_Qtd = value; }
        public long  PERC { get => m_PERC; set => m_PERC = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }

        #endregion

        #region Construtor

        public ConsultarDashBoardHeaderTO()
        {
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarDashboardHeaderTOCommand(int IdEmpresa ,string connection, ref List<ConsultarDashBoardHeaderTO> l_ListTO)
        {

            m_sql = "  SELECT   * " ;
            m_sql += " FROM     [vw_ConsultarDashboardHeader] ";
            m_sql += " WHERE    (Id_Empresa = @Id_Empresa)";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            
            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        #endregion
    }
}
