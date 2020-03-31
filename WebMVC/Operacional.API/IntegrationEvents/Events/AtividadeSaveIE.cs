using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;
using Operacional.API.TO;
using Administrativo.API.TO;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AtividadeSaveIE : IntegrationEvent
    {

        public int AtividadeAcomodacaoId { get; set; }
        public int SituacaoAcomodacaoId { get; set; }
        public int TipoSituacaoAcomodacaoId { get; set; }
        public int TipoAtividadeAcomodacaoId { get; set; }
        public DateTime InicioAtividadeAcomodacaoDt { get; set; }
        public DateTime? FimAtividadeAcomodacaoDt { get; set; }
        public int UsuarioSolicitanteId { get; set; }
        public string PrioritarioCod { get; set; }
        public string PlusCod { get; set; }

        public List<AcaoItem> AcaoItems { get; set; }
        public List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis { get; set; }
        public int IdAcomodacao { get; set; }

        public AtividadeSaveIE(int atividadeAcomodacaoId, int situacaoAcomodacaoId, int tipoSituacaoAcomodacaoId,
            int tipoAtividadeAcomodacaoId, DateTime inicioAtividadeAcomodacaoDt,
            DateTime? fimAtividadeAcomodacaoDt, int usuarioSolicitanteId, List<AcaoItem> lstAcao,string prioritarioCod,string plusCod, 
            List<ConsultarAcessoAtividadeEmpresaPerfilTO> lstPerfis,
            int idAcomodacao)
        {

            AtividadeAcomodacaoId = atividadeAcomodacaoId;
            SituacaoAcomodacaoId = situacaoAcomodacaoId;
            TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
            TipoAtividadeAcomodacaoId = tipoAtividadeAcomodacaoId;
            InicioAtividadeAcomodacaoDt = inicioAtividadeAcomodacaoDt;
            FimAtividadeAcomodacaoDt = fimAtividadeAcomodacaoDt;
            UsuarioSolicitanteId = usuarioSolicitanteId;
            PrioritarioCod = prioritarioCod;
            PlusCod = plusCod;

            AcaoItems = lstAcao;

            Perfis = lstPerfis;

            IdAcomodacao = idAcomodacao;

        }

    }
}
