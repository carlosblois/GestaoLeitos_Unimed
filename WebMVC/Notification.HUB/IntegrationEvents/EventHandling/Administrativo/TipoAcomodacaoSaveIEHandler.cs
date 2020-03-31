using Administrativo.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class TipoAcomodacaoSaveIEHandler :
        IIntegrationEventHandler<TipoAcomodacaoSaveIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public TipoAcomodacaoSaveIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(TipoAcomodacaoSaveIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "TipoAcomodacaoSaveIE", @event.Id, jsonMessage);

        }
    }
}