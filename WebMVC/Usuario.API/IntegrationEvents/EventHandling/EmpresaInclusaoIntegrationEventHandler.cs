

namespace Usuario.API.IntegrationEvents.EventHandling
{
    using EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using global::Setor.API.IntegrationEvents.Events;
    using Usuario.API.Infrastructure;
    using Usuario.API.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Collections.Generic;

    public class EmpresaInclusaoIntegrationEventHandler : IIntegrationEventHandler<EmpresaInclusaoIntegrationEvent>
    {
        private readonly UsuarioContext _setorContext;
        private readonly IUsuarioIntegrationEventService _setorIntegrationEventService;


        public EmpresaInclusaoIntegrationEventHandler(UsuarioContext setorContext, IUsuarioIntegrationEventService setorIntegrationEventService)
        {
            _setorContext = setorContext;
            _setorIntegrationEventService = setorIntegrationEventService;
        }

        public async Task Handle(EmpresaInclusaoIntegrationEvent @event)
        {
            await SetorIncluir(@event.EmpresaId, @event);
            
        }

        private async Task SetorIncluir(Guid EmpresaId, EmpresaInclusaoIntegrationEvent @event)
        {

            //using (_setorContext)
            //{
            //    List<UsuarioItem> list = new List<UsuarioItem>();
            //    //Processado evento EMPRESA
            //    await _setorIntegrationEventService.ProcessedThroughEventBusAsync(@event);
            //}

        }
    }
}

