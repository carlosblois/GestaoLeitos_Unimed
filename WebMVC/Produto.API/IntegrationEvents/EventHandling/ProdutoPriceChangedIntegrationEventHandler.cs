using EventBus.Abstractions;
using Produto.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Produto.API.IntegrationEvents.EventHandling
{
    //public class ProdutoPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProdutoPriceChangedIntegrationEvent>
    //{
    //    //private readonly IProdutoRepository _repository;

    //    //public ProdutoPriceChangedIntegrationEventHandler(IProdutoRepository repository)
    //    //{
    //    //    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    //    //}

    //    //public async Task Handle(ProdutoPriceChangedIntegrationEvent @event)
    //    //{
    //    //        //var basket = await _repository.GetBasketAsync(id);

    //    //        //await UpdatePriceProdutoFornecedor(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
    //    //}

    //    //private async Task UpdatePriceProdutoFornecedor(int productId, decimal newPrice, decimal oldPrice, CustomerBasket basket)
    //    //{
    //    //    //string match = productId.ToString();
    //    //    //var itemsToUpdate = basket?.Items?.Where(x => x.ProductId == match).ToList();

    //    //    //if (itemsToUpdate != null)
    //    //    //{
    //    //    //    foreach (var item in itemsToUpdate)
    //    //    //    {
    //    //    //        if (item.UnitPrice == oldPrice)
    //    //    //        {
    //    //    //            var originalPrice = item.UnitPrice;
    //    //    //            item.UnitPrice = newPrice;
    //    //    //            item.OldUnitPrice = originalPrice;
    //    //    //        }
    //    //    //    }
    //    //    //    await _repository.UpdateBasketAsync(basket);
    //    //    //}
    //    //}
    //}
}
