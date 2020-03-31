using EventBus.Events;

using System;
using System.Collections.Generic;

namespace Administrativo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class TipoAcomodacaoSaveIE : IntegrationEvent
    {
            public int EmpresaId { get; private set; }
            public int TipoAcomodacaoId { get;  set; }
            public string TipoAcomodacaoNome { get; private set; }
            public string TipoAcomodacaoCodExterno { get; private set; }
            public int TipoAcomodacaoIdCaracteristica { get; private set; }

        public TipoAcomodacaoSaveIE(int empresaId, int tipoacomodacaoId, string tipoacomodacaoNome, string tipoAcomodacaoCodExterno,int tipoAcomodacaoIdCaracteristica)
            {
                EmpresaId = empresaId;
                TipoAcomodacaoId = tipoacomodacaoId;
                TipoAcomodacaoNome = tipoacomodacaoNome;
                TipoAcomodacaoCodExterno = tipoAcomodacaoCodExterno;
                TipoAcomodacaoIdCaracteristica = tipoAcomodacaoIdCaracteristica;
            }

        }

}
