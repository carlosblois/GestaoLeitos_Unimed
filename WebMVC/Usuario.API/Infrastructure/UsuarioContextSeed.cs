namespace Usuario.API.Infrastructure
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Model;
    using Polly;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class UsuarioContextSeed
    {
        public async Task SeedAsync(UsuarioContext context, IHostingEnvironment env, IOptions<UsuarioSettings> settings, ILogger<UsuarioContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(UsuarioContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value.UseCustomizationData;
                var contentRootPath = env.ContentRootPath;
 
                await context.SetorItems.AddRangeAsync(useCustomizationData
                    ? GetSetorItemsFromFile(contentRootPath, context, logger)
                    : GetPreconfiguredItems());

                await context.SaveChangesAsync();
                
            });
        }

        public async Task CleanAsync(UsuarioContext context, IHostingEnvironment env, IOptions<UsuarioSettings> settings, ILogger<UsuarioContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(UsuarioContextSeed));
            try {
                await policy.ExecuteAsync(async () =>
                {
                    var useCustomizationData = settings.Value.UseCustomizationData;
                    var contentRootPath = env.ContentRootPath;

                    context.SetorItems.FromSql("SELECT * FROM SETOR");

                    IEnumerable <UsuarioItem> st;
                    st = useCustomizationData ? GetSetorItemsFromFile(contentRootPath, context, logger) : GetPreconfiguredItems();
                    foreach (UsuarioItem it in st)
                    {
                        var item = await context.SetorItems.SingleOrDefaultAsync(ci => ci.id_Setor == it.id_Setor );
                        if (item != null)
                        {
                            context.SetorItems.Remove(item);
                        }
                    }

                    await context.SaveChangesAsync();

                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
  
            }
        }

        private IEnumerable<UsuarioItem> GetSetorItemsFromFile(string contentRootPath, UsuarioContext context, ILogger<UsuarioContextSeed> logger)
        {
            string csvFileSetorItems = Path.Combine(contentRootPath, "Setup", "SetorItems.csv");

            if (!File.Exists(csvFileSetorItems))
            {
                return GetPreconfiguredItems();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "id_empresa", "id_setor", "nome_setor" };
                string[] optionalheaders = {  };
                csvheaders = GetHeaders(csvFileSetorItems, requiredHeaders, optionalheaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return GetPreconfiguredItems();
            }

            return File.ReadAllLines(csvFileSetorItems)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .Select(column => CreateSetorItem(column, csvheaders))
                        .Where(x => x != null);

        }

        private UsuarioItem CreateSetorItem(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            var setorItem = new UsuarioItem()
            {
                id_Empresa = Guid.Parse(column[Array.IndexOf(headers, "id_empresa")].Trim('"').Trim()),
                id_Setor = Guid.Parse(column[Array.IndexOf(headers, "id_setor")].Trim('"').Trim()),
                nome_Setor  = column[Array.IndexOf(headers, "nome_setor")].Trim('"').Trim()
            };

            return setorItem;
        }

        private IEnumerable<UsuarioItem> GetPreconfiguredItems()
        {
            return new List<UsuarioItem>()
            {
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("26856FF1-FF54-466F-ACD5-B830DA68D493"), nome_Setor = "SETOR A" },
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("2F676B7B-516B-479A-8590-09EE4569672A"), nome_Setor = "SETOR B" },
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("27A897E8-4038-4C83-8211-9B0F3CF0DEB1"), nome_Setor = "SETOR C" },
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("D798C207-08E9-4E7B-8766-CEB2252321EC"), nome_Setor = "SETOR D" },
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("4C8D935A-E673-47D3-A8F5-9A3934DF72E7"), nome_Setor = "SETOR E" },
                new UsuarioItem { id_Empresa = Guid.Parse("714579E1-AAB3-4E85-95C3-06FA49488730"), id_Setor = Guid.Parse("781BC344-5242-4A9E-9662-367691603503"), nome_Setor = "SETOR F" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("B411DC8F-6F7C-4651-AE17-333CBF97B234"), nome_Setor = "SETOR G" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("FF7BA2A0-D757-4C96-BCBD-DE4B21130AB4"), nome_Setor = "SETOR H" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("80536ACC-AD8F-4B8D-9909-5EFED895525F"), nome_Setor = "SETOR I" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("3722ADD3-C4A7-400C-9CD2-C55ECE2C74E1"), nome_Setor = "SETOR J" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("2221EE5E-D4F4-44E9-9C39-1AE918EEAE54"), nome_Setor = "SETOR L" },
                new UsuarioItem { id_Empresa = Guid.Parse("23840615-670C-418A-AA4C-9B234BC3C83A"), id_Setor = Guid.Parse("A898A236-C34F-46F8-93AA-636EC4E90D20"), nome_Setor = "SETOR M" }
            };
        }

        private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders != null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        private Policy CreatePolicy(ILogger<UsuarioContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
