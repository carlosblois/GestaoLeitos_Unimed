using EventBus.Events;
using EventBus.Abstractions;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Administrativo.API.Infrastructure;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using Administrativo.API.Model;

namespace Administrativo.API.IntegrationEvents
{
    public class AdministrativoIntegrationEventService : IAdministrativoIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly AdministrativoContext _administrativoContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public AdministrativoIntegrationEventService(IEventBus eventBus, AdministrativoContext tipoAcomodacaoContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _administrativoContext = tipoAcomodacaoContext ?? throw new ArgumentNullException(nameof(tipoAcomodacaoContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_administrativoContext.Database.GetDbConnection());
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

        public async Task SaveEventAndTipoAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndSetorContextChangesAsync(IntegrationEvent evt, SetorItem setorToSave)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.SetorSaveIE)evt).SetorId = setorToSave.id_Setor;
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndEmpresaContextChangesAsync(IntegrationEvent evt, EmpresaItem empresaToSave)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.EmpresaSaveIE)evt).EmpresaId = empresaToSave.id_Empresa;
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndCaracteristicaAcomodacaoContextChangesAsync(IntegrationEvent evt, CaracteristicaAcomodacaoItem caracteristicaAcomodacaoToSave)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.CaracteristicaAcomodacaoSaveIE)evt).CaracteristicaAcomodacaoId = caracteristicaAcomodacaoToSave.id_CaracteristicaAcomodacao;
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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
        
            public async Task SaveEventAndAcessoEmpresaPerfilTSTAContextChangesAsync(IntegrationEvent evt, AcessoEmpresaPerfilTSTAItem AcessoEmpresaPerfilTSTAToSave)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.AcessoEmpresaPerfilTSTASaveIE)evt).AcessoEmpresaPerfilTipoSituacaoTipoAtividadeId  = AcessoEmpresaPerfilTSTAToSave.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade;
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task SaveEventAndAcomodacaoContextChangesAsync(IntegrationEvent evt, AcomodacaoItem acomodacaoToSave)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _administrativoContext.Database.BeginTransaction())
                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        //Tratamento de Identity
                        ((Events.AcomodacaoSaveIE)evt).AcomodacaoId = acomodacaoToSave.Id_Acomodacao;
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndTipoAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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
        
        public async Task DeleteEventAndAcessoEmpresaPerfilTSTContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndCaracteristicaAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndSetorContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndEmpresaContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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

        public async Task DeleteEventAndAcomodacaoContextChangesAsync(IntegrationEvent evt)
        {
            var strategy = _administrativoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {

                using (var transaction = _administrativoContext.Database.BeginTransaction())

                {
                    try
                    {
                        await _administrativoContext.SaveChangesAsync();
                        await _eventLogService.SaveEventAsync(evt, _administrativoContext.Database.CurrentTransaction.GetDbTransaction());
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
