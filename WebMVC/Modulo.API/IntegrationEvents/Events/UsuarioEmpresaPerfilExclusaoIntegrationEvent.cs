using EventBus.Events;
using Usuario.API.Model;
using System;
using System.Collections.Generic;

namespace Usuario.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class UsuarioEmpresaPerfilExclusaoIntegrationEvent : IntegrationEvent
    {
        public int EmpresaId { get; private set; }
        public int UsuarioId { get; private set; }
        public int PerfilId { get; private set; }

        public UsuarioEmpresaPerfilExclusaoIntegrationEvent(int emrpesaId, int usuarioId, int perfilId)
        {
            EmpresaId = emrpesaId;
            UsuarioId = usuarioId;
            PerfilId = perfilId;
        }

    }
    public class UsuarioEmpresaPerfilExclusaoGrupoIntegrationEvent : IntegrationEvent
    {

        public IEnumerable<UsuarioEmpresaPerfilItem> EmpresaPerfilItems { get; }

        public UsuarioEmpresaPerfilExclusaoGrupoIntegrationEvent(IEnumerable<UsuarioEmpresaPerfilItem> empresaPerfilItem)
        {
            EmpresaPerfilItems = empresaPerfilItem;
        }
    }
    public class UsuarioEmpresaPerfilExclusaoTodosIntegrationEvent : IntegrationEvent
    {

        public IEnumerable<UsuarioEmpresaPerfilItem> EmpresaPerfilItems { get; }

        public UsuarioEmpresaPerfilExclusaoTodosIntegrationEvent(IEnumerable<UsuarioEmpresaPerfilItem> empresaPerfilItem)
        {
            EmpresaPerfilItems = empresaPerfilItem;
        }
    }
}
