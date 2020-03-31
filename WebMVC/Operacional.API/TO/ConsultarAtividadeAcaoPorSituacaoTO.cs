using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAtividadeAcaoPorSituacaoTO
    {
        string m_sql;

        #region Atributos
        private int m_Id_AtividadeAcomodacao;
        private int m_Id_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;


        #endregion

        #region Propriedades
        public int Id_AtividadeAcomodacao { get => m_Id_AtividadeAcomodacao; set => m_Id_AtividadeAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        #endregion

        #region Construtor
        public ConsultarAtividadeAcaoPorSituacaoTO()
        {

        }

        #endregion

        #region SQL


        public void ConsultarAtividadesAcaoPorSituacaoTOCommand(int IdSituacaoAcomodacao, string connection, ref List<ConsultarAtividadeAcaoPorSituacaoTO> l_ListAtividadeAcaoTO)
        {

            m_sql = " SELECT Id_AtividadeAcomodacao , Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao";
            m_sql += " FROM AtividadeAcomodacao ";
            m_sql += " WHERE(Id_SituacaoAcomodacao = @Id_SituacaoAcomodacao) ";



            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_SituacaoAcomodacao", IdSituacaoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAtividadeAcaoTO);
            Dados = null;
        }
        #endregion
    }
}
