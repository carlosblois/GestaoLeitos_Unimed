using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAtividadeAcoesDetTO
    {
        string m_sql;

        #region Atributos
        private string m_Nome_TipoAtividadeAcomodacao;
        private DateTime? m_SOLICITADO;
        private DateTime? m_ACEITE;
        private DateTime? m_CHECK_IN;
        private DateTime? m_SEMI_CHECK_OUT;
        private DateTime? m_CHECK_OUT;
        private long m_SLA;
        private int m_Id_TipoAtividadeAcomodacao;
        private int m_Id_AtividadeAcomodacao;
        private DateTime? m_dt_FimAtividadeAcomodacao;
        private string m_Nome_Usuario;
        private string m_Login_Usuario;
        private string m_Id_Usuario;

        #endregion

        #region Propriedades


        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public DateTime? SOLICITADO { get => m_SOLICITADO; set => m_SOLICITADO = value; }
        public DateTime? ACEITE { get => m_ACEITE; set => m_ACEITE = value; }
        public DateTime? CHECKIN { get => m_CHECK_IN; set => m_CHECK_IN = value; }
        public DateTime? SEMICHECKOUT { get => m_SEMI_CHECK_OUT; set => m_SEMI_CHECK_OUT = value; }
        public DateTime? CHECKOUT { get => m_CHECK_OUT; set => m_CHECK_OUT = value; }
        public long SLA { get => m_SLA; set => m_SLA = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int Id_AtividadeAcomodacao { get => m_Id_AtividadeAcomodacao; set => m_Id_AtividadeAcomodacao = value; }
        public DateTime? Dt_FimAtividadeAcomodacao { get => m_dt_FimAtividadeAcomodacao; set => m_dt_FimAtividadeAcomodacao = value; }
        public string Nome_Usuario { get => m_Nome_Usuario; set => m_Nome_Usuario = value; }
        public string Login_Usuario { get => m_Login_Usuario; set => m_Login_Usuario = value; }
        public string Id_Usuario { get => m_Id_Usuario; set => m_Id_Usuario = value; }
        #endregion

        #region Construtor
        public ConsultarAtividadeAcoesDetTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_Usuario = string.Empty;
            m_Login_Usuario = string.Empty;
            m_Id_Usuario = string.Empty;
        }

        #endregion

        #region SQL


        public void ConsultarAtividadesAcoesDetTOCommand(int idEmpresa, int idSituacao, string connection, ref List<ConsultarAtividadeAcoesDetTO> l_ListAtividadeAcaoTO)
        {


            m_sql = " SELECT Id_AtividadeAcomodacao,Id_TipoAtividadeAcomodacao,Nome_TipoAtividadeAcomodacao, ";
            m_sql += " upper(( ";
            m_sql += "      SELECT TOP 1 DT.Login_Usuario FROM vw_DetalheAtividade DT ";
            m_sql += "      WHERE DT.Id_TipoAcaoAcomodacao in (1, 2, 5) ";
            m_sql += "    AND DT.Id_AtividadeAcomodacao = piv.Id_AtividadeAcomodacao ";
            m_sql += "    ORDER BY DT.dt_InicioAcaoAtividade DESC ";
            m_sql += " )) AS Login_Usuario, ";
            m_sql += " upper((";
            m_sql += "    SELECT TOP 1 DT.Nome_Usuario FROM vw_DetalheAtividade DT ";
            m_sql += "    WHERE DT.Id_TipoAcaoAcomodacao in (1, 2, 5) ";
            m_sql += "    AND DT.Id_AtividadeAcomodacao = piv.Id_AtividadeAcomodacao";
            m_sql += "    ORDER BY DT.dt_InicioAcaoAtividade DESC";
            m_sql += ")) AS Nome_Usuario, (";
            m_sql += "    SELECT TOP 1 DT.Id_Usuario FROM vw_DetalheAtividade DT ";
            m_sql += "    WHERE DT.Id_TipoAcaoAcomodacao in (1, 2, 5) ";
            m_sql += "    AND DT.Id_AtividadeAcomodacao = piv.Id_AtividadeAcomodacao";
            m_sql += "    ORDER BY DT.dt_InicioAcaoAtividade DESC";
            m_sql += ") AS Id_Usuario, ";
            m_sql += " SOLICITADO, ACEITE, CHECKIN, SEMICHECKOUT, CHECKOUT, SLA,dt_FimAtividadeAcomodacao ";
            m_sql += " FROM ";
            m_sql += " ( ";
            m_sql += "   SELECT   DA.Id_AtividadeAcomodacao,DA.Id_SituacaoAcomodacao, DA.Id_TipoAtividadeAcomodacao, DA.Nome_TipoAtividadeAcomodacao, DA.Nome_Status, DA.dt_InicioAcaoAtividade,  S.TOT AS SLA , dt_FimAtividadeAcomodacao, Id_Usuario ";
            m_sql += "   FROM     vw_DetalheAtividade AS DA LEFT JOIN ";
            m_sql += "           vw_SLAAtividade AS S ON DA.Id_Empresa = S.Id_Empresa AND DA.Id_TipoSituacaoAcomodacao = S.Id_TipoSituacaoAcomodacao AND DA.Id_TipoAtividadeAcomodacao = S.Id_TipoAtividadeAcomodacao AND  DA.Id_TipoAcomodacao = S.Id_TipoAcomodacao";
            m_sql += "   WHERE   (DA.Id_SituacaoAcomodacao = @Id_SituacaoAcomodacao) AND(DA.Id_Empresa = @Id_Empresa) ";
            m_sql += "  ) d ";
            m_sql += "  PIVOT ";
            m_sql += "   ( ";
            m_sql += "     MAX(dt_InicioAcaoAtividade) ";
            m_sql += "     FOR Nome_Status in (SOLICITADO, ACEITE, CHECKIN, SEMICHECKOUT, CHECKOUT) ";
            m_sql += "     ) piv ";
            //m_sql += "   ORDER BY CHECKOUT DESC, SEMICHECKOUT DESC, CHECKIN DESC, ACEITE DESC, SOLICITADO DESC";
            m_sql += "   ORDER BY SOLICITADO DESC";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", idEmpresa);
            command.Parameters.AddWithValue("Id_SituacaoAcomodacao", idSituacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAtividadeAcaoTO);
            Dados = null;
        }
        #endregion
    }
}
