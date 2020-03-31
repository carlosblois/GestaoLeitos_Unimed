using EventBus.Events;

using System;
using System.Collections.Generic;

namespace Administrativo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AcomodacaoSaveIE : IntegrationEvent
    {                
        public int AcomodacaoId { get; set; }
        public string AcomodacaoNome { get; private set; }
        public int TipoAcomodacaoId { get; set; }
        public int EmpresaId { get; private set; }
        public int SetorId { get; private set; }
        public string AcomodacaoCodExterno { get; private set; }
        public string CodIsolamento { get; private set; }



        public AcomodacaoSaveIE(string acomodacaoNome, int tipoacomodacaoId, int empresaId, int setorId, string acomodacaoCodExterno,string codIsolamento)
        {
            AcomodacaoNome = acomodacaoNome;
            TipoAcomodacaoId = tipoacomodacaoId;
            EmpresaId = empresaId;
            SetorId = setorId;
            AcomodacaoCodExterno = acomodacaoCodExterno;
            CodIsolamento = codIsolamento;
        }

    }

}
