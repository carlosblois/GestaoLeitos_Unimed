
using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Administrativo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class EmpresaSaveIE : IntegrationEvent
    {

        public int EmpresaId { get;  set; }
        public string EmpresaNome { get; private set; }
        public string EmpresaCodExterno { get; private set; }
        public string EmpresaEndereco { get; private set; }
        public string EmpresaComplemento { get; private set; }
        public string EmpresaNumero { get; private set; }
        public string EmpresaBairro { get; private set; }
        public string EmpresaCidade { get; private set; }
        public string EmpresaEstado { get; private set; }
        public string EmpresaCep { get; private set; }
        public string EmpresaFone { get; private set; }
        public string EmpresaContato { get; private set; }
        public string EmpresaCGC { get; private set; }
        public string EmpresaInscricaoMunicipal { get; private set; }
        public string EmpresaInscricaoEstadual { get; private set; }
        public string EmpresaCNES { get; private set; }

        public EmpresaSaveIE(string empresaNome, string empresaCodExterno,
            string empresaEndereco,string empresaComplemento,string empresaNumero,string empresaBairro,
            string empresaCidade, string empresaEstado, string empresaCep, string empresaFone,
            string empresaContato, string empresaCGC, string empresaInscricaoMunicipal, string empresaInscricaoEstadual,
            string empresaCNES)
        {
            EmpresaNome = empresaNome;
            EmpresaCodExterno = empresaCodExterno;
            EmpresaEndereco = empresaEndereco;
            EmpresaComplemento = empresaComplemento;
            EmpresaNumero = empresaNumero;
            EmpresaBairro = empresaBairro;
            EmpresaCidade = empresaCidade;
            EmpresaEstado = empresaEstado;
            EmpresaCep = empresaCep;
            EmpresaFone = empresaFone;
            EmpresaContato = empresaContato;
            EmpresaCGC = empresaCGC;
            EmpresaInscricaoMunicipal = empresaInscricaoMunicipal;
            EmpresaInscricaoEstadual = empresaInscricaoEstadual;
            EmpresaCNES = empresaCNES;
        }

    }

}
