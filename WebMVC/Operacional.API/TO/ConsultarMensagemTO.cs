using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarMensagemTO
    {
        string m_sql;

        #region Atributos

        private int m_IdMensagem;
        private int? m_IdAtividadeAcomodacao;
        private DateTime m_Dt_EnvioMensagem;
        private DateTime? m_Dt_RecebimentoMensagem;
        private int m_IdEmpresa;
        private int m_IdUsuarioEmissor;
        private int m_IdUsuarioDestinatario;
        private string m_LoginUsuarioEmissor;
        private string m_LoginUsuarioDestinatario;
        private string m_TextoMensagem;



        #endregion

        #region Propriedades

        public int Id_Mensagem { get => m_IdMensagem; set => m_IdMensagem = value; }
        public int? Id_AtividadeAcomodacao { get => m_IdAtividadeAcomodacao; set => m_IdAtividadeAcomodacao = value; }
        public DateTime Dt_EnvioMensagem { get => m_Dt_EnvioMensagem; set => m_Dt_EnvioMensagem = value; }
        public DateTime? Dt_RecebimentoMensagem { get => m_Dt_RecebimentoMensagem; set => m_Dt_RecebimentoMensagem = value; }
        public int Id_Empresa { get => m_IdEmpresa; set => m_IdEmpresa = value; }
        public int Id_Usuario_Emissor { get => m_IdUsuarioEmissor; set => m_IdUsuarioEmissor = value; }
        public int Id_Usuario_Destinatario { get => m_IdUsuarioDestinatario; set => m_IdUsuarioDestinatario = value; }
        public string Login_Usuario_Emissor { get => m_LoginUsuarioEmissor; set => m_LoginUsuarioEmissor = value; }
        public string Login_Usuario_Destinatario { get => m_LoginUsuarioDestinatario; set => m_LoginUsuarioDestinatario = value; }
        public string TextoMensagem { get => m_TextoMensagem; set => m_TextoMensagem = value; }

        #endregion

        #region Construtor

        public ConsultarMensagemTO()
        {

            m_LoginUsuarioEmissor = string.Empty;
            m_LoginUsuarioDestinatario = string.Empty;
            m_TextoMensagem = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultaMensagemPorIdAtividadeTOCommand(string idAtividade,  string connection, ref List<ConsultarMensagemTO> l_ListTO)
        {

            m_sql = "  SELECT Id_Mensagem, Id_AtividadeAcomodacao, dt_EnvioMensagem, dt_RecebimentoMensagem, Id_Empresa, ";
            m_sql += " Id_Usuario_Emissor, 	usu_emi.Login_Usuario Login_Usuario_Emissor,  Id_Usuario_Destinatario, usu_des.Login_Usuario Login_Usuario_Destinatario, ";
            m_sql += " TextoMensagem ";
            m_sql += " FROM Mensagem    MSG ";
            m_sql += " LEFT JOIN    Usuario usu_emi ";
            m_sql += " ON msg.Id_Usuario_Emissor = usu_emi.Id_Usuario ";
            m_sql += " LEFT JOIN Usuario     usu_des ";
            m_sql += " ON      msg.Id_Usuario_Destinatario = usu_des.Id_Usuario ";
            m_sql += " WHERE Id_AtividadeAcomodacao = @idAtividade ";             

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("idAtividade", idAtividade);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListTO);
            Dados = null;
        }

        #endregion
    }
}
