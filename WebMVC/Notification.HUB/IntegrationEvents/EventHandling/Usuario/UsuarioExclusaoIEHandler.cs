using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Usuario.API.IntegrationEvents.Events;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class UsuarioExclusaoIEHandler :
        IIntegrationEventHandler<UsuarioExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public UsuarioExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(UsuarioExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "UsuarioExclusaoIE", @event.Id, jsonMessage);

        }
    }
}