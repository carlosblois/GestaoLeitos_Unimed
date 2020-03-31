using EventBus.Events;
using Usuario.API.Model;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class UsuarioExclusaoIntegrationEvent : IntegrationEvent
    {
        public int UsuarioId { get; private set; }

        public UsuarioExclusaoIntegrationEvent(int usurioId)
        {
            UsuarioId = usurioId;
        }

    }
    public class UsuarioExclusaoGrupoIntegrationEvent : IntegrationEvent
    {

        public IEnumerable<ModuloItem> UsuarioItems { get; }

        public UsuarioExclusaoGrupoIntegrationEvent(IEnumerable<ModuloItem> usuarioItems)
        {
            UsuarioItems = usuarioItems;
        }
    }
}
