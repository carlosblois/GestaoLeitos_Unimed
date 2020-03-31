using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarFluxoAutomaticoCheckTO
    {
        string m_sql;
        #region Atributos
        private int m_Id_Checklist;
        private string m_Nome_Checklist;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private int m_Id_ItemChecklist;
        private string m_Nome_ItemChecklist;
        private string m_Cod_Resposta;
        private string m_Cod_PermiteTotal;


        #endregion

        #region Propriedades

        public int Id_Checklist { get => m_Id_Checklist; set => m_Id_Checklist = value; }
        public string Nome_Checklist { get => m_Nome_Checklist; set => m_Nome_Checklist = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int Id_ItemChecklist { get => m_Id_ItemChecklist; set => m_Id_ItemChecklist = value; }
        public string Nome_ItemChecklist { get => m_Nome_ItemChecklist; set => m_Nome_ItemChecklist = value; }
        public string Cod_Resposta { get => m_Cod_Resposta; set => m_Cod_Resposta = value; }
        public string Cod_PermiteTotal { get => m_Cod_PermiteTotal; set => m_Cod_PermiteTotal = value; }

        #endregion

        #region Construtor

        public ConsultarFluxoAutomaticoCheckTO()
        {
            Nome_Checklist = string.Empty;
            Nome_TipoSituacaoAcomodacao = string.Empty;
            Nome_TipoAtividadeAcomodacao = string.Empty;
            Nome_ItemChecklist = string.Empty;
            Cod_Resposta = string.Empty;
            Cod_PermiteTotal = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarFluxoAutomaticoCheckTOCommand(string connection,
                                                           ref List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO)
        {


            m_sql = " SELECT F.Id_Checklist, C.Nome_Checklist, F.Id_TipoSituacaoAcomodacao, TSA.Nome_TipoSituacaoAcomodacao, F.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, F.Id_ItemChecklist, IC.Nome_ItemChecklist, F.Cod_Resposta, F.Cod_PermiteTotal ";
            m_sql += " FROM  FluxoAutomaticoCheck AS F INNER JOIN ";
            m_sql += "      Checklist_ItensChecklist AS CI ON F.Id_Checklist = CI.Id_Checklist AND F.Id_ItemChecklist = CI.Id_ItemChecklist INNER JOIN ";
            m_sql += "      Checklist AS C ON CI.Id_Checklist = C.Id_Checklist INNER JOIN ";
            m_sql += "      ItensChecklist AS IC ON CI.Id_ItemChecklist = IC.Id_ItemChecklist INNER JOIN ";
            m_sql += "      TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON F.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "      TipoSituacaoAcomodacao AS TSA ON TSTA.Id_TipoSituacaoAcomodacao = TSA.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "      TipoAtividadeAcomodacao AS TAA ON TSTA.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        public void ConsultarFluxoAutomaticoCheckPorSituacaoTOCommand(
                                                int IdTipoSituacaoAcomodacao,
                                                string connection,
                                                ref List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO)
        {
            m_sql = " SELECT F.Id_Checklist, C.Nome_Checklist, F.Id_TipoSituacaoAcomodacao, TSA.Nome_TipoSituacaoAcomodacao, F.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, F.Id_ItemChecklist, IC.Nome_ItemChecklist, F.Cod_Resposta, F.Cod_PermiteTotal ";
            m_sql += " FROM  FluxoAutomaticoCheck AS F INNER JOIN ";
            m_sql += "      Checklist_ItensChecklist AS CI ON F.Id_Checklist = CI.Id_Checklist AND F.Id_ItemChecklist = CI.Id_ItemChecklist INNER JOIN ";
            m_sql += "      Checklist AS C ON CI.Id_Checklist = C.Id_Checklist INNER JOIN ";
            m_sql += "      ItensChecklist AS IC ON CI.Id_ItemChecklist = IC.Id_ItemChecklist INNER JOIN ";
            m_sql += "      TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON F.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "      TipoSituacaoAcomodacao AS TSA ON TSTA.Id_TipoSituacaoAcomodacao = TSA.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "      TipoAtividadeAcomodacao AS TAA ON TSTA.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao ";

            m_sql += " WHERE(F.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        public void ConsultarFluxoAutomaticoCheckPorSituacaoIdChecklistTOCommand(
                                        int IdTipoSituacaoAcomodacao,
                                        int IdChecklist,
                                        string connection,
                                        ref List<ConsultarFluxoAutomaticoCheckTO> l_ListFluxoTO)
        {
            m_sql = " SELECT F.Id_Checklist, C.Nome_Checklist, F.Id_TipoSituacaoAcomodacao, TSA.Nome_TipoSituacaoAcomodacao, F.Id_TipoAtividadeAcomodacao, TAA.Nome_TipoAtividadeAcomodacao, F.Id_ItemChecklist, IC.Nome_ItemChecklist, F.Cod_Resposta, F.Cod_PermiteTotal ";
            m_sql += " FROM  FluxoAutomaticoCheck AS F INNER JOIN ";
            m_sql += "      Checklist_ItensChecklist AS CI ON F.Id_Checklist = CI.Id_Checklist AND F.Id_ItemChecklist = CI.Id_ItemChecklist INNER JOIN ";
            m_sql += "      Checklist AS C ON CI.Id_Checklist = C.Id_Checklist INNER JOIN ";
            m_sql += "      ItensChecklist AS IC ON CI.Id_ItemChecklist = IC.Id_ItemChecklist INNER JOIN ";
            m_sql += "      TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON F.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "      TipoSituacaoAcomodacao AS TSA ON TSTA.Id_TipoSituacaoAcomodacao = TSA.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "      TipoAtividadeAcomodacao AS TAA ON TSTA.Id_TipoAtividadeAcomodacao = TAA.Id_TipoAtividadeAcomodacao ";

            m_sql += " WHERE(F.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND  (F.Id_Checklist = @Id_Checklist) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_Checklist", IdChecklist);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        #endregion
    }
}
