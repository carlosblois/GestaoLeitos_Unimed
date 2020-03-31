using EventBus.Events;
using Operacional.API.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operacional.API.IntegrationEvents
{
    public interface IOperacionalIntegrationEventService
    {
        
        Task SaveEventAndCancelaAtividadeAsync(IntegrationEvent evtAT, IntegrationEvent evtAC);

        Task SaveEventAndAcaoContextChangesAsync(IntegrationEvent evt, AcaoItem acaoToSave);
        Task SaveEventAndMensagemContextChangesAsync(IntegrationEvent evt, MensagemItem mensagemToSave);
        Task SaveEventAndMensagemRetornoContextChangesAsync(IntegrationEvent evt, MensagemItem mensagemToSave);
        Task SaveEventAndDeParaAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, SituacaoItem situacaoToSave, List<IntegrationEvent> lstAtvEvento);
        Task SaveEventAndCancelamentoReservaAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, SituacaoItem situacaoToSave);
        Task SaveEventAndCancelaAceiteAcaoAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, AcaoItem acaoToSave);
        Task SaveEventAndGeraAcaoAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, AcaoItem acaoToSave, List<IntegrationEvent> lstAtvEvento);
        Task SaveEventAndGeraAcaoFinalizarAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, IntegrationEvent evtFIMATIVIDADE, AcaoItem acaoToSave, List<IntegrationEvent> lstEvt, List<AtividadeItem> lst, List<IntegrationEvent> lstRespEvt, List<RespostasChecklistItem> lstResposta);
        Task SaveEventAndGeraAcaoFinalizarTotalAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM,IntegrationEvent evtFIMATIVIDADE, AcaoItem acaoToSave, List<IntegrationEvent> lstEvt, List<AtividadeItem> lst, List<IntegrationEvent> lstRespEvt, List<RespostasChecklistItem> lstResposta);
        Task SaveEventAndAtividadeContextChangesAsync(List<IntegrationEvent> evt,List<AtividadeItem>lst);
        Task SaveEventAndAtividadeContextChangesAsync(IntegrationEvent evt, AtividadeItem atividadeToSave);
        Task SaveEventAndAtividadePriorizadaContextChangesAsync(IntegrationEvent evt, AtividadeItem atividadeToSave);
        Task SaveEventAndSituacaoContextChangesAsync(IntegrationEvent evt, SituacaoItem situacaoToSave);

        Task SaveEventAndPacienteContextChangesAsync(IntegrationEvent evt, PacienteItem pacienteToSave);


        Task SaveEventAndInternacaoAsync(IntegrationEvent EvtPac, IntegrationEvent EvtPacAc, IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, PacienteItem pacienteToSave, SituacaoItem situacaoToSave);
        Task SaveEventAndAltaMedicaAsync( IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave);
        Task SaveEventAndAltaHospitalarAsync(IntegrationEvent EvtPacAcomodacao, IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave);
        Task SaveEventAndCancelamentoAltaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave, List<IntegrationEvent> lstAtvEvento);
        

        Task SaveEventAndTransferenciaAsync(IntegrationEvent EvtFimSitOr, IntegrationEvent EvtFimSitDs,
                                            IntegrationEvent EvtSitNewOr, IntegrationEvent EvtSitNewDs,
                                            IntegrationEvent EvtPacNewOr, IntegrationEvent EvtPacNewDs,
                                            SituacaoItem sitOrToSave, SituacaoItem sitDsToSave,
                                            PacienteAcomodacaoItem pacToSaveOr, PacienteAcomodacaoItem pacToSaveDs);

        Task PublishThroughEventBusAsync(List<IntegrationEvent> lstEvt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);

        Task ProcessedThroughEventBusAsync(IntegrationEvent evt);
    }
}
