using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarChecklistDetalheTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Checklist;

        private string m_Nome_Checklist;

        private int m_Id_ItemChecklist;

        private string m_Nome_ItemChecklist;

        private int m_Id_CheckTSTAT;

        private string m_Cod_PermiteTotal;

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

        public int Id_ItemChecklist
        {
            get
            {
                return this.m_Id_ItemChecklist;
            }
            set
            {
                this.m_Id_ItemChecklist = value;
            }
        }
        public string Nome_ItemChecklist
        {
            get
            {
                return this.m_Nome_ItemChecklist;
            }
            set
            {
                this.m_Nome_ItemChecklist = value;
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

        public string Cod_PermiteTotal
        {
            get
            {
                return this.m_Cod_PermiteTotal;
            }
            set
            {
                this.m_Cod_PermiteTotal = value;
            }
        }
        
        #endregion

        #region Construtor

        public ConsultarChecklistDetalheTO()
        {
            m_Nome_Checklist = string.Empty;
            m_Nome_ItemChecklist = string.Empty;
            m_Cod_PermiteTotal = string.Empty;
        }

        #endregion

        #region SQL

        public void ConsultarChecklistDetalhePorTipoSituacaoPorTipoAtividadePorTipoAcomodacaoTOCommand(int Id_Empresa,int Id_TipoAcomodacao,
                                                                      int Id_TipoAtividadeAcomodacao,int Id_TipoSituacaoAcomodacao, 
                                                                      string connection, ref List<ConsultarChecklistDetalheTO> l_ListChecklistDetalheTO)
        {


            m_sql += " SELECT C.Id_Checklist, C.Nome_Checklist, IC.Id_ItemChecklist, IC.Nome_ItemChecklist, CTSTAT.Id_CheckTSTAT , ISNULL(FluCk.Cod_PermiteTotal, 'N') AS Cod_PermiteTotal ";
            m_sql += " FROM Checklist AS C INNER JOIN ";
            m_sql += "      ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao AS CTSTAT ON C.Id_Checklist = CTSTAT.Id_Checklist LEFT OUTER JOIN ";
            m_sql += "      Checklist_ItensChecklist AS CI INNER JOIN ";
            m_sql += "       ItensChecklist AS IC ON CI.Id_ItemChecklist = IC.Id_ItemChecklist ON C.Id_Checklist = CI.Id_Checklist ";
            m_sql += "      LEFT JOIN(select Id_Checklist, Id_TipoSituacaoAcomodacao, Id_ItemChecklist, min(Cod_PermiteTotal) as Cod_PermiteTotal from FluxoAutomaticoCheck ";
            m_sql += "                 GROUP BY Id_Checklist,  Id_TipoSituacaoAcomodacao, Id_ItemChecklist) FluCk ";
            m_sql += "       ON FluCk.Id_Checklist = CI.Id_Checklist AND FluCk.Id_TipoSituacaoAcomodacao = CTSTAT.Id_TipoSituacaoAcomodacao ";
            m_sql += "       AND FluCk.Id_ItemChecklist = CI.Id_ItemChecklist ";
            m_sql += "  WHERE(CTSTAT.Id_Empresa = @Id_Empresa) ";
            m_sql += "    AND(CTSTAT.Id_TipoAcomodacao = @Id_TipoAcomodacao) ";
            m_sql += "    AND(CTSTAT.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) ";
            m_sql += "    AND(CTSTAT.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " ORDER BY C.Nome_Checklist ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", Id_Empresa);
            command.Parameters.AddWithValue("Id_TipoAcomodacao", Id_TipoAcomodacao);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", Id_TipoAtividadeAcomodacao);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", Id_TipoSituacaoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListChecklistDetalheTO);
            Dados = null;
        }

        public void ConsultarChecklistDetalheTOCommand(int Id_Checklist, string connection, ref List<ConsultarChecklistDetalheTO> l_ListChecklistDetalheTO)
        {
            m_sql =  " SELECT C.Id_Checklist, C.Nome_Checklist, IC.Id_ItemChecklist, IC.Nome_ItemChecklist ";
            m_sql += " FROM   Checklist AS C LEFT OUTER JOIN ";
            m_sql += "        Checklist_ItensChecklist AS CI INNER JOIN ";
            m_sql += "        ItensChecklist AS IC ON CI.Id_ItemChecklist = IC.Id_ItemChecklist ON C.Id_Checklist = CI.Id_Checklist ";
            m_sql += " WHERE (C.Id_Checklist = @Id_Checklist) ";
            m_sql += " ORDER BY C.Nome_Checklist ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Checklist", Id_Checklist);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListChecklistDetalheTO);
            Dados = null;
        }
        #endregion
    }
}
