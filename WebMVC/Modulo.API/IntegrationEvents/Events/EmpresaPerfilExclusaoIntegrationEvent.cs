using EventBus.Events;
using Usuario.API.Model;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaPerfilExclusaoIntegrationEvent : IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int PerfilId { get; private set; }

        public EmpresaPerfilExclusaoIntegrationEvent(int emrpesaId, int perfilId)
        {
            EmpresaId = emrpesaId;
            PerfilId = perfilId;
        }

    }
}
