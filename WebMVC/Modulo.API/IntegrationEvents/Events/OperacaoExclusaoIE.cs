using EventBus.Events;
using Modulo.API.Model;
using System.Collections.Generic;

namespace Modulo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class OperacaoExclusaoIE : IntegrationEvent
    {
        public int OperacaoId { get; set; }

        public OperacaoExclusaoIE(int operacaoId)
        {
            OperacaoId = operacaoId;
        }

    }
    public class OperacaoExclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<OperacaoItem> OperacaoItems { get; }

        public OperacaoExclusaoGrupoIE(IEnumerable<OperacaoItem> operacaoItems)
        {
            OperacaoItems = operacaoItems;
        }
    }
}
