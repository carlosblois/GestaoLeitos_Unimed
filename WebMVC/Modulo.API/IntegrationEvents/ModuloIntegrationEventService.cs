using EventBus.Events;
using EventBus.Abstractions;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using Modulo.API.Model;
using Microsoft.Extensions.Options;
using Modulo.API.Infrastructure;


namespace Modulo.API.IntegrationEvents
{
    public class ModuloIntegrationEventService : IModuloIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly ModuloContext _moduloContext;
        private readonly IntegrationEventLogContext _logContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public ModuloIntegrationEventService(ModuloContext moduloContext, IntegrationEventLogContext logContext,IEventBus eventBus, IOptionsSnapshot<ModuloSettings> settings,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            

            _moduloContext = moduloContext ?? throw new ArgumentNullException(nameof(moduloContext));
            _logContext = logContext ?? throw new ArgumentNullException(nameof(logContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_moduloContext.Database.GetDbConnection()); 
        }
        
        public async Task DeleteEventAndOperacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _moduloContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _moduloContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _moduloContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _moduloContext.Database.CurrentTransaction.GetDbTransaction());
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
        public async Task DeleteEventAndModuloContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _moduloContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _moduloContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _moduloContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _moduloContext.Database.CurrentTransaction.GetDbTransaction());
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


        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);

            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        public async Task ProcessedThroughEventBusAsync(IntegrationEvent evt)
        {
            await _eventLogService.MarkEventAsProcessedAsync(evt);
        }

        public async Task SaveEventAndModuloContextChangesAsync(IntegrationEvent evt, ModuloItem moduloToSave)
        {
            var strategy = _moduloContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _moduloContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _moduloContext.SaveChangesAsync ();
                        //Tratamento de Identity
                        ((Events.ModuloInclusaoIE)evt).ModuloId = moduloToSave.Id_Modulo;
                        //Tratamento relacionamento
                        ((Events.ModuloInclusaoIE)evt).OperacaoItems = moduloToSave.OperacaoItems;
                        ((Events.ModuloInclusaoIE)evt).EmpresaPerfilModuloItems = moduloToSave.EmpresaPerfilModuloItems;
                        await _eventLogService.SaveEventAsync(evt, _moduloContext.Database.CurrentTransaction.GetDbTransaction());
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
        
        public async Task SaveEventAndOperacaoContextChangesAsync(IntegrationEvent evt, OperacaoItem operacaoToSave)
        {
            var strategy = _moduloContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _moduloContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _moduloContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.OperacaoInclusaoIE)evt).OperacaoId = operacaoToSave.Id_Operacao;
                        await _eventLogService.SaveEventAsync(evt, _moduloContext.Database.CurrentTransaction.GetDbTransaction());
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


        
        public async Task SaveEventAndEmpresaPefilModuloContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _moduloContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _moduloContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _moduloContext.SaveChangesAsync();

                        await _eventLogService.SaveEventAsync(evt, _moduloContext.Database.CurrentTransaction.GetDbTransaction());
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
