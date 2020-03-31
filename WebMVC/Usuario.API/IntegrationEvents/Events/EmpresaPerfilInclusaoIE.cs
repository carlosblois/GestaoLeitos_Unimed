using EventBus.Events;
using System;
using System.Collections.Generic;
using Usuario.API.Model;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaPerfilInclusaoIE: IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int PerfilId { get; private set; }

        public EmpresaPerfilInclusaoIE(int emrpesaId, int perfilId)
        {
            EmpresaId = emrpesaId;
            PerfilId = perfilId;
        }

    }

    public class EmpresaPerfilInclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<EmpresaPerfilItem> EmpresaPerfilItems { get; }

        public EmpresaPerfilInclusaoGrupoIE(IEnumerable<EmpresaPerfilItem> empresaPerfilItem)
        {
            EmpresaPerfilItems = empresaPerfilItem;
        }
    }
}
