using EventBus.Events;
using System.Threading.Tasks;
using Usuario.API.Model;

namespace Usuario.API.IntegrationEvents
{
    public interface IUsuarioIntegrationEventService
    {
        
        Task SaveEventAndUsuarioChangesAsync(IntegrationEvent evt);

        Task SaveEventAndUsuarioContextChangesAsync(IntegrationEvent evt, UsuarioItem usuarioToSave);
        Task SaveEventAndEmpresaPerfilContextChangesAsync(IntegrationEvent evt);
        Task SaveEventAndUsuarioEmpresaPerfilContextChangesAsync(IntegrationEvent evt);
        Task SaveEventAndPerfilContextChangesAsync(IntegrationEvent evt, PerfilItem perfilToSave);


        Task DeleteEventAndEmpresaPerfilContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndUsuarioEmpresaPerfilContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndUsuarioContextChangesAsync(IntegrationEvent evt);

        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        Task ProcessedThroughEventBusAsync(IntegrationEvent evt);
    }
}
