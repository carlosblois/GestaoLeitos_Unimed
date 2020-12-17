using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQUtil;
using EventBusServiceBusUtil;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Options;
using Notification.HUB.IntegrationEvents.EventHandling;
using RabbitMQ.Client;
using Administrativo.API.IntegrationEvents.Events;
using Configuracao.API.IntegrationEvents.Events;
using Modulo.API.IntegrationEvents.Events;
using Usuario.API.IntegrationEvents.Events;
using Operacional.API.IntegrationEvents.Events;

namespace Notification.HUB
{
        // This method gets called by the runtime. Use this method to add services to the container.

        public class Startup
        {

            public IConfiguration Configuration { get; }
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                //options.MinimumSameSitePolicy = SameSiteMode.None;
            });          

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSignalR();

            //ConfigureAuthService(services);
            services.AddCustomIntegrations(Configuration);
            services.AddEventBus(Configuration);
            services.AddHealthChecks(Configuration);

            services.AddOptions();

            //configure autofac
            var container = new ContainerBuilder();
            container.RegisterModule(new ApplicationModule());
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Error);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseExceptionHandler("/Error");
            app.UseHsts();
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/notificationhub");
            });
            app.UseMvc();

            ConfigureEventBus(app);
        }


        

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            //MODULO
            eventBus.Subscribe<EmpresaPerfilModuloExclusaoGrupoIE, EmpresaPerfilModuloExclusaoGrupoIEHandler>();
            eventBus.Subscribe<EmpresaPerfilModuloExclusaoIE, EmpresaPerfilModuloExclusaoIEHandler>();
            eventBus.Subscribe<EmpresaPerfilModuloInclusaoGrupoIE, EmpresaPerfilModuloInclusaoGrupoIEHandler>();
            eventBus.Subscribe<EmpresaPerfilModuloInclusaoIE, EmpresaPerfilModuloInclusaoIEHandler>();
            eventBus.Subscribe<ModuloExclusaoGrupoIE, ModuloExclusaoGrupoIEHandler>();
            eventBus.Subscribe<ModuloExclusaoIE, ModuloExclusaoIEHandler>();
            eventBus.Subscribe<ModuloInclusaoGrupoIE, ModuloInclusaoGrupoIEHandler>();
            eventBus.Subscribe<ModuloInclusaoIE, ModuloInclusaoIEHandler>();
            eventBus.Subscribe<OperacaoExclusaoGrupoIE, OperacaoExclusaoGrupoIEHandler>();
            eventBus.Subscribe<OperacaoExclusaoIE, OperacaoExclusaoIEHandler>();
            eventBus.Subscribe<OperacaoInclusaoGrupoIE, OperacaoInclusaoGrupoIEHandler>();
            eventBus.Subscribe<OperacaoInclusaoIE, OperacaoInclusaoIEHandler>();


            //ADMINISTRATIVO
            eventBus.Subscribe<AcessoEmpresaPerfilTSTAExclusaoIE, AcessoEmpresaPerfilTSTAExclusaoIEHandler>();
            eventBus.Subscribe<AcessoEmpresaPerfilTSTASaveIE, AcessoEmpresaPerfilTSTASaveIEHandler>();
            eventBus.Subscribe<AcomodacaoExclusaoIE, AcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<AcomodacaoSaveIE, AcomodacaoSaveIEHandler>();
            eventBus.Subscribe<CaracteristicaAcomodacaoExclusaoIE, CaracteristicaAcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<CaracteristicaAcomodacaoSaveIE, CaracteristicaAcomodacaoSaveIEHandler>();
            eventBus.Subscribe<EmpresaExclusaoIE, EmpresaExclusaoIEHandler>();
            eventBus.Subscribe<EmpresaSaveIE, EmpresaSaveIEHandler>();
            eventBus.Subscribe<SetorExclusaoIE, SetorExclusaoIEHandler>();
            eventBus.Subscribe<SetorSaveIE, SetorSaveIEHandler>();
            eventBus.Subscribe<TipoAcomodacaoExclusaoIE, TipoAcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<TipoAcomodacaoSaveIE, TipoAcomodacaoSaveIEHandler>();
            //CONFIGURACAO
            eventBus.Subscribe<ChecklistExclusaoIE, ChecklistExclusaoIEHandler>();
            eventBus.Subscribe<ChecklistItemChecklistExclusaoIE, ChecklistItemChecklistExclusaoIEHandler>();
            eventBus.Subscribe<ChecklistItemChecklistIncluirIE, ChecklistItemChecklistIncluirIEHandler>();
            eventBus.Subscribe<ChecklistSaveIE, ChecklistSaveIEHandler>();
            eventBus.Subscribe<ChecklistTipoSituacaoTATAExcluirIE, ChecklistTipoSituacaoTATAExclusaoIEHandler>();
            eventBus.Subscribe<ChecklistTipoSituacaoTATAIncluirIE, ChecklistTipoSituacaoTATAIncluirIEHandler>();
            eventBus.Subscribe<FluxoAutomaticoAcaoExcluirIE, FluxoAutomaticoAcaoExcluirIEHandler>();
            eventBus.Subscribe<FluxoAutomaticoAcaoIncluirIE, FluxoAutomaticoAcaoIncluirIEHandler>();
            eventBus.Subscribe<ItemChecklistExclusaoIE, ItemChecklistExclusaoIEHandler>();
            eventBus.Subscribe<ItemChecklistSaveIE, ItemChecklistSaveIEHandler>();
            eventBus.Subscribe<SLAExclusaoIE, SLAExclusaoIEHandler>();
            eventBus.Subscribe<SLASaveIE, SLASaveIEHandler>();
            eventBus.Subscribe<SLASituacaoExclusaoIE, SLASituacaoExclusaoIEHandler>();
            eventBus.Subscribe<SLASituacaoSaveIE, SLASituacaoSaveIEHandler>();
            eventBus.Subscribe<TipoAcaoAcomodacaoExclusaoIE, TipoAcaoAcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<TipoAcaoAcomodacaoSaveIE, TipoAcaoAcomodacaoSaveIEHandler>();
            eventBus.Subscribe<TipoAtividadeAcomodacaoExclusaoIE, TipoAtividadeAcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<TipoAtividadeAcomodacaoSaveIE, TipoAtividadeAcomodacaoSaveIEHandler>();
            eventBus.Subscribe<TipoSituacaoAcomodacaoExclusaoIE, TipoSituacaoAcomodacaoExclusaoIEHandler>();
            eventBus.Subscribe<TipoSituacaoAcomodacaoSaveIE, TipoSituacaoAcomodacaoSaveIEHandler>();
            eventBus.Subscribe<TipoSituacaoTAAExclusaoIE, TipoSituacaoTAAExclusaoIEHandler>();
            eventBus.Subscribe<TipoSituacaoTAAIncluirIE, TipoSituacaoTAAIncluirIEHandler>();
            //USUARIO
            eventBus.Subscribe<EmpresaPerfilExclusaoIE, EmpresaPerfilExclusaoIEHandler>();
            eventBus.Subscribe<EmpresaPerfilInclusaoGrupoIE, EmpresaPerfilInclusaoGrupoIEHandler>();
            eventBus.Subscribe<EmpresaPerfilInclusaoIE, EmpresaPerfilInclusaoIEHandler>();
            eventBus.Subscribe<PerfilSaveGrupoIE, PerfilSaveGrupoIEHandler>();
            eventBus.Subscribe<PerfilSaveIE, PerfilSaveIEHandler>();
            eventBus.Subscribe<UsuarioAtualizaLoginIE, UsuarioAtualizaLoginIEHandler>();
            eventBus.Subscribe<UsuarioAtualizaSenhaIE, UsuarioAtualizaSenhaIEHandler>();
            eventBus.Subscribe<UsuarioEmpresaPerfilExclusaoGrupoIE, UsuarioEmpresaPerfilExclusaoGrupoIEHandler>();
            eventBus.Subscribe<UsuarioEmpresaPerfilExclusaoIE, UsuarioEmpresaPerfilExclusaoIEHandler>();
            eventBus.Subscribe<UsuarioEmpresaPerfilExclusaoTodosIE, UsuarioEmpresaPerfilExclusaoTodosIEHandler>();
            eventBus.Subscribe<UsuarioEmpresaPerfilInclusaoGrupoIE, UsuarioEmpresaPerfilInclusaoGrupoIEHandler>();
            eventBus.Subscribe<UsuarioEmpresaPerfilInclusaoIE, UsuarioEmpresaPerfilInclusaoIEHandler>();
            eventBus.Subscribe<UsuarioExclusaoGrupoIE, UsuarioExclusaoGrupoIEHandler>();
            eventBus.Subscribe<UsuarioExclusaoIE, UsuarioExclusaoIEHandler>();
            eventBus.Subscribe<UsuarioInclusaoGrupoIE, UsuarioInclusaoGrupoIEHandler>();
            eventBus.Subscribe<UsuarioInclusaoIE, UsuarioInclusaoIEHandler>();

            //OPERACIONAL
            eventBus.Subscribe<GeraAcaoAcomodacaoIE, GeraAcaoAcomodacaoIEHandler>();
            eventBus.Subscribe<MensagemSaveIE, MensagemSaveIEHandler>();
            eventBus.Subscribe<MensagemRetornoIE, MensagemRetornoIEHandler>();
            eventBus.Subscribe<GeraAcaoAcomodacaoSolicitarIE, GeraAcaoAcomodacaoSolicitarIEHandler>();
            eventBus.Subscribe<FinalizaAcaoAcomodacaoIE, FinalizaAcaoAcomodacaoIEHandler>();
            eventBus.Subscribe<AtividadeSaveIE, AtividadeSaveIEHandler>();
            eventBus.Subscribe<AtividadePriorizadaIE, AtividadePriorizadaIEHandler>();
            eventBus.Subscribe<FinalizaAtividadeAcomodacaoIE, FinalizaAtividadeAcomodacaoIEHandler>();
            eventBus.Subscribe<SituacaoSaveIE, SituacaoSaveIEHandler>();
        }


        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });
            }
            else
            {
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                    }

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            return services;
        }
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });
            }
            else
            {
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));


            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = configuration["EventBusConnection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBusConnection"]
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                    {
                        factory.UserName = configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                    {
                        factory.Password = configuration["EventBusPassword"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusPort"]))
                    {
                        factory.Port = int.Parse(configuration["EventBusPort"]);
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusVirtualHost"]))
                    {
                        factory.VirtualHost = configuration["EventBusVirtualHost"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            }

            return services;
        }
    }

}
