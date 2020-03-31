using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade;
        private int m_Id_Perfil;
        private string m_Nome_Perfil;
        private int m_Id_Empresa;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private string m_Cod_Tipo;

        #endregion

        #region Propriedades

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public int Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade { get => m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade; set => m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade = value; }
        public int Id_Perfil { get => m_Id_Perfil; set => m_Id_Perfil = value; }
        public string Nome_Perfil { get => m_Nome_Perfil; set => m_Nome_Perfil = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public string Cod_Tipo { get => m_Cod_Tipo; set => m_Cod_Tipo = value; }

        #endregion

        #region Construtor

        public ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO()
        {
            Nome_Perfil = string.Empty;
            Nome_TipoSituacaoAcomodacao = string.Empty;
            Nome_TipoAtividadeAcomodacao = string.Empty;
            Cod_Tipo = string.Empty;

        }

        #endregion

        #region SQL
        public void ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTOCommand(int IdEmpresa, int IdPerfil, string connection, ref List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> l_ListTO)
        {


            m_sql = " SELECT A.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade, A.Id_Perfil, P.Nome_Perfil, A.Id_Empresa, A.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, A.Id_TipoAtividadeAcomodacao,  ";
            m_sql += "       TA.Nome_TipoAtividadeAcomodacao, A.Cod_Tipo ";
            m_sql += "FROM   AcessoEmpresaPerfilTipoSituacaoTipoAtividade AS A INNER JOIN ";
            m_sql += "       EmpresaPerfil AS EP ON A.Id_Empresa = EP.Id_Empresa AND A.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "       Perfil AS P ON EP.Id_Perfil = P.Id_Perfil INNER JOIN ";
            m_sql += "       TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON A.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND A.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "       TipoAtividadeAcomodacao AS TA ON TSTA.Id_TipoAtividadeAcomodacao = TA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "       TipoSituacaoAcomodacao AS TS ON TSTA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += "WHERE (A.Id_Perfil = @Id_Perfil) AND(A.Id_Empresa = @Id_Empresa) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_Perfil", IdPerfil);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        public void ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividadeCommand(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividadeAcomodacao, string connection, ref List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> l_ListTO)
        {


            m_sql = " SELECT A.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade, A.Id_Perfil, P.Nome_Perfil, A.Id_Empresa, A.Id_TipoSituacaoAcomodacao, TS.Nome_TipoSituacaoAcomodacao, A.Id_TipoAtividadeAcomodacao,  ";
            m_sql += "       TA.Nome_TipoAtividadeAcomodacao, A.Cod_Tipo ";
            m_sql += "FROM   AcessoEmpresaPerfilTipoSituacaoTipoAtividade AS A INNER JOIN ";
            m_sql += "       EmpresaPerfil AS EP ON A.Id_Empresa = EP.Id_Empresa AND A.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "       Perfil AS P ON EP.Id_Perfil = P.Id_Perfil INNER JOIN ";
            m_sql += "       TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON A.Id_TipoSituacaoAcomodacao = TSTA.Id_TipoSituacaoAcomodacao AND A.Id_TipoAtividadeAcomodacao = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "       TipoAtividadeAcomodacao AS TA ON TSTA.Id_TipoAtividadeAcomodacao = TA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "       TipoSituacaoAcomodacao AS TS ON TSTA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += "WHERE (A.Id_TipoSituacaoAcomodacao = @Id_TipoSituacaoAcomodacao) AND (A.Id_Empresa = @Id_Empresa)  AND (A.Id_TipoAtividadeAcomodacao = @Id_TipoAtividadeAcomodacao)";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacao);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacao", IdTipoAtividadeAcomodacao);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        #endregion
    }
}
