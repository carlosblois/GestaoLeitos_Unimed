using EventBus.Events;

using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class SetorInclusaoIntegrationEvent : IntegrationEvent
    {
        public Guid EmpresaId { get; private set; }
        public Guid SetorId { get; private set; }
        public string SetorNome { get; private set; }

        public SetorInclusaoIntegrationEvent(Guid empresaId, Guid setorId, string setorNome)
        {
            EmpresaId = empresaId;
            SetorId = setorId;
            SetorNome = setorNome;
        }        

    }
    public class SetorInclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<SetorItem> SetorItems { get; }

        public SetorInclusaoGrupoIE(IEnumerable<SetorItem> setorItems)
        {
            SetorItems = setorItems;
        }
    }
}
