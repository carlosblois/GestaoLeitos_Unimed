using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{

    public class FinalizaAtividadeAcomodacaoIE : IntegrationEvent
    {
        public int AtividadeAcomodacaoId { get; set; }
        public DateTime? FimAtividadeDt { get; set; }
        public string TipoAtividadeTEXTO { get; set; }


        public FinalizaAtividadeAcomodacaoIE(string tipoAtividadeTEXTO, int atividadeAcomodacaoId, DateTime? fimAtividadeDt)
        {
            AtividadeAcomodacaoId = atividadeAcomodacaoId;
            FimAtividadeDt = fimAtividadeDt;
            TipoAtividadeTEXTO = tipoAtividadeTEXTO;

        }
    }
}
