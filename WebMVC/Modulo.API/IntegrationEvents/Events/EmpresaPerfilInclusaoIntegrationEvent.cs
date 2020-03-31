using EventBus.Events;
using Usuario.API.Model;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaPerfilInclusaoIntegrationEvent : IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int PerfilId { get; private set; }

        public EmpresaPerfilInclusaoIntegrationEvent(int emrpesaId, int perfilId)
        {
            EmpresaId = emrpesaId;
            PerfilId = perfilId;
        }

    }

    public class EmpresaPerfilInclusaoGrupoIntegrationEvent : IntegrationEvent
    {

        public IEnumerable<EmpresaPerfilItem> EmpresaPerfilItems { get; }

        public EmpresaPerfilInclusaoGrupoIntegrationEvent(IEnumerable<EmpresaPerfilItem> empresaPerfilItem)
        {
            EmpresaPerfilItems = empresaPerfilItem;
        }
    }
}
