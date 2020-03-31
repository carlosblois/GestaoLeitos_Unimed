using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Empresa;
        private string m_Nome_Empresa;
        private int m_Id_Setor;
        private string m_Nome_Setor;
        private int m_Id_Acomodacao;
        private string m_Nome_Acomodacao;
        private int m_Id_TipoAcomodacao;
        private string m_Nome_TipoAcomodacao;
        private int m_Id_CaracteristicaAcomodacao;
        private string m_Nome_CaracteristicaAcomodacao;
        private string m_CodExterno_Acomodacao;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_SituacaoAcomodacao;

        #endregion

        #region Propriedades

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public int Id_Setor { get => m_Id_Setor; set => m_Id_Setor = value; }
        public string Nome_Setor { get => m_Nome_Setor; set => m_Nome_Setor = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public string Nome_Acomodacao { get => m_Nome_Acomodacao; set => m_Nome_Acomodacao = value; }
        public int Id_TipoAcomodacao { get => m_Id_TipoAcomodacao; set => m_Id_TipoAcomodacao = value; }
        public string Nome_TipoAcomodacao { get => m_Nome_TipoAcomodacao; set => m_Nome_TipoAcomodacao = value; }
        public int Id_CaracteristicaAcomodacao { get => m_Id_CaracteristicaAcomodacao; set => m_Id_CaracteristicaAcomodacao = value; }
        public string CodExterno_Acomodacao { get => m_CodExterno_Acomodacao; set => m_CodExterno_Acomodacao = value; }
        public string Nome_CaracteristicaAcomodacao { get => m_Nome_CaracteristicaAcomodacao; set => m_Nome_CaracteristicaAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_SituacaoAcomodacao { get => m_Id_SituacaoAcomodacao; set => m_Id_SituacaoAcomodacao = value; }

        #endregion

        #region Construtor

        public ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO()
        {
            m_Nome_Empresa = string.Empty;
            m_Nome_Setor = string.Empty;
            m_Nome_Acomodacao = string.Empty;
            m_Nome_TipoAcomodacao = string.Empty;
            m_Nome_CaracteristicaAcomodacao = string.Empty;
            m_CodExterno_Acomodacao = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarAcomodacaoPorIdEmpresaIdSituacaoTOCommand(int IdEmpresa, int IdSituacao, string connection, ref List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO> l_ListAcomodacaoTO)
        {

            m_sql += m_sql = "  SELECT A.Id_Empresa, E.Nome_Empresa, A.Id_Setor, S.Nome_Setor, A.Id_Acomodacao, ";
            m_sql += "                 A.Nome_Acomodacao, A.Id_TipoAcomodacao, TA.Nome_TipoAcomodacao, C.Id_CaracteristicaAcomodacao, ";
            m_sql += "                 C.Nome_CaracteristicaAcomodacao, A.CodExterno_Acomodacao, TS.Id_TipoSituacaoAcomodacao, ";
            m_sql += "                 TS.Nome_TipoSituacaoAcomodacao, SA.Id_SituacaoAcomodacao ";
            m_sql += "          FROM   Acomodacao AS A INNER JOIN ";
            m_sql += "                 TipoAcomodacao AS TA ON A.Id_Empresa = TA.Id_Empresa AND A.Id_TipoAcomodacao = TA.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "                 CaracteristicaAcomodacao AS C ON TA.Id_CaracteristicaAcomodacao = C.Id_CaracteristicaAcomodacao INNER JOIN ";
            m_sql += "                 Setor AS S ON A.Id_Empresa = S.Id_Empresa AND A.Id_Setor = S.Id_Setor INNER JOIN ";
            m_sql += "                 Empresa AS E ON S.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "                 SituacaoAcomodacao AS SA ON A.Id_Acomodacao = SA.Id_Acomodacao INNER JOIN ";
            m_sql += "                 TipoSituacaoAcomodacao AS TS ON SA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += "          WHERE(A.Id_Empresa = @Id_Empresa) AND (SA.dt_FimSituacaoAcomodacao IS NULL) ";

            if (IdSituacao > 0)
            {
                m_sql += " AND (TS.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            }

            m_sql += "          ORDER BY E.Nome_Empresa, S.Nome_Setor, A.Nome_Acomodacao ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            if (IdSituacao > 0)
            {
                command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdSituacao);
            }

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
