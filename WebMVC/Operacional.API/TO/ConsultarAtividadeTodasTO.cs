using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAtividadeTodasTO
    {
        string m_sql;


        #region Atributos
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private int m_SLA;
        private int m_MEDIA;
        private int m_QTD;
        private string m_FORASLA;
        private string m_MaiorTempo;
        private string m_Cod_Prioritario_Atividade;
        private string m_PendenciaAdministrativa;
        #endregion

        #region Propriedades

        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public int SLA { get => m_SLA; set => m_SLA = value; }
        public int MEDIA { get => m_MEDIA; set => m_MEDIA = value; }
        public int QTD { get => m_QTD; set => m_QTD = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public string FORASLA { get => m_FORASLA; set => m_FORASLA = value; }
        public string MaiorTempo { get => m_MaiorTempo; set => m_MaiorTempo = value; }
        public string Cod_Prioritario_Atividade { get => m_Cod_Prioritario_Atividade; set => m_Cod_Prioritario_Atividade = value; }
        public string PendenciaAdministrativa { get => m_PendenciaAdministrativa; set => m_PendenciaAdministrativa = value; }
        #endregion

        #region Construtor
        public ConsultarAtividadeTodasTO()
        {
            Nome_TipoAtividadeAcomodacao = string.Empty;
            m_FORASLA = string.Empty;
            m_MaiorTempo = string.Empty;
            m_Cod_Prioritario_Atividade = string.Empty;
            m_PendenciaAdministrativa = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarAtividadeTodasTOCommand(int IdEmpresa, int IdUsuario, int IdPerfil, string connection, ref List<ConsultarAtividadeTodasTO> l_ListAtividadeTodasTO)
        {

            m_sql += " SELECT * ";
            m_sql += " FROM vw_ConsultarAtividadeTodas ";
            m_sql += " WHERE (Id_Usuario = @Id_Usuario) AND(Id_Perfil =  @Id_Perfil) AND(Id_Empresa =  @Id_Empresa)  ";



            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);
            command.Parameters.AddWithValue("Id_Perfil", IdPerfil);
            command.Parameters.AddWithValue("Id_Usuario", IdUsuario);


            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListAtividadeTodasTO);
            Dados = null;
        }
        #endregion
    }
}
