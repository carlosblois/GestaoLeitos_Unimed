using EventBus.Events;
using System;
using System.Collections.Generic;
using Operacional.API.TO;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class GeraAcaoAcomodacaoSolicitarIE : IntegrationEvent
    {
        public int AcaoAtividadeAcomodacaoId { get; set; }
        public int AtividadeAcomodacaoId { get; set; }
        public int TipoAcaoAcomodacaoId { get; set; }
        public DateTime InicioAcaoAtividadeDt { get; set; }
        public DateTime? FimAcaoAtividadeDt { get; set; }
        public int? SLAId { get; set; }
        public string IdUsuarioExecutorId { get; set; }
        public string TipoAcaoTEXTO { get; set; }
        public List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis { get; set; } 
        

        public GeraAcaoAcomodacaoSolicitarIE(string tipoAcaoTEXTO,int acaoAtividadeAcomodacaoId ,int atividadeAcomodacaoId,
            int tipoAcaoAcomodacaoId,DateTime inicioAcaoAtividadeDt,
            DateTime? fimAcaoAtividadeDt, int? sLAId, string idUsuarioExecutorId, List<ConsultarAcessoAtividadeEmpresaPerfilTO> perfilTOs)
        {
            AcaoAtividadeAcomodacaoId = acaoAtividadeAcomodacaoId;
            AtividadeAcomodacaoId = atividadeAcomodacaoId;
            TipoAcaoAcomodacaoId = tipoAcaoAcomodacaoId;
            InicioAcaoAtividadeDt = inicioAcaoAtividadeDt;
            FimAcaoAtividadeDt = fimAcaoAtividadeDt;
            SLAId = sLAId;
            IdUsuarioExecutorId = idUsuarioExecutorId;
            TipoAcaoTEXTO = tipoAcaoTEXTO;
            Perfis = perfilTOs;
        }

    }
}
