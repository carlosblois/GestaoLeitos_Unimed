using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarCheckListAtividadeTO
    {
        string m_sql;


        #region Atributos
        private int m_Id_AtividadeAcomodacaoOrigem;
        private int m_Id_AtividadeAcomodacaoDestino;
        private int m_Id_ItemChecklist;
        private string m_Nome_ItemChecklist;
        private string m_Valor;
        private DateTime m_dt_FimAcaoAtividade;
        private string m_Id_UsuarioExecutor;
        private string m_Nome_UsuarioExecutor;
        private string m_Nome_TipoAtividadeAcomodacaoOrigem;

        #endregion
        public int Id_ItemChecklist { get => m_Id_ItemChecklist; set => m_Id_ItemChecklist = value; }
        public string Nome_ItemChecklist { get => m_Nome_ItemChecklist; set => m_Nome_ItemChecklist = value; }
        public string Valor { get => m_Valor; set => m_Valor = value; }
        public DateTime Dt_FimAcaoAtividade { get => m_dt_FimAcaoAtividade; set => m_dt_FimAcaoAtividade = value; }
        public string Id_UsuarioExecutor { get => m_Id_UsuarioExecutor; set => m_Id_UsuarioExecutor = value; }
        public string Nome_UsuarioExecutor { get => m_Nome_UsuarioExecutor; set => m_Nome_UsuarioExecutor = value; }
        public string Nome_TipoAtividadeAcomodacaoOrigem { get => m_Nome_TipoAtividadeAcomodacaoOrigem; set => m_Nome_TipoAtividadeAcomodacaoOrigem = value; }
        public int Id_AtividadeAcomodacaoDestino { get => m_Id_AtividadeAcomodacaoDestino; set => m_Id_AtividadeAcomodacaoDestino = value; }
        public int Id_AtividadeAcomodacaoOrigem { get => m_Id_AtividadeAcomodacaoOrigem; set => m_Id_AtividadeAcomodacaoOrigem = value; }
        #region Propriedades

        #endregion

        #region Construtor
        public ConsultarCheckListAtividadeTO()
        {
            m_Nome_ItemChecklist = string.Empty;
            m_Valor = string.Empty;
            m_Id_UsuarioExecutor = string.Empty;
            m_Nome_UsuarioExecutor = string.Empty;
            m_Nome_TipoAtividadeAcomodacaoOrigem = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarCheckListAtividadeTOCommand(int IdAtividadeAcomodacao, string connection, ref List<ConsultarCheckListAtividadeTO> l_ListCheckListAtividadeTO)
        {

            m_sql += " SELECT * ";
            m_sql += " FROM vw_ConsultarCheckListAtividadeAnterior ";
            m_sql += " WHERE (Id_AtividadeAcomodacaoDestino = @Id_AtividadeAcomodacaoDestino) ";



            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_AtividadeAcomodacaoDestino", IdAtividadeAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListCheckListAtividadeTO);
            Dados = null;
        }
        #endregion
    }
}
