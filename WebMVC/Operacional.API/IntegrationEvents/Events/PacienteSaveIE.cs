using EventBus.Events;
using Operacional.API.Model;
using System;
using System.Collections.Generic;

namespace Operacional.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class PacienteSaveIE : IntegrationEvent
    {

        public int PacienteId { get; set; }
        public string NomePaciente { get; set; }
        public string CodExterno { get; set; }
        public string GeneroPaciente { get; set; }
        public DateTime DataNascimentoPaciente { get; set; }
        public string PendenciaFinanceira { get; set; }

        public PacienteSaveIE(int pacienteId,
                                            string nomePaciente,
                                            string codExterno,
                                            string generoPaciente,
                                            DateTime dataNascimentoPaciente,
                                            string pendenciaFinanceira)
        {

                        PacienteId = pacienteId;
                        NomePaciente = nomePaciente;
                        CodExterno = codExterno;
                        GeneroPaciente = generoPaciente;
                        DataNascimentoPaciente = dataNascimentoPaciente;
                        PendenciaFinanceira = pendenciaFinanceira;


        }

    }
}
