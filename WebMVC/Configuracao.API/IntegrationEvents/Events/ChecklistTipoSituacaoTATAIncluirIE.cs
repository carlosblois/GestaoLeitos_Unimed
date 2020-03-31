using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ChecklistTipoSituacaoTATAIncluirIE : IntegrationEvent
    {
        public int ChecklistId { get; set; }
        public int TipoSituacaoAcomodacaoId { get; set; }
        public int TipoAtividadeAcomodacaoId { get; set; }
        public int TipoAcomodacaoId { get; set; }
        public int EmpresaId { get; set; }
        public int CheckTSTATId { get; set; }


        public ChecklistTipoSituacaoTATAIncluirIE(int checklistId, 
                                                                                        int tipoSituacaoAcomodacaoId,
                                                                                        int tipoAtividadeAcomodacaoId, 
                                                                                        int tipoAcomodacaoId, 
                                                                                        int empresaId, 
                                                                                        int checkTSTATId)
        {
            ChecklistId = checklistId;
            TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
            TipoAtividadeAcomodacaoId = tipoAtividadeAcomodacaoId;
            TipoAcomodacaoId = tipoAcomodacaoId;
            EmpresaId = empresaId;
            CheckTSTATId = checkTSTATId;
        }

    }
}
