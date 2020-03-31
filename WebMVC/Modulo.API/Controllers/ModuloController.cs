using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modulo.API.Model;
using Modulo.API.Infrastructure;
using Modulo.API.IntegrationEvents;
using Modulo.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Authorization;

namespace Modulo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloController : ControllerBase
    {

        private const string cachePrefix = "MODULO#";
        private ModuloContext _moduloContext;
        private readonly ModuloSettings _settings;
        private readonly IModuloIntegrationEventService _moduloIntegrationEventService;

        public ModuloController(ModuloContext context, IOptionsSnapshot<ModuloSettings> settings, IModuloIntegrationEventService moduloIntegrationEventService)
        {
            _moduloContext = context ?? throw new ArgumentNullException(nameof(context));
            _moduloIntegrationEventService = moduloIntegrationEventService ?? throw new ArgumentNullException(nameof(moduloIntegrationEventService));
            _settings = settings.Value;

            //((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        /// <summary>
        /// Inclui / atualiza um módulo.
        /// Permite a inclusão em conjunto da coleção de operações para um determinado módulo. (opcional)
        /// Durante a atualização apenas o módulo é considerado.
        /// </summary>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SalvarModulo([FromBody]ModuloItem moduloToSave)
        {

            if (_moduloContext.Set<ModuloItem>().Any(e => e.Id_Modulo == moduloToSave.Id_Modulo ))
            {
                _moduloContext.ModuloItems.Update(moduloToSave);
            }
            else
            {
                _moduloContext.ModuloItems.Add(moduloToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var moduloInclusaoEvent = new ModuloInclusaoIE(moduloToSave.Id_Modulo, moduloToSave.Nome_Modulo);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.SaveEventAndModuloContextChangesAsync(moduloInclusaoEvent, moduloToSave);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(moduloInclusaoEvent);


            return CreatedAtAction(nameof(SalvarModulo), moduloToSave.Id_Modulo);
        }

        /// <summary>
        /// Exclui um modulo.
        /// </summary>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirModulo(int id_Modulo)
        {
            if (id_Modulo < 1)
            {
                return BadRequest();
            }


            var moduloToDelete = _moduloContext.ModuloItems
                            .Include(b => b.OperacaoItems)
                            .Include(d => d.EmpresaPerfilModuloItems)
                            .OfType<ModuloItem>()
                            .SingleOrDefault(c => c.Id_Modulo == id_Modulo);

            if (moduloToDelete is null)
            {
                return BadRequest();
            }

            _moduloContext.ModuloItems.Remove(moduloToDelete);

            //Create Integration Event to be published through the Event Bus
            var moduloExclusaoEvent = new ModuloExclusaoIE(moduloToDelete.Id_Modulo);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.DeleteEventAndModuloContextChangesAsync(moduloExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(moduloExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirModulo), null);
        }

    }


}