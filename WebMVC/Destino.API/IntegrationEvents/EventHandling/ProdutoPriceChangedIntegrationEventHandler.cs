using Destino.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destino.API.Infrastructure
{

    namespace Destino.API.IntegrationEvents.EventHandling
    {
        public class ProdutoPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProdutoPriceChangedIntegrationEvent>
        {

            public async Task Handle(ProdutoPriceChangedIntegrationEvent @event)
            {                
                await UpdatePriceProdutoFornecedor(@event.ProductId, @event.NewPrice, @event.OldPrice);
            }

            private async Task UpdatePriceProdutoFornecedor(int productId, decimal newPrice, decimal oldPrice)
            {
                string match = productId.ToString();

            }
        }
    }

}
