using EventBus.Events;
using System.Threading.Tasks;

namespace Dame.API.Application.IntegrationEvents
{
    public interface IDameIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
