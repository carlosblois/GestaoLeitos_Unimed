using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static Operacional.API.Enum.ExpoEnum;

namespace Operacional.API.TO
{
    public class ConsultarQTDAtividadeTO
    {
        string m_sql;

        #region Atributos

        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_QTD;

        #endregion

        #region Propriedades

        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public int QTD { get => m_QTD; set => m_QTD = value; }

        #endregion

        #region Construtor

        public ConsultarQTDAtividadeTO()
        {
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarQTDAtividadeTOCommand(int IdEmpresa, int IdTipoAtividadeAcomodacao, int IdSetor, int IdCaracteristicaAcomodacao, string connection, ref List<ConsultarQTDAtividadeTO> l_ListAtividadeTO)
        {

            m_sql = "SELECT T.*, ";
            m_sql += "       (SELECT COUNT(1) ";
            m_sql += "        FROM( ";
            m_sql += "              SELECT  AA.Id_AtividadeAcomodacao, AA.Id_SituacaoAcomodacao, AA.Id_TipoSituacaoAcomodacao, AA.Id_TipoAtividadeAcomodacao, ";
            m_sql += "                      AA.dt_InicioAtividadeAcomodacao, AA.dt_FimAtividadeAcomodacao, AA.Id_UsuarioSolicitante, ";
            m_sql += "                      AC.Id_Empresa, AC.Id_Setor, T.Id_CaracteristicaAcomodacao ";
            m_sql += "              FROM    AtividadeAcomodacao AS AA INNER JOIN ";
            m_sql += "                      SituacaoAcomodacao AS S ON AA.Id_SituacaoAcomodacao = S.Id_SituacaoAcomodacao INNER JOIN ";
            m_sql += "                      Acomodacao AS AC ON S.Id_Acomodacao = AC.Id_Acomodacao INNER JOIN ";
            m_sql += "                      TipoAcomodacao AS T ON AC.Id_Empresa = T.Id_Empresa AND AC.Id_TipoAcomodacao = T.Id_TipoAcomodacao ";
            m_sql += "              WHERE(AC.Id_Empresa = @Id_Empresa) AND (AA.dt_FimAtividadeAcomodacao IS NULL) ";
            if (IdSetor != 0)
            {
                m_sql += "       AND (Id_Setor = @Id_Setor) ";
            };
            if (IdCaracteristicaAcomodacao != 0)
            {
                m_sql += "       AND (Id_CaracteristicaAcomodacao = @Id_CaracteristicaAcomodacao) ";
            };
            m_sql += "              )  A  ";
            m_sql += "        WHERE A.Id_TipoSituacaoAcomodacao = T.Id_TipoSituacaoAcomodacao   ";
            m_sql += "              AND A.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) AS QTD ";
            m_sql += "        FROM TipoSituacaoAcomodacao T ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividadeAcomodacao);
            if (IdSetor != 0)
            {
                command.Parameters.AddWithValue("Id_Setor", IdSetor);
            };
            if (IdCaracteristicaAcomodacao != 0)
            {
                command.Parameters.AddWithValue("Id_CaracteristicaAcomodacao", IdCaracteristicaAcomodacao);
            };
            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAtividadeTO);
            Dados = null;
        }
  
        #endregion
    }
}
