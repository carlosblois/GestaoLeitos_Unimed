using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AcaoSaveIE : IntegrationEvent
    {

        public int AcaoAtividadeAcomodacaoId { get; set; }
        public int AtividadeAcomodacaoId { get; set; }
        public int TipoAcaoAcomodacaoId { get; set; }
        public DateTime InicioAcaoAtividadeDt{ get; set; }
        public DateTime?FimAcaoAtividadeDt { get; set; }
        public int? SLAId { get; set; }
        public string  UsuarioExecutorId { get; set; }


        public AcaoSaveIE(int acaoAtividadeAcomodacaoId, int atividadeAcomodacaoId, int tipoAcaoAcomodacaoId,
                                         DateTime inicioAcaoAtividadeDt,
                                         DateTime? fimAcaoAtividadeDt, int? sLAId, string usuarioExecutorId)
        {

            AcaoAtividadeAcomodacaoId = acaoAtividadeAcomodacaoId;
            AtividadeAcomodacaoId = atividadeAcomodacaoId;
            TipoAcaoAcomodacaoId = tipoAcaoAcomodacaoId;
            InicioAcaoAtividadeDt = inicioAcaoAtividadeDt;
            FimAcaoAtividadeDt = fimAcaoAtividadeDt;
            SLAId = sLAId;
            UsuarioExecutorId = usuarioExecutorId;

        }

    }
}
