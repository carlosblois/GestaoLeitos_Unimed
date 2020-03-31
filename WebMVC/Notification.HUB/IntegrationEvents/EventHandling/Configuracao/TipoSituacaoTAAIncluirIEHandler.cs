
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class TipoSituacaoTAAIncluirIEHandler :
        IIntegrationEventHandler<TipoSituacaoTAAIncluirIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public TipoSituacaoTAAIncluirIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(TipoSituacaoTAAIncluirIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "TipoSituacaoTAAIncluirIE", @event.Id, jsonMessage);

        }
    }
}