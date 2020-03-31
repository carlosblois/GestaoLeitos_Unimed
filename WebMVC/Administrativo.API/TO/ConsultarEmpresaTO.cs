using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Administrativo.API.TO
{
    public class ConsultarEmpresaTO
    {
        string m_sql;
        #region Atributos


        private int m_Id_Empresa;
        private string m_Nome_Empresa;      
        private string m_CodExterno_Empresa;
        private string m_Endereco_Empresa;
        private string m_Complemento_Empresa;
        private string m_Numero_Empresa;
        private string m_Bairro_Empresa;
        private string m_Cidade_Empresa;
        private string m_Estado_Empresa;
        private string m_Cep_Empresa;
        private string m_Fone_Empresa;
        private string m_Contato_Empresa;
        private string m_CGC_Empresa;
        private string m_InscricaoMunicipal_Empresa;
        private string m_InscricaoEstadual_Empresa;
        private string m_CNES_Empresa;

        #endregion

        #region Propriedades

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public string Nome_Empresa { get => m_Nome_Empresa; set => m_Nome_Empresa = value; }
        public string CodExterno_Empresa { get => m_CodExterno_Empresa; set => m_CodExterno_Empresa = value; }
        public string Endereco_Empresa { get => m_Endereco_Empresa; set => m_Endereco_Empresa = value; }
        public string Complemento_Empresa { get => m_Complemento_Empresa; set => m_Complemento_Empresa = value; }
        public string Numero_Empresa { get => m_Numero_Empresa; set => m_Numero_Empresa = value; }
        public string Bairro_Empresa { get => m_Bairro_Empresa; set => m_Bairro_Empresa = value; }
        public string Cidade_Empresa { get => Cidade_Empresa1; set => Cidade_Empresa1 = value; }
        public string Cidade_Empresa1 { get => m_Cidade_Empresa; set => m_Cidade_Empresa = value; }
        public string Cep_Empresa { get => m_Cep_Empresa; set => m_Cep_Empresa = value; }
        public string Estado_Empresa { get => m_Estado_Empresa; set => m_Estado_Empresa = value; }
        public string Fone_Empresa { get => m_Fone_Empresa; set => m_Fone_Empresa = value; }
        public string Contato_Empresa { get => m_Contato_Empresa; set => m_Contato_Empresa = value; }
        public string CGC_Empresa { get => m_CGC_Empresa; set => m_CGC_Empresa = value; }
        public string InscricaoMunicipal_Empresa { get => m_InscricaoMunicipal_Empresa; set => m_InscricaoMunicipal_Empresa = value; }
        public string InscricaoEstadual_Empresa { get => m_InscricaoEstadual_Empresa; set => m_InscricaoEstadual_Empresa = value; }
        public string CNES_Empresa { get => m_CNES_Empresa; set => m_CNES_Empresa = value; }


        #endregion

        #region Construtor

        public ConsultarEmpresaTO()
        {
            m_Nome_Empresa = string.Empty;
            m_CodExterno_Empresa = string.Empty;
            m_Endereco_Empresa = string.Empty;
            m_Complemento_Empresa = string.Empty;
            m_Numero_Empresa = string.Empty;
            m_Bairro_Empresa = string.Empty;
            m_Cidade_Empresa = string.Empty;
            m_Estado_Empresa = string.Empty;
            m_Cep_Empresa = string.Empty;
            m_Fone_Empresa = string.Empty;
            m_Contato_Empresa = string.Empty;
            m_CGC_Empresa = string.Empty;
            m_InscricaoMunicipal_Empresa = string.Empty;
            m_InscricaoEstadual_Empresa = string.Empty;
            m_CNES_Empresa = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarEmpresaTOCommand( string connection, ref List<ConsultarEmpresaTO> l_ListEmpresaTO)
        {

            m_sql = " SELECT Id_Empresa, Nome_Empresa, CodExterno_Empresa, ";
            m_sql += " Endereco_Empresa, Complemento_Empresa, Numero_Empresa, ";
            m_sql += " Bairro_Empresa, Cidade_Empresa, Estado_Empresa, Cep_Empresa, ";
            m_sql += " Fone_Empresa, Contato_Empresa, CGC_Empresa, InscricaoMunicipal_Empresa, ";
            m_sql += " InscricaoEstadual_Empresa, CNES_Empresa ";
            m_sql += " FROM Empresa AS E ";
            m_sql += " ORDER BY Nome_Empresa ";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListEmpresaTO);
            Dados = null;
        }
        #endregion
    }
}
