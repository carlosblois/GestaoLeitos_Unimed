using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaPerfilExclusaoIE : IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int PerfilId { get; private set; }

        public EmpresaPerfilExclusaoIE(int emrpesaId, int perfilId)
        {
            EmpresaId = emrpesaId;
            PerfilId = perfilId;
        }

    }
}
