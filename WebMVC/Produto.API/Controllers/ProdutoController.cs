using System;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Produto.API.Infrastructure;
using Produto.API.IntegrationEvents;
using Produto.API.IntegrationEvents.Events;
using Produto.API.Model;

namespace Produto.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private readonly ProdutoContext _produtoContext;
        private readonly ProdutoSettings _settings;
        private readonly IProdutoIntegrationEventService _produtoIntegrationEventService;



        public ProdutoController(ProdutoContext context, IOptionsSnapshot<ProdutoSettings> settings, IProdutoIntegrationEventService produtoIntegrationEventService)
        {
            _produtoContext = context ?? throw new ArgumentNullException(nameof(context));
            _produtoIntegrationEventService = produtoIntegrationEventService ?? throw new ArgumentNullException(nameof(produtoIntegrationEventService));
            _settings = settings.Value;

            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProdutoItem), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            ProdutoItem item;
            if (_settings.UseCache)
            {
                Cache<ProdutoItem> mycache = new Cache<ProdutoItem>();
                item = await mycache.GetAsync("PRODUTO"+id.ToString());
                if (item == null)
                {
                    item = await _produtoContext.ProdutoItems.SingleOrDefaultAsync(ci => ci.Id == id);
                    await mycache.SetAsync("PRODUTO"+id.ToString(), item);
                }
            }
            else
            {
                 item = await _produtoContext.ProdutoItems.SingleOrDefaultAsync(ci => ci.Id == id);
            }

            if (item != null)
            {
                return Ok(item);
            }

            return NotFound();
        }

        //PUT api/v1/[controller]/items
        [Route("items")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateProduct([FromBody]ProdutoItem productToUpdate)
        {
            var produtoItem = await _produtoContext.ProdutoItems
                .SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (produtoItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }

            var oldPrice = produtoItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;


            // Update current product
            produtoItem = productToUpdate;
            _produtoContext.ProdutoItems.Update(produtoItem);
            if (_settings.UseCache)
            {
                Cache<ProdutoItem> mycache = new Cache<ProdutoItem>();
                await mycache.SetAsync("PRODUTO"+productToUpdate.Id.ToString(), productToUpdate);
            }

            if (raiseProductPriceChangedEvent) // Save product's data and publish integration event through the Event Bus if price has changed
            {
                //Create Integration Event to be published through the Event Bus
                var priceChangedEvent = new ProdutoPriceChangedIntegrationEvent(produtoItem.Id, productToUpdate.Price, oldPrice);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _produtoIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                // Publish through the Event Bus and mark the saved event as published
                await _produtoIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            }
            else // Just save the updated product because the Product's Price hasn't changed.
            {
                await _produtoContext.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id }, null);
        }

    }

}