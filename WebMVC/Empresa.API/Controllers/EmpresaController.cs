using System;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Empresa.API.Infrastructure;
using Empresa.API.IntegrationEvents;
using Empresa.API.IntegrationEvents.Events;
using Empresa.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Empresa.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private const string cachePrefix = "EMPRESA#";
        private readonly EmpresaContext _empresaContext;
        private readonly EmpresaSettings _settings;
        private readonly IEmpresaIntegrationEventService _empresaIntegrationEventService;



        public EmpresaController(EmpresaContext context, IOptionsSnapshot<EmpresaSettings> settings, IEmpresaIntegrationEventService empresaIntegrationEventService)
        {
            _empresaContext = context ?? throw new ArgumentNullException(nameof(context));
            _empresaIntegrationEventService = empresaIntegrationEventService ?? throw new ArgumentNullException(nameof(empresaIntegrationEventService));
            _settings = settings.Value;

            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //[HttpGet]
        //[Route("items/{id:int}")]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType(typeof(EmpresaItem), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> ConsultarPorId(Guid id)
        //{
        //    EmpresaItem item;
        //    if (_settings.UseCache)
        //    {
        //        Cache<EmpresaItem> mycache = new Cache<EmpresaItem>();
        //        item = await mycache.GetAsync(cachePrefix + id.ToString());
        //        if (item == null)
        //        {
        //            item = await _empresaContext.EmpresaItems.SingleOrDefaultAsync(ci => ci.Id_Empresa == id);
        //            await mycache.SetAsync(cachePrefix + id.ToString(), item);
        //        }
        //    }
        //    else
        //    {
        //        item = await _empresaContext.EmpresaItems.SingleOrDefaultAsync(ci => ci.Id_Empresa == id);
        //    }

        //    if (item != null)
        //    {
        //        return Ok(item);
        //    }

        //    return NotFound();
        //}

        ////POST api/v1/[controller]/items
        //[Route("items")]
        //[HttpPost]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType((int)HttpStatusCode.Created)]
        //public async Task<IActionResult> IncluirEmpresa([FromBody]EmpresaItem empresaToSave)
        //{
        //    Guid gKey;
        //    gKey = Guid.NewGuid();
            
        //    empresaToSave.Id_Empresa = gKey;
        //    // Update current empresa
        //    _empresaContext.EmpresaItems.Add(empresaToSave);


        //    if (_settings.UseCache)
        //    {
        //        Cache<EmpresaItem> mycache = new Cache<EmpresaItem>();
        //        await mycache.SetAsync(cachePrefix + empresaToSave.Id_Empresa, empresaToSave);
        //    }

        //    //Create Integration Event to be published through the Event Bus
        //    var empresaInclusaoEvent = new EmpresaInclusaoIntegrationEvent(empresaToSave.Id_Empresa , empresaToSave.Nome_Empresa);

        //    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
        //    await _empresaIntegrationEventService.SaveEventAndCatalogContextChangesAsync(empresaInclusaoEvent);

        //    // Publish through the Event Bus and mark the saved event as published
        //    await _empresaIntegrationEventService.PublishThroughEventBusAsync(empresaInclusaoEvent);
            

        //    return CreatedAtAction(nameof(IncluirEmpresa), new { id = empresaToSave.Id_Empresa }, null);
        //}
    }
}