using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class TipoSituacaoAcomodacaoSaveIE : IntegrationEvent
    {
        public int TipoSituacaoAcomodacaoId{ get;  set; }
        public string TipoSituacaoAcomodacaoNome { get; private set; }

        public TipoSituacaoAcomodacaoSaveIE(int tipoSituacaoAcomodacaoId, string tipoSituacaoAcomodacaoNome)
        {
            TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
            TipoSituacaoAcomodacaoNome = tipoSituacaoAcomodacaoNome;
        }

    }
}
