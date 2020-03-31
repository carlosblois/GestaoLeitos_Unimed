using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.API.TO
{
    public class ConsultarPerfisPorIdEmpresaTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_Empresa;

        private string m_Nome_Empresa;

        private int m_Id_Perfil;

        private string m_Nome_Perfil;

        private string m_Cod_Tipo_Perfil;

        private string m_Cod_Tipo_Descricao_Perfil;

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

        public String Cod_Tipo_Perfil
        {
            get
            {
                return this.m_Cod_Tipo_Perfil;
            }
            set
            {
                this.m_Cod_Tipo_Perfil = value;
            }

        }

        public String Cod_Tipo_Descricao_Perfil
        {
            get
            {
                return this.m_Cod_Tipo_Descricao_Perfil;
            }
            set
            {
                this.m_Cod_Tipo_Descricao_Perfil = value;
            }

        }
        

        #endregion

        #region Construtor

        public ConsultarPerfisPorIdEmpresaTO()
        {
           
            m_Nome_Empresa = String.Empty;
            m_Nome_Perfil = String.Empty;
            m_Cod_Tipo_Perfil = String.Empty;
            m_Cod_Tipo_Descricao_Perfil = String.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarPorIdEmpresaTOCommand(int IdEmpresa ,string connection ,ref List<ConsultarPerfisPorIdEmpresaTO> l_ListEmpresaPerfilTO)
        {
            m_sql = "SELECT EP.Id_Empresa, E.Nome_Empresa, EP.Id_Perfil, P.Nome_Perfil, ";
            m_sql += " EP.Cod_Tipo  AS Cod_Tipo_Perfil, CASE EP.Cod_Tipo WHEN 'G' THEN 'Gestão' WHEN 'O' THEN 'Operação' END AS Cod_Tipo_Descricao_Perfil ";
            m_sql += " FROM   Empresa AS E INNER JOIN ";
            m_sql += " EmpresaPerfil AS EP ON E.Id_Empresa = EP.Id_Empresa INNER JOIN ";
            m_sql += " Perfil AS P ON EP.Id_Perfil = P.Id_Perfil ";
            m_sql += " WHERE EP.Id_Empresa = @Id_Empresa ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListEmpresaPerfilTO);
            Dados = null;
        }
        #endregion
    }
}
