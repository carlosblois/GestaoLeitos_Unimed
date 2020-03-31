using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Modulo.API.IntegrationEvents.Events;
using Newtonsoft.Json;


using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{


    public class OperacaoInclusaoGrupoIEHandler :
        IIntegrationEventHandler<OperacaoInclusaoGrupoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public OperacaoInclusaoGrupoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(OperacaoInclusaoGrupoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
           
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "OperacaoInclusaoGrupoIE", @event.Id, jsonMessage);

        }
    }
}