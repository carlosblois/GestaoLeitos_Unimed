using EventBus.Events;
using EventBus.Abstractions;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Configuracao.API.Infrastructure;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using Configuracao.API.Model;

namespace Configuracao.API.IntegrationEvents
{
    public class ConfiguracaoIntegrationEventService : IConfiguracaoIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly ConfiguracaoContext _configuracaoContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public ConfiguracaoIntegrationEventService(IEventBus eventBus, ConfiguracaoContext configuracaoContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _configuracaoContext = configuracaoContext ?? throw new ArgumentNullException(nameof(configuracaoContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_configuracaoContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);

            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        public async Task ProcessedThroughEventBusAsync(IntegrationEvent evt)
        {
            await _eventLogService.MarkEventAsProcessedAsync(evt);
        }

        public async Task IncluirEventAndFluxoAutomaticoSituacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task IncluirEventAndFluxoAutomaticoCheckContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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


        public async Task IncluirEventAndFluxoAutomaticoAcaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task IncluirEventAndTipoSituacaoTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task IncluirEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(IntegrationEvent evt, ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.ChecklistTipoSituacaoTATAIncluirIE)evt).CheckTSTATId = ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoToSave.Id_CheckTSTAT;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task IncluirEventAndChecklistItemChecklistContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndTipoSituacaoAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoSituacaoAcomodacaoItem tipoSituacaoAcomodacaoToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.TipoSituacaoAcomodacaoSaveIE)evt).TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoToSave.Id_TipoSituacaoAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoAtividadeAcomodacaoItem tipoAtividadeAcomodacaoToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.TipoAtividadeAcomodacaoSaveIE)evt).TipoAtividadeAcomodacaoId = tipoAtividadeAcomodacaoToSave.Id_TipoAtividadeAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndTipoAcaoAcomodacaoContextChangesAsync(IntegrationEvent evt, TipoAcaoAcomodacaoItem tipoAcaoAcomodacaoToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.TipoAcaoAcomodacaoSaveIE)evt).TipoAcaoAcomodacaoId = tipoAcaoAcomodacaoToSave.Id_TipoAcaoAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndSLASituacaoContextChangesAsync(IntegrationEvent evt, SLASituacaoItem sLASituacaoToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SLASituacaoSaveIE)evt).SLASituacaoId = sLASituacaoToSave.Id_SLA;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndSLAContextChangesAsync(IntegrationEvent evt, SLAItem sLAToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SLASaveIE)evt).SLAId = sLAToSave.Id_SLA;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndChecklistContextChangesAsync(IntegrationEvent evt, ChecklistItem checklistToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.ChecklistSaveIE)evt).ChecklistId= checklistToSave.Id_Checklist;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndItemChecklistContextChangesAsync(IntegrationEvent evt, ItemChecklistItem itemChecklistToSave)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _configuracaoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.ItemChecklistSaveIE)evt).ItemChecklistId = itemChecklistToSave.Id_ItemChecklist;
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndTipoSituacaoAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndChecklistItemChecklistContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndTipoAcaoAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndTipoSituacaoTipoAtividadeAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndSLASituacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndSLAContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndChecklistContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndItemChecklistContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndFluxoAutomaticoCheckContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndFluxoAutomaticoSituacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndFluxoAutomaticoAcaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _configuracaoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _configuracaoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _configuracaoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _configuracaoContext.Database.CurrentTransaction.GetDbTransaction());
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
