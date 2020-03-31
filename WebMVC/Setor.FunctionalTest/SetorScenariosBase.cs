using IntegrationEventLogEF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Setor.API;
using Setor.API.Infrastructure;
using System;
using System.IO;
using System.Reflection;

namespace Setor.FunctionalTests
{
    public class SetorScenariosBase :IDisposable 
    {
        private TestServer ts;
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(SetorScenariosBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<Startup>();

            var testServer = new TestServer(hostBuilder);

            testServer.Host
                .MigrateDbContext<SetorContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<SetorSettings>>();
                    var logger = services.GetService<ILogger<SetorContextSeed>>();

                    new SetorContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });
            ts = testServer;
            return testServer;
        }

        public void Dispose()
        {

            ts.Host
                .MigrateDbContext<SetorContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<SetorSettings>>();
                    var logger = services.GetService<ILogger<SetorContextSeed>>();

                    new SetorContextSeed()
                    .CleanAsync(context, env, settings, logger)
                    .Wait();
                });
        }

        public static class Get
        {
            public static string ConsultarPorId(Guid id)
            {
                return $"api/Setor/items/{id}";
            }

            public static string ConsultarPorIdEmpresa(Guid id)
            {
                return $"api/Setor/items/empresa/{id}";
            }            

        }

        public static class Del
        {
            public static string ExcluirSetor(Guid id)
            {
                return $"api/Setor/items/{id}";
            }

        }

        public static class Post
        {
            public static string IncluirSetor()
            {
                return $"api/Setor/items/";
            }

            public static string IncluirEmpresa()
            {
                return $"api/Empresa/items/";
            }

        }
    }
}
