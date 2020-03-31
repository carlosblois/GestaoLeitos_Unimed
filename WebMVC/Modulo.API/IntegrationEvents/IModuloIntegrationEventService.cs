using EventBus.Events;
using Modulo.API.Model;
using System.Threading.Tasks;

namespace Modulo.API.IntegrationEvents
{
    public interface IModuloIntegrationEventService
    {
        Task SaveEventAndModuloContextChangesAsync(IntegrationEvent evt, ModuloItem moduloToSave);
        Task SaveEventAndOperacaoContextChangesAsync(IntegrationEvent evt, OperacaoItem operacaoToSave);
        Task SaveEventAndEmpresaPefilModuloContextChangesAsync(IntegrationEvent evt);
        

        Task DeleteEventAndModuloContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndOperacaoContextChangesAsync(IntegrationEvent evt);

        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        Task ProcessedThroughEventBusAsync(IntegrationEvent evt);
    }
}
