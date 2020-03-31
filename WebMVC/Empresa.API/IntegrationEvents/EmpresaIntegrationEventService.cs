using EventBus.Events;
using EventBus.Abstractions;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Empresa.API.Infrastructure;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Empresa.API.IntegrationEvents
{
    public class EmpresaIntegrationEventService : IEmpresaIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly EmpresaContext _empresaContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public EmpresaIntegrationEventService(IEventBus eventBus, EmpresaContext empresaContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _empresaContext = empresaContext ?? throw new ArgumentNullException(nameof(empresaContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_empresaContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);

            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_empresaContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _empresaContext.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _empresaContext.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }

}
