using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class LoginView: BaseView
    {

        public LoginTO Login { get; set; }

        public List<LogarUsuarioEmpresaPerfilTO> logarUsuarioEmpresaPerfilTO { get; set; }
        public List<LogarUsuarioEmpresaTO> logarUsuarioEmpresaTO { get; set; }
        public List<LogarUsuarioPerfilTO> logarUsuarioPerfilTO { get; set; }

        public LoginView()
        {
            this.Login = new LoginTO();
            this.logarUsuarioEmpresaPerfilTO = new List<LogarUsuarioEmpresaPerfilTO>();
            this.logarUsuarioEmpresaTO = new List<LogarUsuarioEmpresaTO>();
            this.logarUsuarioPerfilTO = new List<LogarUsuarioPerfilTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}