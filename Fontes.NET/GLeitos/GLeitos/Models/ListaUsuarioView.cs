using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class ListaUsuarioView: BaseView
    {

        public List<UsuarioTO> ListaUsuarios { get; set; }

        public ListaUsuarioView()
        {
            this.ListaUsuarios = new List<UsuarioTO>();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}