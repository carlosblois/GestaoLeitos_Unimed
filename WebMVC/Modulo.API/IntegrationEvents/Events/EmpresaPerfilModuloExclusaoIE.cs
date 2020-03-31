using EventBus.Events;
using Modulo.API.Model;
using System.Collections.Generic;

namespace Modulo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaPerfilModuloExclusaoIE : IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int PerfilId { get; private set; }
        public int ModuloId { get; private set; }

        public EmpresaPerfilModuloExclusaoIE(int empresaId, int perfilId, int moduloId)
        {
            EmpresaId = empresaId;
            PerfilId = perfilId;
            ModuloId = moduloId;

        }

    }
    public class EmpresaPerfilModuloExclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<EmpresaPerfilModuloItem> EmpresaPerfilModuloItems { get; }

        public EmpresaPerfilModuloExclusaoGrupoIE(IEnumerable<EmpresaPerfilModuloItem> empresaPerfilModuloItems)
        {
            EmpresaPerfilModuloItems = empresaPerfilModuloItems;
        }
    }
}
