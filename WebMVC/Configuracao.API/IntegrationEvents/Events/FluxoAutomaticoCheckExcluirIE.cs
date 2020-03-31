using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class FluxoAutomaticoCheckExcluirIE : IntegrationEvent
    {

        public int ChecklistId { get; set; }
        public int TipoSituacaoAcomodacaoId { get; set; }
        public int ItemChecklistId { get; set; }
        public int TipoAtividadeAcomodacaoId { get; set; }


        public FluxoAutomaticoCheckExcluirIE(int checklistId, int tipoSituacaoAcomodacaoId, int itemChecklistId, int tipoAtividadeAcomodacaoId)
        {

            ChecklistId = checklistId;
            TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
            ItemChecklistId = itemChecklistId;
            TipoAtividadeAcomodacaoId = tipoAtividadeAcomodacaoId;

        }

    }
}
