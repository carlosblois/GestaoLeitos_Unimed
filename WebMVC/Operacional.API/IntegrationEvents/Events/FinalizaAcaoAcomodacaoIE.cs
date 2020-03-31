using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{

    public class FinalizaAcaoAcomodacaoIE : IntegrationEvent
    {
        public int AcaoAtividadeAcomodacaoId { get; set; }
        public DateTime? FimAcaoAtividadeDt { get; set; }
        public string TipoAcaoTEXTO { get; set; }


        public FinalizaAcaoAcomodacaoIE(string tipoAcaoTEXTO, int acaoAtividadeAcomodacaoId, DateTime? fimAcaoAtividadeDt)
        {
            AcaoAtividadeAcomodacaoId = acaoAtividadeAcomodacaoId;
            FimAcaoAtividadeDt = fimAcaoAtividadeDt;
            TipoAcaoTEXTO = tipoAcaoTEXTO;

        }
    }
}
