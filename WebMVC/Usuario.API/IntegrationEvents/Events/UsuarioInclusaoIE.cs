using EventBus.Events;
using System;
using System.Collections.Generic;
using Usuario.API.Model;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class UsuarioInclusaoIE : IntegrationEvent
    {
        public int UsuarioId { get;  set; }
        public string UsuarioNome { get; private set; }
        public string UsuarioLogin { get; private set; }
        public string UsuarioSenha { get; private set; }
        public List<UsuarioEmpresaPerfilItem> UsuarioEmpresaPerfilItems { get; set; }

        public UsuarioInclusaoIE(int usuarioId, string usuarioNome, string usuarioLogin, string usuarioSenha)
        {
            UsuarioId = usuarioId;
            UsuarioNome = usuarioNome;
            UsuarioLogin = usuarioLogin;
            UsuarioSenha = usuarioSenha;
        }        

    }
    public class UsuarioInclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<UsuarioItem> UsuarioItems { get; }

        public UsuarioInclusaoGrupoIE(IEnumerable<UsuarioItem> usuarioItems)
        {
            UsuarioItems = usuarioItems;
        }
    }
}
