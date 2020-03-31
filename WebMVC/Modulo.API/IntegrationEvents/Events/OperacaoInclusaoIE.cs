using EventBus.Events;
using Modulo.API.Model;
using System.Collections.Generic;

namespace Modulo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class OperacaoInclusaoIE : IntegrationEvent
    {
        public int OperacaoId { get;  set; }
        public string OperacaoNome { get; private set; }
        public int ModuloId { get; private set; }

        public OperacaoInclusaoIE(int operacaoId, string operacaoNome, int moduloId)
        {
            OperacaoId = operacaoId;
            OperacaoNome = operacaoNome;
            ModuloId = moduloId;

        }

    }
    public class OperacaoInclusaoGrupoIE: IntegrationEvent
    {

        public IEnumerable<OperacaoItem> OperacaoItems { get; }

        public OperacaoInclusaoGrupoIE(IEnumerable<OperacaoItem> operacaoItems)
        {
            OperacaoItems = operacaoItems;
        }
    }
}
