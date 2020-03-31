using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class MensagemRetornoIE : IntegrationEvent
    {

        public int IdMensagem { get; set; }
        public int? IdAtividadeAcomodacao { get; set; }
        public DateTime Dt_EnvioMensagem { get; set; }
        public DateTime? Dt_RecebimentoMensagem { get; set; }
        public string LoginUsuarioEmissor { get; set; }
        public string LoginUsuarioDestinatario { get; set; }
        public string TextoMensagem { get; set; }


        public MensagemRetornoIE(int idMensagem, 
                              int? idAtividadeAcomodacao, 
                              DateTime dtEnvioMensagem,
                              DateTime? dtRecebimentoMensagem, 
                              string loginUsuarioEmissor, 
                              string loginUsuarioDestinatario, 
                              string textoMensagem)
        {
            IdMensagem = idMensagem;
            IdAtividadeAcomodacao = idAtividadeAcomodacao;
            Dt_EnvioMensagem = dtEnvioMensagem;
            Dt_RecebimentoMensagem = dtRecebimentoMensagem;
            LoginUsuarioEmissor = loginUsuarioEmissor;
            LoginUsuarioDestinatario = loginUsuarioDestinatario;
            TextoMensagem = textoMensagem;
        }

    }
}
