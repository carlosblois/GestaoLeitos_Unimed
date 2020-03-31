using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Modulo.API.IntegrationEvents.Events;
using Newtonsoft.Json;


using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{

    public class EmpresaPerfilModuloExclusaoGrupoIEHandler :
        IIntegrationEventHandler<EmpresaPerfilModuloExclusaoGrupoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmpresaPerfilModuloExclusaoGrupoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(EmpresaPerfilModuloExclusaoGrupoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "EmpresaPerfilModuloExclusaoGrupoIE", @event.Id, jsonMessage);

        }
    }
}