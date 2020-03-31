
using Operacional.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class MensagemRetornoIEHandler :
        IIntegrationEventHandler<MensagemRetornoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public MensagemRetornoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(MensagemRetornoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "MensagemRetornoIE", @event.Id, jsonMessage);

        }
    }
}