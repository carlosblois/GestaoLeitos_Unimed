using EventBus.Events;
using Usuario.API.Model;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class PerfilSaveIE : IntegrationEvent
    {
        public int PerfilId { get;  set; }
        public string PerfilNome { get; private set; }
        public List<EmpresaPerfilItem> EmpresaPerfilItems { get; set; }

        public PerfilSaveIE(int perfilId, string perfilNome)
        {
            PerfilId = perfilId;
            PerfilNome = perfilNome;
        }

    }
    public class PerfilSaveGrupoIE : IntegrationEvent
    {

        public IEnumerable<PerfilItem> PerfilItems { get; }

        public PerfilSaveGrupoIE(IEnumerable<PerfilItem> perfilItems)
        {
            PerfilItems = perfilItems;
        }
    }
}
