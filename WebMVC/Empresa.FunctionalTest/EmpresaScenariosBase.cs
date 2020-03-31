using IntegrationEventLogEF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Empresa.API;
using Empresa.API.Infrastructure;
using System;
using System.IO;
using System.Reflection;

namespace Empresa.FunctionalTests
{
    public class EmpresaScenariosBase : IDisposable
    {
        private TestServer ts;
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(EmpresaScenariosBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<Startup>();

            var testServer = new TestServer(hostBuilder);

            ts = testServer;
            return testServer;
        }

        public void Dispose()
        {
        }


        public static class Post
        {

            public static string IncluirEmpresa()
            {
                return $"api/Empresa/items/";
            }

        }
    }
}
