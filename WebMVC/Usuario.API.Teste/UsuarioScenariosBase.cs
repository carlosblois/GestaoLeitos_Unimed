using IntegrationEventLogEF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Usuario.API.Infrastructure;
using Usuario.API;
using System;
using System.IO;
using System.Reflection;

namespace Usuario.FunctionalTests
{
    public class UsuarioScenariosBase : IDisposable
    {
        private TestServer ts;
        //public TestServer CreateServer()
        //{
        //    var path = Assembly.GetAssembly(typeof(UsuarioScenariosBase))
        //      .Location;

        //    var hostBuilder = new WebHostBuilder()
        //        .UseContentRoot(Path.GetDirectoryName(path))
        //        .ConfigureAppConfiguration(cb =>
        //        {
        //            cb.AddJsonFile("appsettings.json", optional: false)
        //            .AddEnvironmentVariables();
        //        }).UseStartup<Startup>();

        //    var testServer = new TestServer(hostBuilder);

        //    //testServer.Host
        //    //    .MigrateDbContext<UsuarioContext>((context, services) =>
        //    //    {
        //    //        var env = services.GetService<IHostingEnvironment>();
        //    //        var settings = services.GetService<IOptions<UsuarioSettings>>();
 
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
            public static string SaveUsuario()
            {
                return "http://localhost:5002/api/Usuario/items";
            }


        }
    }
}
