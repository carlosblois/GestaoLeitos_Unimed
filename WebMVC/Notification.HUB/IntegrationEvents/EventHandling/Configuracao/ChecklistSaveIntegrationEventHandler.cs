
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class ChecklistSaveIEHandler :
        IIntegrationEventHandler<ChecklistSaveIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChecklistSaveIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(ChecklistSaveIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "ChecklistSaveIE", @event.Id, jsonMessage);

        }
    }
}