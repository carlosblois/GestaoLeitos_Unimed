using EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Modulo.API.IntegrationEvents.Events;
using Newtonsoft.Json;

using System;

using System.Threading.Tasks;

namespace Notification.HUB.IntegrationEvents.EventHandling
{


    public class EmpresaPerfilModuloInclusaoGrupoIEHandler :
        IIntegrationEventHandler<EmpresaPerfilModuloInclusaoGrupoIE>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmpresaPerfilModuloInclusaoGrupoIEHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(EmpresaPerfilModuloInclusaoGrupoIE @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", "EmpresaPerfilModuloInclusaoGrupoIE", @event.Id, jsonMessage);

        }
    }
}