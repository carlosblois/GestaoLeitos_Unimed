using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Operacional.API.Infrastructure;
using Operacional.API.IntegrationEvents.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Operacional.API.IntegrationEvents
{
    public class OperacionalIntegrationEventService : IOperacionalIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly OperacionalContext _operacionalContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly IIntegrationEventLogService _eventLogServiceSec;

        public OperacionalIntegrationEventService(IEventBus eventBus, OperacionalContext operacionalContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _operacionalContext = operacionalContext ?? throw new ArgumentNullException(nameof(operacionalContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_operacionalContext.Database.GetDbConnection());
            _eventLogServiceSec = _integrationEventLogServiceFactory(_operacionalContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);

            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        public async Task PublishThroughEventBusAsync(List<IntegrationEvent> lstEvt)
        {
            foreach (IntegrationEvent evt in lstEvt)
            {
                _eventBus.Publish(evt);

                await _eventLogService.MarkEventAsPublishedAsync(evt);
            }
        }

        public async Task ProcessedThroughEventBusAsync(IntegrationEvent evt)
        {
            await _eventLogService.MarkEventAsProcessedAsync(evt);
        }

        public async Task SaveEventAndPacienteContextChangesAsync(IntegrationEvent evt,PacienteItem pacienteToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.PacienteSaveIE)evt).PacienteId = pacienteToSave.Id_Paciente;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndGeraAcaoFinalizarAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, IntegrationEvent evtFIMATIVIDADE, AcaoItem acaoToSave, List<IntegrationEvent> lstEvt, List<AtividadeItem> lst, List<IntegrationEvent> lstRespEvt, List<RespostasChecklistItem> lstResposta)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {

                        await _operacionalContext.SaveChangesAsync();

                        int cont = 0;

                        if (lst != null)
                        {
                            foreach (AtividadeItem Atividade in lst)
                            {
                                AtividadeSaveIE evt = (Events.AtividadeSaveIE)lstEvt[cont];
                                //Tratamento de Identity
                                evt.AtividadeAcomodacaoId = Atividade.Id_AtividadeAcomodacao;
                                if (Atividade.AcaoItems != null)
                                {
                                    evt.AcaoItems[0].Id_AcaoAtividadeAcomodacao = Atividade.AcaoItems[0].Id_AcaoAtividadeAcomodacao;
                                }
                                cont = cont + 1;
                            }
                        }

                        //Tratamento de Identity
                        ((Events.GeraAcaoAcomodacaoIE)evtINI).AcaoAtividadeAcomodacaoId = acaoToSave.Id_AcaoAtividadeAcomodacao;

                        cont = 0;
                        if (lstResposta != null)
                        {
                            foreach (RespostasChecklistItem Resposta in lstResposta)
                            {
                                RespostaChecklistSaveIE evt = (Events.RespostaChecklistSaveIE)lstRespEvt[cont];
                                //Tratamento de Identity
                                evt.RespostasChecklistId = Resposta.Id_RespostasChecklist;
                                cont = cont + 1;
                                lstEvt.Add(evt);
                            }
                        }

                        lstEvt.Add(evtFIM);
                        lstEvt.Add(evtINI);
                        lstEvt.Add(evtFIMATIVIDADE);

                        await _eventLogService.SaveEventAsync(lstEvt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        
        public async Task SaveEventAndCancelaAtividadeAsync(IntegrationEvent EvtAT, IntegrationEvent EvtAC)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        lst.Add(EvtAT);
                        lst.Add(EvtAC);

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
        public async Task SaveEventAndCancelaAceiteAcaoAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, AcaoItem acaoToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();

                        List<IntegrationEvent> lst = new List<IntegrationEvent>();

                        //Tratamento de Identity
                        ((Events.GeraAcaoAcomodacaoSolicitarIE)evtINI).AcaoAtividadeAcomodacaoId = acaoToSave.Id_AcaoAtividadeAcomodacao;

                        lst.Add(evtFIM);
                        lst.Add(evtINI);

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndGeraAcaoAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM, AcaoItem acaoToSave,List<IntegrationEvent> lstAtvEvento)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();

                        List<IntegrationEvent> lst = new List<IntegrationEvent>();

                        //Tratamento de Identity
                        ((Events.GeraAcaoAcomodacaoIE)evtINI).AcaoAtividadeAcomodacaoId = acaoToSave.Id_AcaoAtividadeAcomodacao;

                        lst.Add(evtFIM);
                        lst.Add(evtINI);

                        foreach (IntegrationEvent AtvEVT in lstAtvEvento)
                        {
                            lst.Add(AtvEVT);
                        }

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
        
        public async Task SaveEventAndSituacaoContextChangesAsync(IntegrationEvent evt, SituacaoItem situacaoToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)evt).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAltaHospitalarAsync(IntegrationEvent EvtPacAcomodacao, IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);
                        lst.Add(EvtPacAcomodacao);
                       
                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
        
        public async Task SaveEventAndDeParaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave, List<IntegrationEvent> lstAtvEvento)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);

                        foreach (IntegrationEvent AtvEVT in lstAtvEvento)
                        {
                            lst.Add(AtvEVT);
                        }

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }


        public async Task SaveEventAndCancelamentoReservaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }


        public async Task SaveEventAndCancelamentoAltaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave, List<IntegrationEvent> lstAtvEvento)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);

                        foreach (IntegrationEvent AtvEVT in lstAtvEvento)
                        {
                            lst.Add(AtvEVT);
                        }

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAltaMedicaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU , SituacaoItem situacaoToSave, List<IntegrationEvent> lstAtvEvento)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);

                        foreach (IntegrationEvent AtvEVT in lstAtvEvento)
                        {
                            lst.Add(AtvEVT);
                        }

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAltaMedicaAsync(IntegrationEvent EvtSitS, IntegrationEvent EvtSitU, SituacaoItem situacaoToSave)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);

                        
                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
        public async Task SaveEventAndInternacaoAsync(IntegrationEvent EvtPac, IntegrationEvent EvtPacAc, IntegrationEvent EvtSitS, IntegrationEvent EvtSitU,  PacienteItem pacienteToSave, SituacaoItem situacaoToSave)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.PacienteSaveIE)EvtPac).PacienteId = pacienteToSave.Id_Paciente;
                        lst.Add(EvtPac);
                        ((Events.PacienteAcomodacaoSaveIE)EvtPacAc).PacienteId = pacienteToSave.Id_Paciente;
                        ((Events.PacienteAcomodacaoSaveIE)EvtPacAc).PacienteAcomodacaoId = pacienteToSave.PacienteAcomodacaoItems[0].Id_PacienteAcomodacao ;
                        lst.Add(EvtPacAc);

                        ((Events.SituacaoSaveIE)EvtSitS).SituacaoAcomodacaoId = situacaoToSave.Id_SituacaoAcomodacao ;
                        lst.Add(EvtSitS);
                        lst.Add(EvtSitU);
                        
                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }


        public async Task SaveEventAndTransferenciaAsync(IntegrationEvent EvtFimSitOr, 
                                                            IntegrationEvent EvtFimSitDs,
                                                            IntegrationEvent EvtSitNewOr, 
                                                            IntegrationEvent EvtSitNewDs,
                                                            IntegrationEvent EvtPacNewOr, 
                                                            IntegrationEvent EvtPacNewDs,
                                                            SituacaoItem sitOrToSave, 
                                                            SituacaoItem sitDsToSave,
                                                            PacienteAcomodacaoItem pacToSaveOr, 
                                                            PacienteAcomodacaoItem pacToSaveDs,
                                                            List<IntegrationEvent> lstAtvEvento)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        lst.Add(EvtFimSitOr);
                        lst.Add(EvtFimSitDs);

                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitNewOr).SituacaoAcomodacaoId = sitOrToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitNewOr);
                        ((Events.SituacaoSaveIE)EvtSitNewDs).SituacaoAcomodacaoId = sitDsToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitNewDs);


                        ((Events.PacienteAcomodacaoSaveIE)EvtPacNewOr).PacienteAcomodacaoId  = pacToSaveOr.Id_PacienteAcomodacao;
                        lst.Add(EvtPacNewOr);
                        ((Events.PacienteAcomodacaoSaveIE)EvtPacNewDs).PacienteAcomodacaoId = pacToSaveDs.Id_PacienteAcomodacao;
                        lst.Add(EvtPacNewDs);

                        foreach (IntegrationEvent AtvEVT in lstAtvEvento)
                        {
                            lst.Add(AtvEVT);
                        }

                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndTransferenciaAsync(IntegrationEvent EvtFimSitOr,
                                                            IntegrationEvent EvtFimSitDs,
                                                            IntegrationEvent EvtSitNewOr,
                                                            IntegrationEvent EvtSitNewDs,
                                                            IntegrationEvent EvtPacNewOr,
                                                            IntegrationEvent EvtPacNewDs,
                                                            SituacaoItem sitOrToSave,
                                                            SituacaoItem sitDsToSave,
                                                            PacienteAcomodacaoItem pacToSaveOr,
                                                            PacienteAcomodacaoItem pacToSaveDs)
        {
            List<IntegrationEvent> lst = new List<IntegrationEvent>();
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        lst.Add(EvtFimSitOr);
                        lst.Add(EvtFimSitDs);

                        //Tratamento de Identity
                        ((Events.SituacaoSaveIE)EvtSitNewOr).SituacaoAcomodacaoId = sitOrToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitNewOr);
                        ((Events.SituacaoSaveIE)EvtSitNewDs).SituacaoAcomodacaoId = sitDsToSave.Id_SituacaoAcomodacao;
                        lst.Add(EvtSitNewDs);


                        ((Events.PacienteAcomodacaoSaveIE)EvtPacNewOr).PacienteAcomodacaoId = pacToSaveOr.Id_PacienteAcomodacao;
                        lst.Add(EvtPacNewOr);
                        ((Events.PacienteAcomodacaoSaveIE)EvtPacNewDs).PacienteAcomodacaoId = pacToSaveDs.Id_PacienteAcomodacao;
                        lst.Add(EvtPacNewDs);


                        await _eventLogService.SaveEventAsync(lst, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
        public async Task SaveEventAndAcaoContextChangesAsync(IntegrationEvent evt, AcaoItem acaoToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.AcaoSaveIE)evt).AcaoAtividadeAcomodacaoId = acaoToSave.Id_AcaoAtividadeAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndMensagemContextChangesAsync(IntegrationEvent evt, MensagemItem mensagemToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.MensagemSaveIE)evt).id_Mensagem = mensagemToSave.Id_Mensagem;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }


        public async Task SaveEventAndMensagemRetornoContextChangesAsync(IntegrationEvent evt, MensagemItem mensagemToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.MensagemRetornoIE)evt).id_Mensagem = mensagemToSave.Id_Mensagem;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAtividadeContextChangesAsync(IntegrationEvent evt, AtividadeItem atividadeToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.AtividadeSaveIE)evt).AtividadeAcomodacaoId = atividadeToSave.Id_AtividadeAcomodacao;
                        SituacaoItem l_Situacao = _operacionalContext.SituacaoItems.Find(atividadeToSave.Id_SituacaoAcomodacao);
                        ((Events.AtividadeSaveIE)evt).IdAcomodacao = l_Situacao.Id_Acomodacao;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAtividadePriorizadaContextChangesAsync(IntegrationEvent evt, AtividadeItem atividadeToSave)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.AtividadePriorizadaIE)evt).AtividadeAcomodacaoId = atividadeToSave.Id_AtividadeAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

        public async Task SaveEventAndAtividadeContextChangesAsync(List<IntegrationEvent> lstEvt, List<AtividadeItem> lst)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _operacionalContext.SaveChangesAsync();
                        int cont =0;
                        foreach (AtividadeItem  Atividade in lst)
                        {
                            AtividadeSaveIE evt = (Events.AtividadeSaveIE)lstEvt[cont];
                            //Tratamento de Identity
                            evt.AtividadeAcomodacaoId = Atividade.Id_AtividadeAcomodacao;
                            if (Atividade.AcaoItems != null)
                            {
                                evt.AcaoItems[0].Id_AcaoAtividadeAcomodacao = Atividade.AcaoItems[0].Id_AcaoAtividadeAcomodacao;
                            }
                            cont = cont + 1;
                        }

                        await _eventLogService.SaveEventAsync(lstEvt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());
                        
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }
       

        public async Task SaveEventAndGeraAcaoFinalizarTotalAsync(IntegrationEvent evtINI, IntegrationEvent evtFIM,  IntegrationEvent evtFIMATIVIDADE,AcaoItem acaoToSave, List<IntegrationEvent> lstEvt, List<AtividadeItem> lst, List<IntegrationEvent> lstRespEvt, List<RespostasChecklistItem> lstResposta)
        {
            var strategy = _operacionalContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _operacionalContext.Database.BeginTransaction())
                {
                    try
                    {

                        await _operacionalContext.SaveChangesAsync();

                        int cont = 0;

                        if (lst != null)
                        {
                            foreach (AtividadeItem Atividade in lst)
                            {
                                AtividadeSaveIE evt = (Events.AtividadeSaveIE)lstEvt[cont];
                                //Tratamento de Identity
                                evt.AtividadeAcomodacaoId = Atividade.Id_AtividadeAcomodacao;
                                if (Atividade.AcaoItems != null)
                                {
                                    evt.AcaoItems[0].Id_AcaoAtividadeAcomodacao = Atividade.AcaoItems[0].Id_AcaoAtividadeAcomodacao;
                                }
                                cont = cont + 1;
                            }
                        }

                        //Tratamento de Identity
                        ((Events.GeraAcaoAcomodacaoIE)evtINI).AcaoAtividadeAcomodacaoId = acaoToSave.Id_AcaoAtividadeAcomodacao;

                        cont = 0;
                        if (lstResposta != null)
                        {
                            foreach (RespostasChecklistItem Resposta in lstResposta)
                            {
                                RespostaChecklistSaveIE evt = (Events.RespostaChecklistSaveIE)lstRespEvt[cont];
                                //Tratamento de Identity
                                evt.RespostasChecklistId = Resposta.Id_RespostasChecklist;
                                cont = cont + 1;
                                lstEvt.Add(evt);
                            }
                        }

                        lstEvt.Add(evtFIM);
                        lstEvt.Add(evtINI);
                        lstEvt.Add(evtFIMATIVIDADE);

                        await _eventLogService.SaveEventAsync(lstEvt, _operacionalContext.Database.CurrentTransaction.GetDbTransaction());

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var sqlException = ex.InnerException as System.Data.SqlClient.SqlException;
                        throw new Exception(sqlException.Number + "::" + sqlException.Message);
                    }
                }
            });
        }

    }

}
