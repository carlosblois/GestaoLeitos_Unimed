using EventBus.Events;
using System.Collections.Generic;

namespace Notification.HUB.IntegrationEvents.Events
{

    public class UsuarioInclusaoIntegrationEvent : IntegrationEvent
    {
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; private set; }
        public  List<UsuarioEmpresaPerfilItem> UsuarioEmpresaPerfilItems { get; set; }

        public UsuarioInclusaoIntegrationEvent(int usuarioId, string usuarioNome)
        {
            UsuarioId = usuarioId;
            UsuarioNome = usuarioNome;
        }

        public class UsuarioEmpresaPerfilItem
        {
            public int id_Usuario { get; set; }

            public int id_Empresa { get; set; }

            public int id_Perfil { get; set; }
        }

    }


}
