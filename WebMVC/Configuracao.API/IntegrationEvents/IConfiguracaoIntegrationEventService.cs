using Configuracao.API.Model;
using EventBus.Events;
using System.Threading.Tasks;

namespace Configuracao.API.IntegrationEvents
{
    public interface IConfiguracaoIntegrationEventService
    {
        Task IncluirEventAndTipoSituacaoTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task IncluirEventAndChecklistItemChecklistContextChangesAsync(IntegrationEvent evt);
        Task IncluirEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(IntegrationEvent evt, ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave);
        Task IncluirEventAndFluxoAutomaticoAcaoContextChangesAsync(IntegrationEvent evt);
        Task IncluirEventAndFluxoAutomaticoSituacaoContextChangesAsync(IntegrationEvent evt);
        Task IncluirEventAndFluxoAutomaticoCheckContextChangesAsync(IntegrationEvent evt);

        Task SaveEventAndTipoSituacaoAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoSituacaoAcomodacaoItem tipoSituacaoAcomodacaoToSave);
        Task SaveEventAndTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoAtividadeAcomodacaoItem tipoAtividadeAcomodacaoToSave);
        Task SaveEventAndTipoAcaoAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoAcaoAcomodacaoItem tipoAcaoAcomodacaoToSave);
        Task SaveEventAndSLASituacaoContextChangesAsync(IntegrationEvent evt, SLASituacaoItem sLASituacaoToSave);
        Task SaveEventAndSLAContextChangesAsync(IntegrationEvent evt, SLAItem sLAToSave);
        Task SaveEventAndChecklistContextChangesAsync(IntegrationEvent evt, ChecklistItem checklistToSave);
        Task SaveEventAndItemChecklistContextChangesAsync(IntegrationEvent evt, ItemChecklistItem itemChecklistToSave);

        Task DeleteEventAndTipoSituacaoAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndTipoAcaoAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndTipoSituacaoTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndSLASituacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndSLAContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndChecklistContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndItemChecklistContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndChecklistItemChecklistContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndFluxoAutomaticoAcaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndFluxoAutomaticoSituacaoContextChangesAsync(IntegrationEvent evt);
        Task DeleteEventAndFluxoAutomaticoCheckContextChangesAsync(IntegrationEvent evt);


        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        Task ProcessedThroughEventBusAsync(IntegrationEvent evt);
    }
}
