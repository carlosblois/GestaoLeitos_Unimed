using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Administrativo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AcessoEmpresaPerfilTSTAExclusaoIE : IntegrationEvent
    {
        public int AcomodacaoId { get; private set; }

        public AcessoEmpresaPerfilTSTAExclusaoIE(int acomodacaoId)
        {
            AcomodacaoId = acomodacaoId;
        }

    }

    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AcessoEmpresaPerfilTSTAExclusaoTodosIntegrationEvent : IntegrationEvent
    {
        public int IdEmpresa { get; private set; }
        public int IdPerfil { get; private set; }

        public AcessoEmpresaPerfilTSTAExclusaoTodosIntegrationEvent(int empresaId, int perfilId)
        {
            IdEmpresa = empresaId;
            IdPerfil = perfilId;

        }

    }
}
