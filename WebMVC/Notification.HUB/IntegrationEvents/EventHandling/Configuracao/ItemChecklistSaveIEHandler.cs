
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class ItemChecklistSaveIEHandler :
        IIntegrationEventHandler<ItemChecklistSaveIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public ItemChecklistSaveIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(ItemChecklistSaveIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "ItemChecklistSaveIE", @event.Id, jsonMessage);

        }
    }
}