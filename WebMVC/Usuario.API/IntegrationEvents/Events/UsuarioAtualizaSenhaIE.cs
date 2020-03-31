using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class UsuarioAtualizaSenhaIE : IntegrationEvent
    {
        public int UsuarioId { get; private set; }
        public string UsuarioSenha { get; private set; }


        public UsuarioAtualizaSenhaIE(int usuarioId,string usuarioSenha)
        {
            UsuarioId = usuarioId;
            UsuarioSenha = usuarioSenha;
     
        }

    }
}
