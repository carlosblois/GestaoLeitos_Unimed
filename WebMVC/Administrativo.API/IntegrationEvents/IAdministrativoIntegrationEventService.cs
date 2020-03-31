using Administrativo.API.Model;
using EventBus.Events;
using System.Threading.Tasks;

namespace Administrativo.API.IntegrationEvents
{
    public interface IAdministrativoIntegrationEventService
    {
        Task SaveEventAndAcessoEmpresaPerfilTSTAContextChangesAsync(IntegrationEvent evt, AcessoEmpresaPerfilTSTAItem acessoEmpresaPerfilTSTAToSave);
        Task SaveEventAndTipoAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task SaveEventAndSetorContextChangesAsync(IntegrationEvent evt, SetorItem setorToSave);
        Task SaveEventAndCaracteristicaAcomodacaoContextChangesAsync(IntegrationEvent evt, CaracteristicaAcomodacaoItem caracteristicaAcomodacaoToSave);
        Task SaveEventAndEmpresaContextChangesAsync(IntegrationEvent evt, EmpresaItem empresaToSave);
        Task SaveEventAndAcomodacaoContextChangesAsync(IntegrationEvent evt, AcomodacaoItem acomodacaoToSave);

        Task DeleteEventAndAcessoEmpresaPerfilTSTContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndTipoAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndSetorContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndCaracteristicaAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndEmpresaContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndAcomodacaoContextChangesAsync(IntegrationEvent evt);

        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        Task ProcessedThroughEventBusAsync(IntegrationEvent evt);
    }
}
