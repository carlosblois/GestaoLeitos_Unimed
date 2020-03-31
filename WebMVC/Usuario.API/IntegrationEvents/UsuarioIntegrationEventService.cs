using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using Usuario.API.Infrastructure;
using Usuario.API.Model;

namespace Usuario.API.IntegrationEvents
{
    public class UsuarioIntegrationEventService : IUsuarioIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly UsuarioContext _usuarioContext;
        private readonly IntegrationEventLogContext _logContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public UsuarioIntegrationEventService(UsuarioContext usuarioContext, IntegrationEventLogContext logContext,IEventBus eventBus, IOptionsSnapshot<UsuarioSettings> settings,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            

            _usuarioContext = usuarioContext ?? throw new ArgumentNullException(nameof(usuarioContext));
            _logContext = logContext ?? throw new ArgumentNullException(nameof(logContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_usuarioContext.Database.GetDbConnection()); 
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

        public async Task SaveEventAndUsuarioChangesAsync(IntegrationEvent evt)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _usuarioContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _usuarioContext.SaveChangesAsync();            
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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
        public async Task SaveEventAndUsuarioContextChangesAsync(IntegrationEvent evt, UsuarioItem usuarioToSave)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _usuarioContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _usuarioContext.SaveChangesAsync ();
                        //Tratamento de Identity
                        ((Events.UsuarioInclusaoIE)evt).UsuarioId = usuarioToSave.id_Usuario;
                        //Tratamento relacionamento
                        ((Events.UsuarioInclusaoIE)evt).UsuarioEmpresaPerfilItems = usuarioToSave.UsuarioEmpresaPerfilItems ;
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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
        
        public async Task SaveEventAndPerfilContextChangesAsync(IntegrationEvent evt,PerfilItem  perfilToSave)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _usuarioContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _usuarioContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.PerfilSaveIE)evt).PerfilId = perfilToSave.id_Perfil;
                        //Tratamento relacionamento
                        ((Events.PerfilSaveIE)evt).EmpresaPerfilItems = perfilToSave.EmpresaPerfilItems;
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndUsuarioEmpresaPerfilContextChangesAsync(IntegrationEvent evt)
        {
                var strategy = _usuarioContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _usuarioContext.Database.BeginTransaction())
                    {
                        try
                        {
                            await _usuarioContext.SaveChangesAsync();
                            await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndEmpresaPerfilContextChangesAsync(IntegrationEvent evt)
        {
                var strategy = _usuarioContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _usuarioContext.Database.BeginTransaction())
                    {
                        try
                        {
                            await _usuarioContext.SaveChangesAsync();
                            await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndEmpresaPerfilContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _usuarioContext.Database.BeginTransaction())

                {
                    try
                    {
                        EmpresaPerfilItem empresaPerfilItem = new EmpresaPerfilItem();
                        empresaPerfilItem.id_Empresa = ((Events.EmpresaPerfilExclusaoIE)evt).EmpresaId;
                        empresaPerfilItem.id_Perfil = ((Events.EmpresaPerfilExclusaoIE)evt).PerfilId;
                        await _usuarioContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndUsuarioEmpresaPerfilContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _usuarioContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _usuarioContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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

        
        public async Task DeleteEventAndUsuarioContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _usuarioContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _usuarioContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _usuarioContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _usuarioContext.Database.CurrentTransaction.GetDbTransaction());
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
