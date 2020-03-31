﻿using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class UsuarioAtualizaLoginIE : IntegrationEvent
    {
        public int UsuarioId { get; private set; }
        public string UsuarioLogin { get; private set; }


        public UsuarioAtualizaLoginIE(int usuarioId , string usuarioLogin)
        {
            UsuarioId = usuarioId;
            UsuarioLogin = usuarioLogin;
        }

    }
}
