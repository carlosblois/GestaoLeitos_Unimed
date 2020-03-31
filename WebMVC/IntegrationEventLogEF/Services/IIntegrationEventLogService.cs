using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace IntegrationEventLogEF
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);
        Task SaveEventAsync(List<IntegrationEvent> @event, DbTransaction transaction);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
        Task MarkEventAsProcessedAsync(IntegrationEvent @event);
        
    }
}
