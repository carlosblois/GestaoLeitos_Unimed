using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarSituacaoTO
    {
        string m_sql;

        #region Atributos

        private int m_Id_SituacaoAcomodacao;
        private int m_Id_Acomodacao;
        private int m_Id_TipoSituacaoAcomodacao;
        private DateTime m_dt_InicioSituacaoAcomodacao;
        private DateTime? m_dt_FimSituacaoAcomodacao;
        private string m_cod_NumAtendimento;
        private int m_Id_SLA;
        private string m_Cod_Prioritario;
        private string m_Nome_TipoSituacaoAcomodacao;
        private string m_Alta_Administrativa;


        #endregion

        #region Propriedades
        public DateTime Dt_InicioSituacaoAcomodacao { get => m_dt_InicioSituacaoAcomodacao; set => m_dt_InicioSituacaoAcomodacao = value; }
        public string Cod_Prioritario { get => m_Cod_Prioritario; set => m_Cod_Prioritario = value; }
        public int Id_SituacaoAcomodacao { get => m_Id_SituacaoAcomodacao; set => m_Id_SituacaoAcomodacao = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public DateTime? Dt_FimSituacaoAcomodacao { get => m_dt_FimSituacaoAcomodacao; set => m_dt_FimSituacaoAcomodacao = value; }
        public string Cod_NumAtendimento { get => m_cod_NumAtendimento; set => m_cod_NumAtendimento = value; }
        public int Id_SLA { get => m_Id_SLA; set => m_Id_SLA = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public string Alta_Administrativa { get => m_Alta_Administrativa; set => m_Alta_Administrativa = value; }
        #endregion

        #region Construtor

        public ConsultarSituacaoTO()
        {

            m_cod_NumAtendimento = string.Empty;
            m_Cod_Prioritario = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Alta_Administrativa = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarSituacaoPorCodExternoTOCommand(string CodExterno,  string connection, ref List<ConsultarSituacaoTO> l_ListSituacaoTO)
        {

            m_sql = "  SELECT S.Id_SituacaoAcomodacao, S.Id_Acomodacao, S.Id_TipoSituacaoAcomodacao, ";
            m_sql += "        S.dt_InicioSituacaoAcomodacao, S.dt_FimSituacaoAcomodacao, ";
            m_sql += "        S.cod_NumAtendimento, S.Id_SLA, S.Cod_Prioritario, T.Nome_TipoSituacaoAcomodacao, S.Alta_Administrativa ";
            m_sql += " FROM SituacaoAcomodacao AS S INNER JOIN";
            m_sql += "      Acomodacao AS A ON S.Id_Acomodacao = A.Id_Acomodacao INNER JOIN";
            m_sql += "      TipoSituacaoAcomodacao AS T ON S.Id_TipoSituacaoAcomodacao = T.Id_TipoSituacaoAcomodacao";
            m_sql += " WHERE(A.CodExterno_Acomodacao = @CodExterno) AND(S.dt_FimSituacaoAcomodacao IS NULL) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("CodExterno", CodExterno);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListSituacaoTO);
            Dados = null;
        }

        #endregion
    }
}
