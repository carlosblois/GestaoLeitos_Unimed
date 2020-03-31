using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class UsuarioView : BaseView
    {
        public UsuarioTO Usuario { get; set; }
        public List<PerfilTO> ListaPerfil { get; set; }
        public string ListaPerfilSel { get; set; }

        public UsuarioView()
        {
            this.ListaPerfil = new List<PerfilTO>();
            this.Usuario = new UsuarioTO();
            this.ListaPerfilSel = string.Empty;
            this.erro = "";
            this.mensagem = "";
        }
    }
    
}