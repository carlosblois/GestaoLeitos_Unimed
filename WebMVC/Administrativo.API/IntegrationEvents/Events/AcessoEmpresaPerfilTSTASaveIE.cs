using EventBus.Events;

using System;
using System.Collections.Generic;

namespace Administrativo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class AcessoEmpresaPerfilTSTASaveIE : IntegrationEvent
    {
        public int AcessoEmpresaPerfilTipoSituacaoTipoAtividadeId { get;  set; }
        public int PerfilId { get; private set; }
        public int EmpresaId { get; private set; }
        public int TipoSituacaoAcomodacaoId { get; private set; }
        public int TipoAtividadeAcomodacaoId { get; private set; }
        public string Cod_Tipo { get; private set; }


        public AcessoEmpresaPerfilTSTASaveIE( int acessoEmpresaPerfilTipoSituacaoTipoAtividadeId, 
                                                            int perfilId, 
                                                            int empresaId, 
                                                            int tipoSituacaoAcomodacaoId, 
                                                            int tipoAtividadeAcomodacaoId, 
                                                            string cod_Tipo)
        {
            AcessoEmpresaPerfilTipoSituacaoTipoAtividadeId = acessoEmpresaPerfilTipoSituacaoTipoAtividadeId;
            PerfilId = perfilId;
            EmpresaId = empresaId;
            TipoSituacaoAcomodacaoId = tipoSituacaoAcomodacaoId;
            TipoAtividadeAcomodacaoId =tipoAtividadeAcomodacaoId;
            Cod_Tipo = cod_Tipo;
        }

    }

}
