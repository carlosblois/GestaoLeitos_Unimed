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

        public int id_Mensagem { get; set; }
        public int? id_AtividadeAcomodacao { get; set; }
        public DateTime dt_EnvioMensagem { get; set; }
        public DateTime? dt_RecebimentoMensagem { get; set; }
        public int id_Empresa { get; set; }
        public int id_UsuarioEmissor { get; set; }
        public int? id_UsuarioDestinatario { get; set; }
        public string textoMensagem { get; set; }


        public MensagemRetornoIE(int idMensagem, 
                              int? idAtividadeAcomodacao, 
                              DateTime dtEnvioMensagem,
                              DateTime? dtRecebimentoMensagem, 
                              int idEmpresa,
                              int idUsuarioEmissor, 
                              int? idUsuarioDestinatario, 
                              string texto_Mensagem)
        {
            this.id_Mensagem = idMensagem;
            this.id_AtividadeAcomodacao = idAtividadeAcomodacao;
            this.dt_EnvioMensagem = dtEnvioMensagem;
            this.dt_RecebimentoMensagem = dtRecebimentoMensagem;
            this.id_Empresa = idEmpresa;
            this.id_UsuarioEmissor = idUsuarioEmissor;
            this.id_UsuarioDestinatario = idUsuarioDestinatario;
            this.textoMensagem = texto_Mensagem;
        }

    }
}
