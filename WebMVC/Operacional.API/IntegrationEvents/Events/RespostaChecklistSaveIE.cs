using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class RespostaChecklistSaveIE : IntegrationEvent
    {
        public int ChecklistId { get; set; }
        public int ItemChecklistId { get; set; }
        public string Valor { get; set; }
        public int AtividadeAcomodacaoId { get; set; }
        public int CheckTSTATId { get; set; }
        public int RespostasChecklistId { get; set; }


        public RespostaChecklistSaveIE( int checklistId, int itemChecklistId,
            string valor, int atividadeAcomodacaoId,
            int respostasChecklistId, int checkTSTATId)
        {

            ChecklistId = checklistId;
            ItemChecklistId = itemChecklistId;
            Valor = valor;
            AtividadeAcomodacaoId = atividadeAcomodacaoId;
            RespostasChecklistId = respostasChecklistId;
            CheckTSTATId = checkTSTATId;
        }

    }
}
