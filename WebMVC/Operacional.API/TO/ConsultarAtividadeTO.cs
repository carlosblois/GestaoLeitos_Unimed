using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAtividadeTO
    {
        string m_sql;

        #region Atributos

        private string m_Nome_TipoAtividadeAcomodacao;
        private string m_Nome_Acomodacao;
        private string m_Nome_Setor;
        private string m_Nome_TipoSituacaoAcomodacao;
        private DateTime m_dt_InicioSituacaoAcomodacao;
        private int m_Tempo_Minutos;
        private string m_Nome_TipoAcomodacao;
        private string m_Nome_CaracteristicaAcomodacao;
        private string m_Cod_Prioritario;
        private int m_Id_AtividadeAcomodacao;
        private string m_CodExterno_Acomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private int m_Id_Acomodacao;
        private int m_Id_Setor;
        private int m_Id_TipoSituacaoAcomodacao;
        private int m_Id_CaracteristicaAcomodacao;
        private int m_Id_SituacaoAcomodacao;
        private int m_IdSLASituacao;
        private int m_TempoTotalSLAAtividade;
        private int m_Id_TipoAcomodacao;
        private string m_Cod_Isolamento;
        private string m_PrioridadeAtividade;
        private string m_Cod_Plus;
        private DateTime? m_dt_InicioAcaoAtividade;
        private string m_PendenciaAdministrativa;
        #endregion

        #region Propriedades
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public string Nome_Acomodacao { get => m_Nome_Acomodacao; set => m_Nome_Acomodacao = value; }
        public string Nome_Setor { get => m_Nome_Setor; set => m_Nome_Setor = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public DateTime Dt_InicioSituacaoAcomodacao { get => m_dt_InicioSituacaoAcomodacao; set => m_dt_InicioSituacaoAcomodacao = value; }
        public int Tempo_Minutos { get => m_Tempo_Minutos; set => m_Tempo_Minutos = value; }
        public string Nome_TipoAcomodacao { get => m_Nome_TipoAcomodacao; set => m_Nome_TipoAcomodacao = value; }
        public string Nome_CaracteristicaAcomodacao { get => m_Nome_CaracteristicaAcomodacao; set => m_Nome_CaracteristicaAcomodacao = value; }
        public string Cod_Prioritario { get => m_Cod_Prioritario; set => m_Cod_Prioritario = value; }
        public int Id_AtividadeAcomodacao { get => m_Id_AtividadeAcomodacao; set => m_Id_AtividadeAcomodacao = value; }
        public string CodExterno_Acomodacao { get => m_CodExterno_Acomodacao; set => m_CodExterno_Acomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public int Id_Setor { get => m_Id_Setor; set => m_Id_Setor = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public int Id_CaracteristicaAcomodacao { get => m_Id_CaracteristicaAcomodacao; set => m_Id_CaracteristicaAcomodacao = value; }
        public int Id_SituacaoAcomodacao { get => m_Id_SituacaoAcomodacao; set => m_Id_SituacaoAcomodacao = value; }
        public int IdSLASituacao { get => m_IdSLASituacao; set => m_IdSLASituacao = value; }
        public int TempoTotalSLAAtividade { get => m_TempoTotalSLAAtividade; set => m_TempoTotalSLAAtividade = value; }
        public int Id_TipoAcomodacao { get => m_Id_TipoAcomodacao; set => m_Id_TipoAcomodacao = value; }
        public string Cod_Isolamento { get => m_Cod_Isolamento; set => m_Cod_Isolamento = value; }
        public string PrioridadeAtividade { get => m_PrioridadeAtividade; set => m_PrioridadeAtividade = value; }
        public string Cod_Plus { get => m_Cod_Plus; set => m_Cod_Plus = value; }
        public DateTime? Dt_InicioAcaoAtividade { get => m_dt_InicioAcaoAtividade; set => m_dt_InicioAcaoAtividade = value; }
        public string PendenciaAdministrativa { get => m_PendenciaAdministrativa; set => m_PendenciaAdministrativa = value; }
        #endregion

        #region Construtor

        public ConsultarAtividadeTO()
        {
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_Nome_Acomodacao = string.Empty;
            m_Nome_Setor = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAcomodacao = string.Empty;
            m_Nome_CaracteristicaAcomodacao = string.Empty;
            m_Cod_Prioritario = string.Empty;
            m_CodExterno_Acomodacao = string.Empty;
            m_Cod_Isolamento = string.Empty;
            m_PrioridadeAtividade = string.Empty;
            m_Cod_Plus = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarAtividadeAcaoNaoFinalizadaPorUsuarioPorTipoAcaoTOCommand(int IdEmpresa, int IdUsuario,int IdTipoAcaoAcomodacao, int IdSetor, int IdCaracteristicaAcomodacao, string connection, ref List<ConsultarAtividadeTO> l_ListAtividadeTO)
        {
            m_sql = "  SELECT vw.Nome_TipoAtividadeAcomodacao, vw.Nome_Acomodacao, vw.Nome_Setor, vw.Nome_TipoSituacaoAcomodacao,  ";
            m_sql += "		  vw.dt_InicioSituacaoAcomodacao, vw.Tempo_Minutos, vw.Nome_TipoAcomodacao,  ";
            m_sql += "        vw.Nome_CaracteristicaAcomodacao, vw.Cod_Prioritario, vw.Id_AtividadeAcomodacao,  ";
            m_sql += "		  vw.CodExterno_Acomodacao, vw.Id_TipoAtividadeAcomodacao, vw.Id_Acomodacao,  ";
            m_sql += "		  vw.Id_Setor, vw.Id_TipoSituacaoAcomodacao,  ";
            m_sql += "        vw.Id_CaracteristicaAcomodacao, vw.Id_SituacaoAcomodacao, vw.Id_SLA AS IdSLASituacao, ";
            m_sql += "		  vw.TOT AS TempoTotalSLAAtividade, vw.Id_TipoAcomodacao, vw.Cod_Isolamento, vw.PrioridadeAtividade, vw.Cod_Plus , AcaoAtividadeAcomodacao.dt_InicioAcaoAtividade, vw.PendenciaAdministrativa ";
            m_sql += " FROM   vw_ListaOperacional AS vw INNER JOIN ";
            m_sql += "        AcaoAtividadeAcomodacao ON vw.Id_AtividadeAcomodacao = AcaoAtividadeAcomodacao.Id_AtividadeAcomodacao ";
            m_sql += " WHERE (vw.Id_Empresa = @Id_Empresa) AND(vw.Id_Usuario = @Id_Usuario) ";
            m_sql += " AND (vw.dt_FimAtividadeAcomodacao IS NULL) ";
            m_sql += " AND (AcaoAtividadeAcomodacao.dt_FimAcaoAtividade IS NULL) ";
            m_sql += " AND (AcaoAtividadeAcomodacao.Id_TipoAcaoAcomodacao = @Id_TipoAcaoAcomodacao) ";
            m_sql += " AND (AcaoAtividadeAcomodacao.Id_UsuarioExecutor = @Id_Usuario) ";

            if (IdSetor != 0)
            {
                m_sql += "       AND (Id_Setor = @Id_Setor)";
            };
            if (IdCaracteristicaAcomodacao != 0)
            {
                m_sql += "       AND (Id_CaracteristicaAcomodacao = @Id_CaracteristicaAcomodacao)";
            };

            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao, Cod_Prioritario DESC, dt_InicioSituacaoAcomodacao";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_Usuario", IdUsuario);
            command.Parameters.AddWithValue("Id_TipoAcaoAcomodacao", IdTipoAcaoAcomodacao);
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


        public void ConsultarAtividadeNaoFinalizadaPorUsuarioTOCommand(int IdEmpresa,int IdUsuario, int IdSetor, int IdCaracteristicaAcomodacao, string connection, ref List<ConsultarAtividadeTO> l_ListAtividadeTO)
        {

            m_sql = " SELECT Nome_TipoAtividadeAcomodacao, Nome_Acomodacao, Nome_Setor, ";
            m_sql += "       Nome_TipoSituacaoAcomodacao, dt_InicioSituacaoAcomodacao, Tempo_Minutos,";
            m_sql += "       Nome_TipoAcomodacao, Nome_CaracteristicaAcomodacao, Cod_Prioritario, Id_AtividadeAcomodacao, CodExterno_Acomodacao,Id_TipoAtividadeAcomodacao, Id_Acomodacao, Id_Setor, Id_TipoSituacaoAcomodacao, Id_CaracteristicaAcomodacao, Id_SituacaoAcomodacao , ";
            m_sql += "       id_SLA AS IdSLASituacao, TOT AS TempoTotalSLAAtividade , Id_TipoAcomodacao , Cod_Isolamento, PrioridadeAtividade, Cod_Plus, PendenciaAdministrativa";
            m_sql += " FROM  vw_ListaOperacional";
            m_sql += " WHERE (Id_Empresa = @Id_Empresa) AND (dt_FimAtividadeAcomodacao IS NULL) ";
            m_sql += "       AND (Id_Usuario = @Id_Usuario)";
            if (IdSetor != 0)
            {
                    m_sql += "       AND (Id_Setor = @Id_Setor)";
            };
            if (IdCaracteristicaAcomodacao != 0)
            {
                    m_sql += "       AND (Id_CaracteristicaAcomodacao = @Id_CaracteristicaAcomodacao)";
            };

            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao, Cod_Prioritario DESC, dt_InicioSituacaoAcomodacao";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_Usuario", IdUsuario);
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

        public void ConsultarAtividadeNaoFinalizadaTOCommand(int IdEmpresa,int IdTipoAtividadeAcomodacao, int IdSetor, int IdCaracteristicaAcomodacao, string connection, ref List<ConsultarAtividadeTO> l_ListAtividadeTO)
        {

            m_sql = " SELECT Nome_TipoAtividadeAcomodacao, Nome_Acomodacao, Nome_Setor, ";
            m_sql += "       Nome_TipoSituacaoAcomodacao, dt_InicioSituacaoAcomodacao, Tempo_Minutos,";
            m_sql += "       Nome_TipoAcomodacao, Nome_CaracteristicaAcomodacao, Cod_Prioritario, Id_AtividadeAcomodacao, CodExterno_Acomodacao,Id_TipoAtividadeAcomodacao, Id_Acomodacao, Id_Setor, Id_TipoSituacaoAcomodacao, Id_CaracteristicaAcomodacao, Id_SituacaoAcomodacao , ";
            m_sql += "       id_SLA AS IdSLASituacao, TOT AS TempoTotalSLAAtividade, Id_TipoAcomodacao, Cod_Isolamento, PrioridadeAtividade, Cod_Plus, PendenciaAdministrativa ";
            m_sql += " FROM  vw_ListaOperacionalSemUser ";
            m_sql += " WHERE Id_AtividadeAcomodacao  IN (SELECT Id_AtividadeAcomodacao FROM AcaoAtividadeAcomodacao WHERE Id_TipoAcaoAcomodacao=5 and dt_FimAcaoAtividade IS NULL)  ";
            m_sql += " AND   (Id_Empresa = @Id_Empresa) AND (Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao) AND (dt_FimAtividadeAcomodacao IS NULL) ";
            if (IdSetor != 0)
            {
                m_sql += "       AND (Id_Setor = @Id_Setor)";
            };
            if (IdCaracteristicaAcomodacao != 0)
            {
                m_sql += "       AND (Id_CaracteristicaAcomodacao = @Id_CaracteristicaAcomodacao)";
            };
            m_sql += " ORDER BY Nome_TipoAtividadeAcomodacao, Cod_Prioritario DESC, dt_InicioSituacaoAcomodacao";

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
