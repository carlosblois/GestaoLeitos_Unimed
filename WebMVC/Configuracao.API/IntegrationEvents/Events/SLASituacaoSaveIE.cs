using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class SLASituacaoSaveIE : IntegrationEvent
    {
        public int SLASituacaoId { get; set; }
        public int IdTipoSituacaoAcomodacao { get; private set; }
        public int IdEmpresa { get; private set; }
        public int SLASituacaTempoMinutos { get; set; }
        public int SLASituacaoVersao { get; set; }

        public SLASituacaoSaveIE(int sLASituacaoId,int idTipoSituacaoAcomodacao, int idEmpresa, int sLASituacaTempoMinutos,int sLASituacaoVersao)
        {
            SLASituacaoId = sLASituacaoId;
            IdTipoSituacaoAcomodacao = idTipoSituacaoAcomodacao;
            IdEmpresa = idEmpresa;
            SLASituacaTempoMinutos = sLASituacaTempoMinutos;
            SLASituacaoVersao = sLASituacaoVersao;
        }

    }
}
