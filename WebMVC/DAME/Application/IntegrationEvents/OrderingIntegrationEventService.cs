using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using IntegrationEventLogEF;
using Services.Dame.Infrastructure;

namespace Dame.API.Application.IntegrationEvents
{
    public class DameIntegrationEventService : IDameIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly DameContext _DameContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public DameIntegrationEventService(IEventBus eventBus, DameContext DameContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _DameContext = DameContext ?? throw new ArgumentNullException(nameof(DameContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_DameContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            await SaveEventAndDameContextChangesAsync(evt);
            _eventBus.Publish(evt);
            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        private async Task SaveEventAndDameContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_DameContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original Dame database operation and the IntegrationEventLog thanks to a local transaction
                    await _DameContext.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _DameContext.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}
