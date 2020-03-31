using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAtividadeAcaoTO
    {
        string m_sql;

        #region Atributos
        private string m_Nome_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAcaoAcomodacao;
        private string m_Nome_Status;
        private DateTime m_dt_InicioAcaoAtividade;
        private int m_Tempo_Minutos;
        private int m_Id_TipoAtividadeAcomodacao;
        private int m_Id_TipoAcaoAcomodacao;

        #endregion

        #region Propriedades
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int Tempo_Minutos { get => m_Tempo_Minutos; set => m_Tempo_Minutos = value; }
        public string Nome_TipoAcaoAcomodacao { get => m_Nome_TipoAcaoAcomodacao; set => m_Nome_TipoAcaoAcomodacao = value; }
        public string Nome_Status { get => m_Nome_Status; set => m_Nome_Status = value; }
        public DateTime Dt_InicioAcaoAtividade { get => m_dt_InicioAcaoAtividade; set => m_dt_InicioAcaoAtividade = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int Id_TipoAcaoAcomodacao { get => m_Id_TipoAcaoAcomodacao; set => m_Id_TipoAcaoAcomodacao = value; }
        #endregion

        #region Construtor
        public ConsultarAtividadeAcaoTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;

        }

        #endregion

        #region SQL


        public void ConsultarAtividadesAcaoTOCommand(int IdEmpresa, int IdSituacaoAcomodacao,  string connection, ref List<ConsultarAtividadeAcaoTO> l_ListAtividadeAcaoTO)
        {

            m_sql = "  SELECT Nome_TipoAtividadeAcomodacao, Nome_TipoAcaoAcomodacao, ";
            m_sql += "        Nome_Status, dt_InicioAcaoAtividade, Tempo_Minutos,Id_TipoAtividadeAcomodacao, Id_TipoAcaoAcomodacao ";
            m_sql += " FROM vw_DetalheAtividade ";
            m_sql += " WHERE(Id_SituacaoAcomodacao = @Id_SituacaoAcomodacao) AND(Id_Empresa = @Id_Empresa) ";
            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao, dt_InicioAcaoAtividade ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_SituacaoAcomodacao", IdSituacaoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAtividadeAcaoTO);
            Dados = null;
        }
        #endregion
    }
}
