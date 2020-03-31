using EventBus.Events;
using TipoAcomodacao.API.Model;
using System;
using System.Collections.Generic;

namespace TipoAcomodacao.API.IntegrationEvents.EventHandling
{

        // Integration Events notes: 
        // An Event is “something that has happened in the past”, therefore its name has to be   
        // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
        public class TipoAcomodacaoInclusaoIntegrationEvent : IntegrationEvent
        {
            public Guid EmpresaId { get; private set; }
            public Guid TipoAcomodacaoId { get; private set; }
            public string TipoAcomodacaoNome { get; private set; }

            public TipoAcomodacaoInclusaoIntegrationEvent(Guid empresaId, Guid tipoAcomodacaoId, string tipoAcomodacaoNome)
            {
                EmpresaId = empresaId;
                TipoAcomodacaoId = tipoAcomodacaoId;
                TipoAcomodacaoNome = tipoAcomodacaoNome;
            }

        }
        public class TipoAcomodacaoInclusaoGrupoIntegrationEvent : IntegrationEvent
        {

            public IEnumerable<TipoAcomodacaoItem> TipoAcomodacaoItems { get; }

            public TipoAcomodacaoInclusaoGrupoIntegrationEvent(IEnumerable<TipoAcomodacaoItem> tipoAcomodacaoItems)
            {
                TipoAcomodacaoItems = tipoAcomodacaoItems;
            }
        }

}
