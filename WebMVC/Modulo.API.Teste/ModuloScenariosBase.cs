using IntegrationEventLogEF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modulo.API.Infrastructure;
using Modulo.API;
using System;
using System.IO;
using System.Reflection;

namespace Modulo.FunctionalTests
{
    public class ModuloScenariosBase : IDisposable
    {
        private TestServer ts;
        //public TestServer CreateServer()
        //{
        //    var path = Assembly.GetAssembly(typeof(ModuloScenariosBase))
        //      .Location;

        //    //var hostBuilder = new WebHostBuilder()
        //    //    .UseContentRoot(Path.GetDirectoryName(path))
        //    //    .ConfigureAppConfiguration(cb =>
        //    //    {
        //    //        cb.AddJsonFile("appsettings.json", optional: false)
        //    //        .AddEnvironmentVariables();
        //    //    }).UseStartup<Startup>();

        //    //var testServer = new TestServer(hostBuilder);
        //    //var testServer = new TestServer();

        //    //testServer.Host
        //    //    .MigrateDbContext<ModuloContext>((context, services) =>
        //    //    {
        //    //        var env = services.GetService<IHostingEnvironment>();
        //    //        var settings = services.GetService<IOptions<ModuloSettings>>();
        //    //    })
        //    //    .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });
        //    ts = testServer;
        //    return testServer;
        //}

        public void Dispose()
        {
        }
        public static class Post
        {
            public static string SaveModulo()
            {
                //return "http://localhost:5000/api/Modulo/items";
                return "https://moduloapi.azurewebsites.net/api/Modulo/items";
            }


        }
    }
}
