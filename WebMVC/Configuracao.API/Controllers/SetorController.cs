using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Configuracao.API.Infrastructure;
using Configuracao.API.IntegrationEvents;
using Configuracao.API.IntegrationEvents.Events;
using Configuracao.API.Model;

namespace Configuracao.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SetorController : ControllerBase
    {

        private const string cachePrefix = "SETOR#";
        private readonly ConfiguracaoContext _setorContext;
        private readonly ConfiguracaoSettings _settings;
        private readonly IConfiguracaoIntegrationEventService _setorIntegrationEventService;



        public SetorController(ConfiguracaoContext context, IOptionsSnapshot<ConfiguracaoSettings> settings, IConfiguracaoIntegrationEventService setorIntegrationEventService)
        {
            _setorContext = context ?? throw new ArgumentNullException(nameof(context));
            _setorIntegrationEventService = setorIntegrationEventService ?? throw new ArgumentNullException(nameof(setorIntegrationEventService));
            _settings = settings.Value;

            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        ////PUT api/v1/[controller]/items
        //[Route("items")]
        //[HttpPut]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType((int)HttpStatusCode.Created)]
        //public async Task<IActionResult> AtualizarSetor([FromBody]SetorItem setorToSave)
        //{

        //    // Update current setor
        //    _setorContext.SetorItems.Update(setorToSave);

        //    if (_settings.UseCache)
        //    {
        //        Cache<SetorItem> mycache = new Cache<SetorItem>();
        //        await mycache.SetAsync(cachePrefix + setorToSave.id_Setor, setorToSave);
        //        await mycache.UpdateListAsync(cachePrefix + setorToSave.id_Empresa );
        //    }

        //    //Create Integration Event to be published through the Event Bus
        //    var setorAtualizarEvent = new SetorAtualizarIntegrationEvent(setorToSave.id_Empresa, setorToSave.id_Setor, setorToSave.nome_Setor);

        //    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
        //    await _setorIntegrationEventService.SaveEventAndSetorContextChangesAsync(setorAtualizarEvent);

        //    // Publish through the Event Bus and mark the saved event as published
        //    await _setorIntegrationEventService.PublishThroughEventBusAsync(setorAtualizarEvent);


        //    return CreatedAtAction(nameof(AtualizarSetor), null);
        //}

        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirSetor([FromBody]SetorItem setorToSave)
        {
            Guid gKey;
            gKey = Guid.NewGuid();

            setorToSave.id_Setor = gKey;
            // Update current setor
            _setorContext.SetorItems.Add(setorToSave);


            //if (_settings.UseCache)
            //{
            //    Cache<SetorItem> mycache = new Cache<SetorItem>();
            //    await mycache.SetAsync(cachePrefix + setorToSave.id_Empresa+ setorToSave.id_Setor, setorToSave);
            //}

            //Create Integration Event to be published through the Event Bus
            var setorInclusaoEvent = new SetorInclusaoIntegrationEvent(setorToSave.id_Empresa, setorToSave.id_Setor ,setorToSave.nome_Setor );

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _setorIntegrationEventService.SaveEventAndSetorContextChangesAsync(setorInclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _setorIntegrationEventService.PublishThroughEventBusAsync(setorInclusaoEvent);


            return CreatedAtAction(nameof(IncluirSetor), null);
        }

        ////POST api/v1/[controller]/items
        //[Route("items/{id}")]
        //[HttpDelete]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType((int)HttpStatusCode.Created)]
        //public async Task<IActionResult> ExcluirSetor(Guid? id)
        //{

        //    SetorItem setorToDelete;
        //    // Delete setor
        //    setorToDelete = await _setorContext.SetorItems.SingleAsync(ci => ci.id_Setor == id);
        //    _setorContext.SetorItems.Remove(setorToDelete);


        //    if (_settings.UseCache)
        //    {
        //        Cache<SetorItem> mycache = new Cache<SetorItem>();
        //        await mycache.DelAsync(cachePrefix + setorToDelete.id_Setor, setorToDelete);
        //        await mycache.DelListAsync(cachePrefix + setorToDelete.id_Empresa);
        //    }

        //    //Create Integration Event to be published through the Event Bus
        //    var setorExclusaoEvent = new SetorExclusaoIntegrationEvent(setorToDelete.id_Empresa, setorToDelete.id_Setor, setorToDelete.nome_Setor);

        //    // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
        //    await _setorIntegrationEventService.SaveEventAndSetorContextChangesAsync(setorExclusaoEvent);

        //    // Publish through the Event Bus and mark the saved event as published
        //    await _setorIntegrationEventService.PublishThroughEventBusAsync(setorExclusaoEvent);


        //    return CreatedAtAction(nameof(ExcluirSetor), null);
        //}

        //[HttpGet]
        //[Route("items/{id}")]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType(typeof(SetorItem), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> ConsultarPorId(Guid? id)
        //{
        //    SetorItem item;
        //    if (_settings.UseCache)
        //    {
        //        Cache<SetorItem> mycache = new Cache<SetorItem>();
        //        item = await mycache.GetAsync(cachePrefix + id.ToString());
        //        if (item == null)
        //        {
        //            item = await _setorContext.SetorItems.SingleOrDefaultAsync(ci => ci.id_Setor == id);
        //            if (item != null)
        //            {
        //                await mycache.SetAsync(cachePrefix + id.ToString(), item);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        item = await _setorContext.SetorItems.SingleOrDefaultAsync(ci => ci.id_Setor == id);
        //    }

        //    if (item != null)
        //    {
        //        return Ok(item);
        //    }

        //    return NotFound();
        //}

        //[HttpGet]
        //[Route("items/empresa/{idEmpresa}")]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //[ProducesResponseType(typeof(List<SetorItem>), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> ConsultarPorIdEmpresa(Guid? idEmpresa)
        //{
        //    List<SetorItem> lst;
        //    if (_settings.UseCache)
        //    {
        //        Cache<SetorItem> mycache = new Cache<SetorItem>();
        //        lst = await mycache.GetListAsync(cachePrefix + idEmpresa.ToString());
        //        if (lst.Count == 0)
        //        {
        //            String sql = String.Format("SELECT * FROM SETOR WHERE ID_EMPRESA ='{0}'", idEmpresa);
        //            lst = _setorContext.SetorItems.FromSql(sql).ToList();
        //            if (lst.Count !=0)
        //            {
        //                await mycache.SetListAsync(cachePrefix + idEmpresa.ToString(), lst);
        //            }
                    
        //        }

        //    }
        //    else
        //    {
        //        String sql = String.Format("SELECT * FROM SETOR WHERE ID_EMPRESA ='{0}'", idEmpresa);
        //        lst = _setorContext.SetorItems.FromSql(sql).ToList();
        //    }

        //    if (lst.Count != 0)
        //    {
        //        return Ok(lst);
        //    }

        //    return NotFound();
        //}
    }


}