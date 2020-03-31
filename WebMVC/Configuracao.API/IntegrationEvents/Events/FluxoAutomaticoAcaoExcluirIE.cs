using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class FluxoAutomaticoAcaoExcluirIE : IntegrationEvent
    {
        public int TipoAcaoAcomodacaoOrigemId { get; set; }
        public int TipoAtividadeAcomodacaoOrigemId { get; set; }
        public int TipoSituacaoAcomodacaoOrigemId { get; set; }

        public int TipoAtividadeAcomodacaoDestinoId { get; set; }
        public int TipoSituacaoAcomodacaoDestinoId { get; set; }

        public int EmpresaId { get; set; }


        public FluxoAutomaticoAcaoExcluirIE(int tipoSituacaoAcomodacaoOrigemId, int tipoAtividadeAcomodacaoOrigemId, int tipoAcaoAcomodacaoOrigemId, int tipoSituacaoAcomodacaoDestinoId, int tipoAtividadeAcomodacaoDestinoId , int empresaId)
        {
            TipoAcaoAcomodacaoOrigemId = tipoAcaoAcomodacaoOrigemId;
            TipoAtividadeAcomodacaoOrigemId = tipoAtividadeAcomodacaoOrigemId;
            TipoSituacaoAcomodacaoOrigemId = tipoSituacaoAcomodacaoOrigemId;

            TipoAtividadeAcomodacaoDestinoId = tipoAtividadeAcomodacaoDestinoId;
            TipoSituacaoAcomodacaoDestinoId = tipoSituacaoAcomodacaoDestinoId;

            EmpresaId = empresaId;
        }

    }
}
