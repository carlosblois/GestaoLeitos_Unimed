using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ChecklistSaveIE : IntegrationEvent
    {
        public int ChecklistId { get; set; }
        public string ChecklistNome { get; private set; }

        public ChecklistSaveIE(int checklistId, string checklistNome)
        {
            ChecklistId = checklistId;
            ChecklistNome = checklistNome;
        }

    }
}
