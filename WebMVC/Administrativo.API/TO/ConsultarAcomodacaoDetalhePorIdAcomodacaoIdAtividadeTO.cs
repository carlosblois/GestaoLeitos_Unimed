using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Framework.Data;

namespace Administrativo.API.TO
{
    public class ConsultarAcomodacaoDetalhePorIdAcomodacaoIdAtividadeTO
    {
        string m_sql;

        #region Atributos
        private int m_Id_Acomodacao;
        private string m_Nome_Acomodacao;
        private string m_CodExterno_Acomodacao;
        private int m_Id_Empresa;
        private string m_Cod_Isolamento;
        private int m_Id_Setor;
        private string m_Nome_Setor;
        private int m_Id_Paciente;
        private string m_Nome_Paciente;
        private DateTime? m_Dt_NascimentoPaciente;
        private string m_GeneroPaciente;
        private int m_Id_SituacaoAcomodacao;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_AtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private int m_SLAAtividade;
        private int m_SLASituacao;
        private string m_PrioridadeSituacao;
        private string m_PrioridadeAtividade;
        private string m_Cod_Plus;
        private int m_Id_TipoAcomodacao;
        private string m_Nome_TipoAcomodacao;

        private int m_Id_AcaoAtividadeAcomodacao;
        private DateTime? m_dt_InicioAcaoAtividade;
        private int m_Tempo_Minutos;
        private int m_Id_TipoAcaoAcomodacao;
        private string m_Nome_TipoAcaoAcomodacao;
        private string m_Nome_Status;

        private int m_Id_TipoAtividadeAcomodacao;
        private string m_PendenciaFinanceira;

        #endregion

        #region Propriedades

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public int Id_Setor { get => m_Id_Setor; set => m_Id_Setor = value; }
        public string Nome_Setor { get => m_Nome_Setor; set => m_Nome_Setor = value; }
        public int Id_Acomodacao { get => m_Id_Acomodacao; set => m_Id_Acomodacao = value; }
        public string Nome_Acomodacao { get => m_Nome_Acomodacao; set => m_Nome_Acomodacao = value; }
        public int Id_TipoAcomodacao { get => m_Id_TipoAcomodacao; set => m_Id_TipoAcomodacao = value; }
        public string Nome_TipoAcomodacao { get => m_Nome_TipoAcomodacao; set => m_Nome_TipoAcomodacao = value; }
        public string CodExterno_Acomodacao { get => m_CodExterno_Acomodacao; set => m_CodExterno_Acomodacao = value; }
        public string Cod_Isolamento { get => m_Cod_Isolamento; set => m_Cod_Isolamento = value; }
        public int Id_Paciente { get => m_Id_Paciente; set => m_Id_Paciente = value; }
        public string Nome_Paciente { get => m_Nome_Paciente; set => m_Nome_Paciente = value; }
        public DateTime? Dt_NascimentoPaciente { get => m_Dt_NascimentoPaciente; set => m_Dt_NascimentoPaciente = value; }
        public string GeneroPaciente { get => m_GeneroPaciente; set => m_GeneroPaciente = value; }
        public int Id_SituacaoAcomodacao { get => m_Id_SituacaoAcomodacao; set => m_Id_SituacaoAcomodacao = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_AtividadeAcomodacao { get => m_Id_AtividadeAcomodacao; set => m_Id_AtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public int SLAAtividade { get => m_SLAAtividade; set => m_SLAAtividade = value; }
        public int SLASituacao { get => m_SLASituacao; set => m_SLASituacao = value; }
        public string PrioridadeSituacao { get => m_PrioridadeSituacao; set => m_PrioridadeSituacao = value; }
        public string PrioridadeAtividade { get => m_PrioridadeAtividade; set => m_PrioridadeAtividade = value; }
        public string Cod_Plus { get => m_Cod_Plus; set => m_Cod_Plus = value; }
        public int Id_AcaoAtividadeAcomodacao { get => m_Id_AcaoAtividadeAcomodacao; set => m_Id_AcaoAtividadeAcomodacao = value; }
        public DateTime? Dt_InicioAcaoAtividade { get => m_dt_InicioAcaoAtividade; set => m_dt_InicioAcaoAtividade = value; }
        public int Tempo_Minutos { get => m_Tempo_Minutos; set => m_Tempo_Minutos = value; }
        public int Id_TipoAcaoAcomodacao { get => m_Id_TipoAcaoAcomodacao; set => m_Id_TipoAcaoAcomodacao = value; }
        public string Nome_TipoAcaoAcomodacao { get => m_Nome_TipoAcaoAcomodacao; set => m_Nome_TipoAcaoAcomodacao = value; }
        public string Nome_Status { get => m_Nome_Status; set => m_Nome_Status = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string PendenciaFinanceira { get => m_PendenciaFinanceira; set => m_PendenciaFinanceira = value; }

        #endregion

        #region Construtor

        public ConsultarAcomodacaoDetalhePorIdAcomodacaoIdAtividadeTO()
        {      
            m_Nome_Acomodacao = string.Empty;
            m_CodExterno_Acomodacao= string.Empty;
            m_Cod_Isolamento = string.Empty;
            m_Nome_Setor = string.Empty;
            m_Nome_Paciente = string.Empty;
            m_GeneroPaciente = string.Empty;
            m_Nome_TipoSituacaoAcomodacao = string.Empty;
            m_Nome_TipoAtividadeAcomodacao = string.Empty;
            m_PrioridadeSituacao = string.Empty;
            m_PrioridadeAtividade = string.Empty;
            m_Cod_Plus = string.Empty;
            m_Nome_TipoAcomodacao = string.Empty;

            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;

            m_PendenciaFinanceira = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarAcomodacaoDetalhePorIdAcomodacaoIdAtividadeTOCommand(int IdAcomodacao, int IdAtividadeAcomodacao, string connection, ref List<ConsultarAcomodacaoDetalhePorIdAcomodacaoIdAtividadeTO> l_ListAcomodacaoTO)
        {

            m_sql = " SELECT * ";
            m_sql += " FROM  vw_ConsultarAcomodacaoDetalhe ";
            m_sql += " WHERE(Id_Acomodacao = @Id_Acomodacao AND Id_AtividadeAcomodacao = @Id_AtividadeAcomodacao) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Acomodacao", IdAcomodacao);
            command.Parameters.AddWithValue("Id_AtividadeAcomodacao", IdAtividadeAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAcomodacaoTO);
            Dados = null;
        }
        #endregion
    }
}
