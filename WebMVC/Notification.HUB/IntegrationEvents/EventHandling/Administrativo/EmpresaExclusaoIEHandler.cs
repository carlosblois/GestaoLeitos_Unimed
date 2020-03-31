﻿using Administrativo.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class EmpresaExclusaoIEHandler :
        IIntegrationEventHandler<EmpresaExclusaoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmpresaExclusaoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(EmpresaExclusaoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "EmpresaExclusaoIE", @event.Id, jsonMessage);

        }
    }
}