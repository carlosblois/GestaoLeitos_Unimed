using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class LogarUsuarioEmpresaPerfilTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Usuario;
        private String m_Nome_Usuario;
        private int m_Id_Empresa;
        private String m_Nome_Empresa;
        private int m_Id_Perfil;
        private String m_Nome_Perfil;
        private String m_Cod_Tipo;
        private String m_Cod_Tipo_Descricao;
        private int m_Ativo;

        #endregion

        #region Propriedades
        public int Id_Usuario
        {
            get
            {
                return this.m_Id_Usuario;
            }
            set
            {
                this.m_Id_Usuario = value;
            }
        }
        public String Nome_Usuario
        {
            get
            {
                return this.m_Nome_Usuario;
            }
            set
            {
                this.m_Nome_Usuario = value;
            }
        }

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public int Id_Perfil { get => m_Id_Perfil; set => m_Id_Perfil = value; }
        public string Nome_Perfil { get => m_Nome_Perfil; set => m_Nome_Perfil = value; }
        public string Cod_Tipo { get => m_Cod_Tipo; set => m_Cod_Tipo = value; }
        public string Cod_Tipo_Descricao { get => m_Cod_Tipo_Descricao; set => m_Cod_Tipo_Descricao = value; }
        public int Ativo { get => m_Ativo; set => m_Ativo = value; }

        #endregion

        #region Construtor

        public LogarUsuarioEmpresaPerfilTO()
        {
            m_Nome_Usuario = String.Empty;
            m_Nome_Empresa = String.Empty;
            m_Nome_Perfil = String.Empty;
            m_Cod_Tipo = String.Empty;
            m_Cod_Tipo_Descricao = String.Empty;
        }

        #endregion

        #region SQL
        public void LogarUsuarioEmpresaPerfilTOCommand(string LoginUsuario, string SenhaUsuario, string connection, ref List<LogarUsuarioEmpresaPerfilTO> l_ListUsuarioPerfilTO)
        {

            m_sql = " SELECT U.Id_Usuario, U.Nome_Usuario, UEP.Id_Empresa, E.Nome_Empresa, ";
            m_sql += "       UEP.Id_Perfil, P.Nome_Perfil, EP.Cod_Tipo, ";
            m_sql += "       CASE EP.Cod_Tipo WHEN 'G' THEN 'Gestão' WHEN 'O' THEN 'Operação' END AS Cod_Tipo_Descricao, U.Ativo  ";
            m_sql += " FROM  Usuario AS U INNER JOIN ";
            m_sql += "       UsuarioEmpresaPerfil AS UEP ON U.Id_Usuario = UEP.Id_Usuario INNER JOIN ";
            m_sql += "       EmpresaPerfil AS EP ON UEP.Id_Empresa = EP.Id_Empresa AND UEP.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "       Empresa AS E ON EP.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "       Perfil AS P ON EP.Id_Perfil = P.Id_Perfil ";
            m_sql += " WHERE(U.Login_Usuario = @Login_Usuario) AND(U.Senha_Usuario = @Senha_Usuario) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Login_Usuario", LoginUsuario);
            command.Parameters.AddWithValue("Senha_Usuario", SenhaUsuario);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListUsuarioPerfilTO);
            Dados = null;
        }

        #endregion
    }
}
