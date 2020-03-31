using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class ConsultarPerfisPorIdUsuarioTO
    {
        string m_sql;
        #region Atributos
        private int m_Id_Usuario;

        private String m_Nome_Usuario;

        private int m_Id_Empresa;

        private String m_Nome_Empresa;

        private int m_Id_Perfil;

        private String m_Nome_Perfil;

        #endregion

        #region Propriedades
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
        public String Nome_Empresa
        {
            get
            {
                return this.m_Nome_Empresa;
            }
            set
            {
                this.m_Nome_Empresa = value;
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

        #endregion

        #region Construtor

        public ConsultarPerfisPorIdUsuarioTO()
        {

            m_Nome_Empresa = String.Empty;
            m_Nome_Perfil = String.Empty;
            m_Nome_Usuario = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarPerfisPorIdUsuarioTOCommand(int IdUsuario, string connection, ref List<ConsultarPerfisPorIdUsuarioTO> l_ListUsuarioPerfilTO)
        {

            m_sql = " SELECT U.Id_Usuario, U.Nome_Usuario, UEP.Id_Empresa, E.Nome_Empresa, UEP.Id_Perfil, P.Nome_Perfil ";
            m_sql += " FROM   Usuario AS U INNER JOIN ";
            m_sql += "              UsuarioEmpresaPerfil AS UEP ON U.Id_Usuario = UEP.Id_Usuario INNER JOIN ";
            m_sql += "              EmpresaPerfil AS EP ON UEP.Id_Empresa = EP.Id_Empresa AND UEP.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "              Empresa AS E ON EP.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "              Perfil AS P ON EP.Id_Perfil = P.Id_Perfil ";
            m_sql += " WHERE(U.Id_Usuario = @Id_Usuario) ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Usuario", IdUsuario);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListUsuarioPerfilTO);
            Dados = null;
        }

        public void ConsultarPerfisPorIdUsuarioTOCommand(int IdUsuario, int IdPerfil, string connection, ref List<ConsultarPerfisPorIdUsuarioTO> l_ListUsuarioPerfilTO)
        {

            m_sql = " SELECT U.Id_Usuario, U.Nome_Usuario, UEP.Id_Empresa, E.Nome_Empresa, UEP.Id_Perfil, P.Nome_Perfil ";
            m_sql += " FROM   Usuario AS U INNER JOIN ";
            m_sql += "              UsuarioEmpresaPerfil AS UEP ON U.Id_Usuario = UEP.Id_Usuario INNER JOIN ";
            m_sql += "              EmpresaPerfil AS EP ON UEP.Id_Empresa = EP.Id_Empresa AND UEP.Id_Perfil = EP.Id_Perfil INNER JOIN ";
            m_sql += "              Empresa AS E ON EP.Id_Empresa = E.Id_Empresa INNER JOIN ";
            m_sql += "              Perfil AS P ON EP.Id_Perfil = P.Id_Perfil ";
            m_sql += " WHERE(U.Id_Usuario = @Id_Usuario) AND (UEP.Id_Perfil = @Id_Perfil)";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Usuario", IdUsuario);
            command.Parameters.AddWithValue("Id_Perfil", IdPerfil);
            

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListUsuarioPerfilTO);
            Dados = null;
        }
        #endregion
    }
}
