using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class SituacaoSaveIE : IntegrationEvent
    {
        public int SituacaoAcomodacaoId { get; set; }
        public int AcomodacaoId { get; set; }
        public int TipoSituacaoAcomodacaoId { get; set; }
        public DateTime InicioSituacaoAcomodacaoDt { get; set; }
        public DateTime? FimSituacaoAcomodacaoDt { get; set; }
        public string NumAtendimentoCod { get; set; }
        public int? SLAId { get; set; }
        public string PrioritarioCod { get; set; }

        public List<AtividadeItem> AtividadeItems { get; set; }

        public SituacaoSaveIE(int situacaoAcomodacaoId, 
                                            int acomodacaoId, 
                                            int tipoSituacaoAcomodacaoId,
                                            DateTime inicioSituacaoAcomodacaoDt, 
                                            DateTime? fimSituacaoAcomodacaoDt,
                                            string numAtendimentoCod, 
                                            int? sLAId, 
                                            string prioritarioCod,
                                            List<AtividadeItem> lstAtividade)
        {

                SituacaoAcomodacaoId = situacaoAcomodacaoId;
                AcomodacaoId = acomodacaoId;
                TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
                InicioSituacaoAcomodacaoDt = inicioSituacaoAcomodacaoDt;
                FimSituacaoAcomodacaoDt = fimSituacaoAcomodacaoDt;
                NumAtendimentoCod = numAtendimentoCod;
                SLAId = sLAId;
                PrioritarioCod = prioritarioCod;

                AtividadeItems = lstAtividade;

        }

    }
}
