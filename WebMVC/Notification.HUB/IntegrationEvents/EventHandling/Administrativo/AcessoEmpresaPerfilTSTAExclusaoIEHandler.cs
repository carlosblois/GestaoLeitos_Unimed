using Administrativo.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class AcessoEmpresaPerfilTSTAExclusaoIEHandler :
        IIntegrationEventHandler<AcessoEmpresaPerfilTSTAExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public AcessoEmpresaPerfilTSTAExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(AcessoEmpresaPerfilTSTAExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "AcessoEmpresaPerfilTSTAExclusaoIE", @event.Id, jsonMessage);

        }
    }
}