using EventBus.Events;
using System.Threading.Tasks;

namespace Empresa.API.IntegrationEvents
{
    public interface IEmpresaIntegrationEventService
    {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
