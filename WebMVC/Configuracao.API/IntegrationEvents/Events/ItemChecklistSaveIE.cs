using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ItemChecklistSaveIE : IntegrationEvent
    {
        public int ItemChecklistId { get; set; }
        public string ItemChecklistNome { get; private set; }

        public ItemChecklistSaveIE(int itemChecklistId, string itemChecklistNome)
        {
            ItemChecklistId = itemChecklistId;
            ItemChecklistNome = itemChecklistNome;
        }

    }
}
