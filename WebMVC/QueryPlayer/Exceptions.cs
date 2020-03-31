using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Text;

namespace Framework.Exceptions
{
    public class ExcecaoBase : System.Exception
    {

        public ExcecaoBase(string message)
            : base(message)
        {
        }

        public ExcecaoBase()
            : base()
        {
        }

        public ExcecaoBase(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        protected ExcecaoBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable()]
    public class ExcecaoDb : ExcecaoBase
    {

        #region "Atributo(s) / Propriedade(s)"
        private string m_NomeParametro = string.Empty;
        public string NomeParametro
        {
            get { return m_NomeParametro; }
        }

        protected void SetNomeParametro(string value)
        {
            m_NomeParametro = value.Trim().ToUpper();
        }

        #endregion

        #region "Metodo(s) Construtor(es)"

        public ExcecaoDb()
            : base()
        {
        }

        public ExcecaoDb(string pMensagem)
            : base(pMensagem)
        {
        }

        public ExcecaoDb(string pMensagem, Exception pEx)
            : base(pMensagem, pEx)
        {
        }

        public ExcecaoDb(string pNomeParametro, string pMensagem)
        {
            m_NomeParametro = pNomeParametro.Trim().ToUpper();
            ErrorHandler.TrataErro ObjTrataErros = new ErrorHandler.TrataErro();
            // Envio da excecao.
            ObjTrataErros.Main(pNomeParametro + "::" + pMensagem);
        }

        protected ExcecaoDb(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public ExcecaoDb(string pNomeParametro, string pMensagem, System.Exception pInnerException)
        {
            m_NomeParametro = pNomeParametro.Trim().ToUpper();
        }



        public ExcecaoDb(string pNomeParametro, string pMensagem, params string[] pArray)
        {
            m_NomeParametro = pNomeParametro.Trim().ToUpper();
        }


        public ExcecaoDb(List<string> pMensagem)
            : base(TrataListaMensagem(pMensagem))
        {

        }

        #endregion

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("NomeParametro", NomeParametro);
        }

        public static string TrataListaMensagem(List<string> pMensagem)
        {
            StringBuilder msgRetorno = new StringBuilder();
            foreach (string item in pMensagem)
            {
                msgRetorno.AppendFormat("{0}\n", item);
            }

            return msgRetorno.ToString().PadLeft(msgRetorno.Length - 2);

        }

    }


    public class TrataErro
    {


        #region "Variaveis"

        #endregion

        #region "Metodos"


        public string Main(Exception pErro, params string[] pComplementosErro)
        {

            string Msg = null;
            string DescErro = null;

            DescErro = pErro.Message;

            Msg = DescErro;
            
            Msg = Msg.Replace("'", " ");

            return Msg;


        }

        #endregion

    }


}

