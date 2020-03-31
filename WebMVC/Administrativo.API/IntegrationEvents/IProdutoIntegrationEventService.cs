using EventBus.Events;
using System.Threading.Tasks;

namespace Produto.API.IntegrationEvents
{
    public interface IProdutoIntegrationEventService
    {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
