using EventBus.Events;
using Modulo.API.Model;
using System;
using System.Collections.Generic;

namespace Modulo.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ModuloInclusaoIE : IntegrationEvent
    {
        public int ModuloId { get; set; }
        public string ModuloNome { get; private set; }
        public List<OperacaoItem> OperacaoItems { get;  set; }
        public List<EmpresaPerfilModuloItem> EmpresaPerfilModuloItems { get; set; }

        public ModuloInclusaoIE(int moduloId, string moduloNome)
        {
            ModuloId = moduloId;
            ModuloNome = moduloNome;
        }        

    }
    public class ModuloInclusaoGrupoIE : IntegrationEvent
    {

        public IEnumerable<ModuloItem> ModuloItems { get; }

        public ModuloInclusaoGrupoIE(IEnumerable<ModuloItem> moduloItems)
        {
            ModuloItems = moduloItems;
        }
    }
}
