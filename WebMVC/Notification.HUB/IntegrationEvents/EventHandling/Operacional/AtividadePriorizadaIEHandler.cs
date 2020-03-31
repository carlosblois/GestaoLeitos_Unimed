
using Operacional.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class AtividadePriorizadaIEHandler :
        IIntegrationEventHandler<AtividadePriorizadaIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public AtividadePriorizadaIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(AtividadePriorizadaIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "AtividadePriorizadaIE", @event.Id, jsonMessage);

        }
    }
}