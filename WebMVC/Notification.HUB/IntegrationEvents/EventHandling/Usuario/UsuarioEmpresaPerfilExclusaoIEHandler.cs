using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Usuario.API.IntegrationEvents.Events;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class UsuarioEmpresaPerfilExclusaoIEHandler :
        IIntegrationEventHandler<UsuarioEmpresaPerfilExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public UsuarioEmpresaPerfilExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(UsuarioEmpresaPerfilExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "UsuarioEmpresaPerfilExclusaoIE", @event.Id, jsonMessage);

        }
    }
}