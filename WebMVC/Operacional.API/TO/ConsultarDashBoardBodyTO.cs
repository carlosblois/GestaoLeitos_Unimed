using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarDashBoardBodyTO
    {
        string m_sql;

        #region Atributos
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private long m_QTD_POR_ATV;
        private long m_PER_POR_ATV;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private long m_QTD_POR_SIT;
        private long m_TEMPO_Utilizado;
        private long m_TEMPO_UtilizadoAt;
        private string m_FORASLA;
        private string m_MaiorTempo;



        #endregion

        #region Propriedades
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public long QTD_POR_ATV { get => m_QTD_POR_ATV; set => m_QTD_POR_ATV = value; }
        public long PER_POR_ATV { get => m_PER_POR_ATV; set => m_PER_POR_ATV = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public long QTD_POR_SIT { get => m_QTD_POR_SIT; set => m_QTD_POR_SIT = value; }
        public long TEMPO_Utilizado { get => m_TEMPO_Utilizado; set => m_TEMPO_Utilizado = value; }
        public long TEMPO_UtilizadoAt { get => m_TEMPO_UtilizadoAt; set => m_TEMPO_UtilizadoAt = value; }
        public string FORASLA { get => m_FORASLA; set => m_FORASLA = value; }
        public string MaiorTempo { get => m_MaiorTempo; set => m_MaiorTempo = value; }
   
        #endregion

        #region Construtor

        public ConsultarDashBoardBodyTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_FORASLA = string.Empty;
            m_MaiorTempo = string.Empty;

        }



        #endregion

        #region SQL
        public void ConsultarDashBoardBodyTOCommand(int IdEmpresa,string connection, ref List<ConsultarDashBoardBodyTO> l_ListTO)
        {


            m_sql = "  SELECT * ";
            m_sql += " FROM vw_ConsultarDashBoardBody ";
            m_sql += " WHERE (Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao, Nome_TipoSituacaoAcomodacao ";


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
