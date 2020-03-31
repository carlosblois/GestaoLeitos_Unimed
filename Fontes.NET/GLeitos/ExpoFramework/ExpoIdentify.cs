using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace ExpoFramework.Framework.Utils
{
    /// <summary>
    /// Classe que representa a identidade do usuário
    /// </summary>
    public partial class ExpoIdentity
        : MarshalByRefObject, IIdentity
    {

        #region Variáveis privadas

        private readonly FormsAuthenticationTicket _ticket;
        private readonly string _uniqueIdentifier;

        #endregion

        #region Construtor

        /// <summary>
        /// Inicializar a identidade do usuário com o FormsAuthenticationTicket para ler os dados personalizado
        /// </summary>
        /// <param name="ticket">Representa um objeto FormsAuthenticationTicket</param>
        public ExpoIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
            _uniqueIdentifier = _ticket.UserData;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indica o tipo de autenticação do usuário
        /// </summary>
        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        /// <summary>
        /// Indica se o usuário está ou não indicado
        /// </summary>
        public bool IsAuthenticated
        {
            get { return true; }
        }

        /// <summary>
        /// Indica o nome associado à propriedade Ticket
        /// </summary>
        public string Name
        {
            get { return _ticket.Name; }
        }

        /// <summary>
        /// Representa o objeto FormsAuthenticationTicket associado ao usuário
        /// </summary>
        public FormsAuthenticationTicket Ticket
        {
            get { return _ticket; }
        }

        /// <summary>
        /// Indica o identificador exclusivo do usuário armazenado ao objeto FormsAuthenticationTicket
        /// </summary>
        public string UniqueIdentifier
        {
            get
            {
                return _uniqueIdentifier;
            }
        }

        #endregion

    }
}