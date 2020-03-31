using Administrativo.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class AcomodacaoSaveIEHandler :
        IIntegrationEventHandler<AcomodacaoSaveIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public AcomodacaoSaveIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(AcomodacaoSaveIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "AcomodacaoSaveIE", @event.Id, jsonMessage);

        }
    }
}