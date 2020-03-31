using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class ValidarLoginUsuarioTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Usuario ;

        private String m_Nome_Usuario;
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

        public int Ativo { get => m_Ativo; set => m_Ativo = value; }

        #endregion

        #region Construtor

        public ValidarLoginUsuarioTO()
        {
            m_Nome_Usuario = String.Empty;
        }

        #endregion

        #region SQL
        public void ValidarLoginUsuarioTOCommand(string LoginUsuario, string SenhaUsuario,string connection, ref List<ValidarLoginUsuarioTO> l_ListUsuarioTO)
        {

            m_sql =  " SELECT Id_Usuario, Nome_Usuario, Ativo ";
            m_sql += " FROM Usuario ";
            m_sql += " WHERE(Login_Usuario = @Login_Usuario) AND (Senha_Usuario = @Senha_Usuario)";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Login_Usuario", LoginUsuario);
            command.Parameters.AddWithValue("Senha_Usuario", SenhaUsuario);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListUsuarioTO);
            Dados = null;
        }

        #endregion
    }
}
