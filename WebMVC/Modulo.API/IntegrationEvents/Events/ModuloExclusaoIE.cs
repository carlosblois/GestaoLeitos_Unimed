using EventBus.Events;
using System;
using System.Collections.Generic;
using Modulo.API.Model;

namespace Modulo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ModuloExclusaoIE : IntegrationEvent
    {
        public int ModuloId { get; private set; }

        public ModuloExclusaoIE(int moduloId)
        {
            ModuloId = moduloId;
        }

    }
    public class ModuloExclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<ModuloItem>ModuloItems { get; }

        public ModuloExclusaoGrupoIE(IEnumerable<ModuloItem> moduloItems)
        {
            ModuloItems = moduloItems;
        }
    }
}
