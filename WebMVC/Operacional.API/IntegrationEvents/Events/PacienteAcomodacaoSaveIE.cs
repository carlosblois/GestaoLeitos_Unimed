using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class PacienteAcomodacaoSaveIE : IntegrationEvent
    {
        public int PacienteAcomodacaoId { get; set; }
        public int PacienteId { get; set; }
        public DateTime EntradaDt { get; set; }
        public DateTime? SaidaDt { get; set; }
        public int AcomodacaoId { get; set; }
        public string NumAtendimento { get; set; }


        public PacienteAcomodacaoSaveIE(int pacienteAcomodacaoId,
                                            int pacienteId,
                                            DateTime entradaDt,
                                            DateTime? saidaDt,
                                            int acomodacaoId,
                                            string numAtendimento)
        {
            PacienteAcomodacaoId = pacienteAcomodacaoId;
            PacienteId = pacienteId;
            EntradaDt = entradaDt;
            SaidaDt = saidaDt;
            AcomodacaoId = acomodacaoId;
            NumAtendimento = numAtendimento;


        }

    }
}
