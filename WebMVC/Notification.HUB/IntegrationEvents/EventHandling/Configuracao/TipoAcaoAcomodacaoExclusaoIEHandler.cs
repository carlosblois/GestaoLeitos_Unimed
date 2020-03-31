
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class TipoAcaoAcomodacaoExclusaoIEHandler :
        IIntegrationEventHandler<TipoAcaoAcomodacaoExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public TipoAcaoAcomodacaoExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(TipoAcaoAcomodacaoExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "TipoAcaoAcomodacaoExclusaoIE", @event.Id, jsonMessage);

        }
    }
}