using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarSLAAtividadeTO
    {
        string m_sql;
        #region Atributos
        private int m_Id_SLA;

        private int m_Id_Empresa;

        private int m_Id_TipoSituacaoAcomodacao;

        private int m_Id_TipoAtividadeAcomodacao;

        private int m_TOT;

        #endregion

        #region Propriedades

        public int Id_SLA { get => m_Id_SLA; set => m_Id_SLA = value; }
        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int TOT { get => m_TOT; set => m_TOT = value; }
        #endregion

        #region Construtor

        public ConsultarSLAAtividadeTO()
        {
        }

        #endregion
        #region SQL

        public void ConsultarSLAPorIdEmpresaIdTipoSituacaoIdTipoAtividadeIdTipoAcomodacaoTOCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividade,int IdTipoAcomodacao, string connection, ref List<ConsultarSLAAtividadeTO> l_ListSLATO)
        {

            m_sql = "  SELECT Id_TipoSituacaoAcomodacao, Id_TipoAtividadeAcomodacao,Id_TipoAcomodacao, TOT, Id_Empresa";
            m_sql += " FROM vw_SLAAtividade ";
            m_sql += " WHERE(Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) AND(Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) AND(Id_Empresa = @Id_Empresa) AND Id_TipoAcomodacao = @Id_TipoAcomodacao";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividade);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_TipoAcomodacao", IdTipoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSLATO);
            Dados = null;
        }


        #endregion

    }
}
