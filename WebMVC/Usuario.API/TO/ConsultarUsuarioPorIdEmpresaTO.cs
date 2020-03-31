using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class ConsultarUsuarioPorIdEmpresaTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Usuario;
        private String m_Nome_Usuario;
        private int m_Id_Empresa;
        private String m_Nome_Empresa;
        private String m_Login_Usuario;
        private int m_Id_Perfil;
        private String m_Nome_Perfil;
        private int m_Ativo;
        private String m_Senha_Usuario;

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
        public int Id_Perfil
        {
            get
            {
                return this.m_Id_Perfil;
            }
            set
            {
                this.m_Id_Perfil = value;
            }
        }
        public String Nome_Perfil
        {
            get
            {
                return this.m_Nome_Perfil;
            }
            set
            {
                this.m_Nome_Perfil = value;
            }
        }
        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public string Login_Usuario { get => m_Login_Usuario; set => m_Login_Usuario = value; }
        public int Ativo { get => m_Ativo; set => m_Ativo = value; }
        public string Senha_Usuario { get => m_Senha_Usuario; set => m_Senha_Usuario = value; }

        #endregion

        #region Construtor

        public ConsultarUsuarioPorIdEmpresaTO()
        {
            m_Nome_Usuario = String.Empty;
            m_Nome_Empresa = String.Empty;
            m_Login_Usuario = String.Empty;
            m_Nome_Perfil = String.Empty;
            m_Senha_Usuario = String.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarUsuarioPorIdEmpresaTOCommand(int IdEmpresa,   string connection, ref List<ConsultarUsuarioPorIdEmpresaTO> l_ListUsuarioTO)
        {

            m_sql = " SELECT DISTINCT U.Id_Usuario, U.Nome_Usuario, U.Login_Usuario, U.Senha_Usuario, U.Ativo ";
            m_sql += "FROM   Usuario AS U INNER JOIN ";
            m_sql += "       UsuarioEmpresaPerfil AS UE ON U.Id_Usuario = UE.Id_Usuario INNER JOIN ";
            m_sql += "       EmpresaPerfil AS EP ON UE.Id_Empresa = EP.Id_Empresa AND UE.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "       Empresa AS E ON EP.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "        Perfil AS P ON EP.Id_Perfil = P.Id_Perfil ";
            m_sql += "WHERE(E.Id_Empresa = @Id_Empresa) ";



            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListUsuarioTO);
            Dados = null;
        }

        #endregion
    }
}
