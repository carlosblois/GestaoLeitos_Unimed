
using Configuracao.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class ChecklistItemChecklistIncluirIEHandler :
        IIntegrationEventHandler<ChecklistItemChecklistIncluirIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChecklistItemChecklistIncluirIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(ChecklistItemChecklistIncluirIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "ChecklistItemChecklistIncluirIE", @event.Id, jsonMessage);

        }
    }
}