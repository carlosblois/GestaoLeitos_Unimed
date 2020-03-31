

namespace Setor.API.IntegrationEvents.EventHandling
{
    using EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using global::Setor.API.IntegrationEvents.Events;
    using Setor.API.Infrastructure;
    using Setor.API.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Collections.Generic;

    public class EmpresaInclusaoIntegrationEventHandler : IIntegrationEventHandler<EmpresaInclusaoIntegrationEvent>
    {
        private readonly SetorContext _setorContext;
        private readonly ISetorIntegrationEventService _setorIntegrationEventService;


        public EmpresaInclusaoIntegrationEventHandler(SetorContext setorContext, ISetorIntegrationEventService setorIntegrationEventService)
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

            using (_setorContext)
            {
                List<SetorItem> list = new List<SetorItem>();
                var lstSetorDefaultNames = _setorContext.SetorDefaultItems.FromSql("SELECT * FROM DefaultSetor").ToList();
                foreach (SetorDefaultItem it in lstSetorDefaultNames)
                {

                    Guid gKey;
                    gKey = Guid.NewGuid();
                    SetorItem setorToSave = new SetorItem();
                    setorToSave.id_Setor = gKey;
                    setorToSave.id_Empresa = EmpresaId;
                    setorToSave.nome_Setor = it.Nome_Setor;
                    
                    // Update current setor
                    _setorContext.SetorItems.Add(setorToSave);

                    list.Add(setorToSave);

                }
                //var setorInclusaoEvent = new SetorInclusaoGrupoIntegrationEvent(list);

                //await _setorIntegrationEventService.SaveEventAndSetorContextChangesAsync(setorInclusaoEvent);
                //await _setorIntegrationEventService.PublishThroughEventBusAsync(setorInclusaoEvent);

                //Processado evento EMPRESA
                await _setorIntegrationEventService.ProcessedThroughEventBusAsync(@event);
            }

        }
    }
}

