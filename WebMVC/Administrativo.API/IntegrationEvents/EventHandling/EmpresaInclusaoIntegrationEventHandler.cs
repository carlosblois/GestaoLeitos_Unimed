

namespace TipoAcomodacao.API.IntegrationEvents.EventHandling
{

    using EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using global::TipoAcomodacao.API.IntegrationEvents.Events;
    using TipoAcomodacao.API.Infrastructure;
    using TipoAcomodacao.API.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Collections.Generic;
   // using Empresa.API.IntegrationEvents.Events;

    public class EmpresaInclusaoIntegrationEventHandler : IIntegrationEventHandler<EmpresaInclusaoIntegrationEvent>
    {

        private readonly TipoAcomodacaoContext _tipoAcomodacaoContext;
        private readonly ITipoAcomodacaoIntegrationEventService _tipoAcomodacaoIntegrationEventService;

        public EmpresaInclusaoIntegrationEventHandler(TipoAcomodacaoContext tipoAcomodacaoContext, ITipoAcomodacaoIntegrationEventService tipoAcomodacaoIntegrationEventService)
        {
            _tipoAcomodacaoContext = tipoAcomodacaoContext;
            _tipoAcomodacaoIntegrationEventService = tipoAcomodacaoIntegrationEventService;
        }

        public async Task Handle(EmpresaInclusaoIntegrationEvent @event)
        {
            await TipoAcomodacaoIncluir(@event.EmpresaId, @event);

        }

        private async Task TipoAcomodacaoIncluir(Guid EmpresaId, EmpresaInclusaoIntegrationEvent @event)
        {

            using (_tipoAcomodacaoContext)
            {
                List<TipoAcomodacaoItem> list = new List<TipoAcomodacaoItem>();
                var lstTipoAcomodacaoDefaultNames = _tipoAcomodacaoContext.TipoAcomodacaoDefaultItems.FromSql("SELECT * FROM DefaultTipoAcomodacao").ToList();
                foreach (TipoAcomodacaoDefaultItem it in lstTipoAcomodacaoDefaultNames)
                {

                    Guid gKey;
                    gKey = Guid.NewGuid();
                    TipoAcomodacaoItem tipoAcomodacaoToSave = new TipoAcomodacaoItem();
                    tipoAcomodacaoToSave.Id_TipoAcomodacao = gKey;
                    tipoAcomodacaoToSave.Id_Empresa = EmpresaId;
                    tipoAcomodacaoToSave.Nome_TipoAcomodacao = it.Nome_TipoAcomodacao;

                    // Update current setor
                    _tipoAcomodacaoContext.TipoAcomodacaoItems.Add(tipoAcomodacaoToSave);

                    list.Add(tipoAcomodacaoToSave);

                }
                var tipoAcomodacaoInclusaoEvent = new TipoAcomodacaoInclusaoGrupoIntegrationEvent(list);

                await _tipoAcomodacaoIntegrationEventService.SaveEventAndTipoAcomodacaoContextChangesAsync(tipoAcomodacaoInclusaoEvent);
                await _tipoAcomodacaoIntegrationEventService.PublishThroughEventBusAsync(tipoAcomodacaoInclusaoEvent);

                //Processado evento EMPRESA
                await _tipoAcomodacaoIntegrationEventService.ProcessedThroughEventBusAsync(@event);
            }

        }
    }
}

