
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class SLASituacaoExclusaoIEHandler :
        IIntegrationEventHandler<SLASituacaoExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SLASituacaoExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(SLASituacaoExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "SLASituacaoExclusaoIE", @event.Id, jsonMessage);

        }
    }
}