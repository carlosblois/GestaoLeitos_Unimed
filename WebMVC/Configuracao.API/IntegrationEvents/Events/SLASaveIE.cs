using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Configuracao.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class SLASaveIE : IntegrationEvent
    {
        public int SLAId { get; set; }
        public int IdTipoSituacaoAcomodacao { get; private set; }
        public int IdTipoAtividadeAcomodacao { get; private set; }
        public int IdTipoAcaoAcomodacao { get; private set; }
        public int SLASituacaTempoMinutos { get; set; }
        public int SLASituacaoVersao { get; set; }
        public int IdEmpresa { get; private set; }

        public SLASaveIE(int sLAId, int idTipoSituacaoAcomodacao,int idTipoAtividadeAcomodacao, int idTipoAcaoAcomodacao,int idEmpresa, int sLASituacaTempoMinutos, int sLASituacaoVersao)
        {
            SLAId = sLAId;
            IdTipoSituacaoAcomodacao = idTipoSituacaoAcomodacao;
            IdTipoAtividadeAcomodacao = idTipoAtividadeAcomodacao;
            IdTipoAcaoAcomodacao = idTipoAcaoAcomodacao;
            IdEmpresa = idEmpresa;
            SLASituacaTempoMinutos = sLASituacaTempoMinutos;
            SLASituacaoVersao = sLASituacaoVersao;
        }

    }
}
