
using Operacional.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class AtividadeSaveIEHandler :
        IIntegrationEventHandler<AtividadeSaveIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public AtividadeSaveIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(AtividadeSaveIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "AtividadeSaveIE", @event.Id, jsonMessage);

        }
    }
}