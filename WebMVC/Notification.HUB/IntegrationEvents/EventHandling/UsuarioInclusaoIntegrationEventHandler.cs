using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Notification.HUB.IntegrationEvents.Events;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{


    public class UsuarioInclusaoIntegrationEventHandler :
        IIntegrationEventHandler<UsuarioInclusaoIntegrationEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public UsuarioInclusaoIntegrationEventHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(UsuarioInclusaoIntegrationEvent @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            //    await _hubContext.Clients
            //        .Group(@event.UsuarioId.ToString())
            //        .SendAsync("UsuarioSalvar", new { UsuarioId = @event.UsuarioId, UsuarioNome = @event.UsuarioNome  });
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", @event.Id, jsonMessage);

        }
    }
}