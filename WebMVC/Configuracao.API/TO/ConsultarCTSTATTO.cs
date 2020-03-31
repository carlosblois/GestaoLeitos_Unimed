using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarCTSTATTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Checklist;
        private string m_Nome_Checklist;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private int m_Id_TipoAcomodacao;
        private string m_Nome_TipoAcomodacao;
        private int m_Id_Empresa;
        private int m_Id_CheckTSTAT;
        private string m_CodExterno_TipoAcomodacao;


        #endregion

        #region Propriedades

        public int Id_Checklist
        {
            get
            {
                return this.m_Id_Checklist;
            }
            set
            {
                this.m_Id_Checklist = value;
            }
        }
        public string Nome_Checklist
        {
            get
            {
                return this.m_Nome_Checklist;
            }
            set
            {
                this.m_Nome_Checklist = value;
            }
        }

        public int Id_TipoSituacaoAcomodacao
        {
            get
            {
                return this.m_Id_TipoSituacaoAcomodacao;
            }
            set
            {
                this.m_Id_TipoSituacaoAcomodacao = value;
            }
        }
        public string Nome_TipoSituacaoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoSituacaoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoSituacaoAcomodacao = value;
            }
        }

        public int Id_TipoAtividadeAcomodacao
        {
            get
            {
                return this.m_Id_TipoAtividadeAcomodacao;
            }
            set
            {
                this.m_Id_TipoAtividadeAcomodacao = value;
            }
        }
        public string Nome_TipoAtividadeAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAtividadeAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAtividadeAcomodacao = value;
            }
        }

        public int Id_TipoAcomodacao
        {
            get
            {
                return this.m_Id_TipoAcomodacao;
            }
            set
            {
                this.m_Id_TipoAcomodacao = value;
            }
        }
        public string Nome_TipoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAcomodacao = value;
            }
        }

        public int Id_Empresa
        {
            get
            {
                return this.m_Id_Empresa;
            }
            set
            {
                this.m_Id_Empresa = value;
            }
        }

        public int Id_CheckTSTAT
        {
            get
            {
                return this.m_Id_CheckTSTAT;
            }
            set
            {
                this.m_Id_CheckTSTAT = value;
            }
        }

        public string CodExterno_TipoAcomodacao
        {
            get
            {
                return this.m_CodExterno_TipoAcomodacao;
            }
            set
            {
                this.m_CodExterno_TipoAcomodacao = value;
            }
        }
        #endregion

        #region Construtor

        public ConsultarCTSTATTO()
        {
            m_Nome_Checklist = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_TipoAcomodacao = string.Empty;
            m_CodExterno_TipoAcomodacao = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarCTSTATTOCommand(int IdEmpresa, string connection, ref List<ConsultarCTSTATTO> l_ListCTSTATTO)
        {
            m_sql = "  SELECT CTSTAT.Id_Checklist, C.Nome_Checklist, CTSTAT.Id_TipoSituacaoAcomodacao,  ";
            m_sql += "        TS.Nome_TipoSituacaoAcomodacao, CTSTAT.Id_TipoAtividadeAcomodacao,  ";
            m_sql += "        TA.Nome_TipoAtividadeAcomodacao, CTSTAT.Id_TipoAcomodacao,  ";
            m_sql += "        TAC.Nome_TipoAcomodacao, CTSTAT.Id_Empresa,  ";
            m_sql += "        CTSTAT.Id_CheckTSTAT, TAC.CodExterno_TipoAcomodacao ";
            m_sql += " FROM   ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao AS CTSTAT INNER JOIN ";
            m_sql += "        Checklist AS C ON CTSTAT.Id_Checklist = C.Id_Checklist INNER JOIN ";
            m_sql += "        TipoAcomodacao AS TAC ON CTSTAT.Id_Empresa = TAC.Id_Empresa AND CTSTAT.Id_TipoAcomodacao = TAC.Id_TipoAcomodacao INNER JOIN ";
            m_sql += "        TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON CTSTAT.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND CTSTAT.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoSituacaoAcomodacao AS TS ON TSTA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "        TipoAtividadeAcomodacao AS TA ON TSTA.Id_TipoAtividadeAcomodacao = TA.Id_TipoAtividadeAcomodacao ";
            m_sql += " WHERE(CTSTAT.Id_Empresa = @Id_Empresa) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListCTSTATTO);
            Dados = null;
        }

        #endregion
    }
}
