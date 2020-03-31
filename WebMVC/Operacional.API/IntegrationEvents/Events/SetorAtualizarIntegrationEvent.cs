using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API.IntegrationEvents.Events
{
    public class SetorAtualizarIntegrationEvent : IntegrationEvent
    {
        public Guid EmpresaId { get; private set; }
        public Guid SetorId { get; private set; }
        public string SetorNome { get; private set; }

        public SetorAtualizarIntegrationEvent(Guid empresaId, Guid setorId, string setorNome)
        {
            EmpresaId = empresaId;
            SetorId = setorId;
            SetorNome = setorNome;
        }

    }
}
