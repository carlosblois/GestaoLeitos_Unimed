using EventBus.Events;
using System;

namespace Setor.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaInclusaoIE : IntegrationEvent
    {
        public Guid EmpresaId { get; private set; }
        public string EmpresaNome { get; private set; }

        public EmpresaInclusaoIE(Guid empresaId, string empresaNome)
        {
            EmpresaId = empresaId;
            EmpresaNome = empresaNome;
        }
    }
}
