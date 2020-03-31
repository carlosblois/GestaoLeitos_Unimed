using System;
using System.Collections.Generic;
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
    public class EmpresaPerfilModuloController : ControllerBase
    {

        private const string cachePrefix = "EMPRESAPERFILMODULO#";
        private ModuloContext _moduloContext;
        private readonly ModuloSettings _settings;
        private readonly IModuloIntegrationEventService _moduloIntegrationEventService;

        public EmpresaPerfilModuloController(ModuloContext context, IOptionsSnapshot<ModuloSettings> settings, IModuloIntegrationEventService moduloIntegrationEventService)
        {
            _moduloContext = context ?? throw new ArgumentNullException(nameof(context));
            _moduloIntegrationEventService = moduloIntegrationEventService ?? throw new ArgumentNullException(nameof(moduloIntegrationEventService));
            _settings = settings.Value;

            //((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Inclui uma associação de modulo e perfil.
        /// </summary>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SalvarEmpresaPerfilModulo([FromBody]EmpresaPerfilModuloItem empresaPerfilModuloToSave)
        {

            if (_moduloContext.Set<EmpresaPerfilModuloItem>().Any(e => e.Id_Empresa  == empresaPerfilModuloToSave.Id_Empresa && e.Id_Perfil == empresaPerfilModuloToSave.Id_Perfil && e.Id_Modulo == empresaPerfilModuloToSave.Id_Modulo))
            {
                _moduloContext.EmpresaPerfilModuloItems.Update(empresaPerfilModuloToSave);
            }
            else
            {
                _moduloContext.EmpresaPerfilModuloItems.Add(empresaPerfilModuloToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var empresaPerfilModuloInclusaoEvent = new EmpresaPerfilModuloInclusaoIE(empresaPerfilModuloToSave.Id_Empresa, empresaPerfilModuloToSave.Id_Perfil, empresaPerfilModuloToSave.Id_Modulo );

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.SaveEventAndEmpresaPefilModuloContextChangesAsync(empresaPerfilModuloInclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(empresaPerfilModuloInclusaoEvent);


            return CreatedAtAction(nameof(SalvarEmpresaPerfilModulo), null);
        }

        /// <summary>
        /// Exclui uma associação de modulo e perfil.
        /// </summary>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirEmpresaPerfilModulo(int id_Empresa, int id_Perfil, int id_Modulo)
        {
            if (id_Empresa < 1 || id_Perfil < 1|| id_Modulo < 1)
            {
                return BadRequest();
            }


            var empresaPerfilModuloToDelete = _moduloContext.EmpresaPerfilModuloItems
                            .OfType<EmpresaPerfilModuloItem>()
                            .SingleOrDefault(c => c.Id_Empresa == id_Empresa && c.Id_Perfil  == id_Perfil && c.Id_Modulo == id_Modulo);

            if (empresaPerfilModuloToDelete is null)
            {
                return BadRequest();
            }

            _moduloContext.EmpresaPerfilModuloItems.Remove(empresaPerfilModuloToDelete);

            //Create Integration Event to be published through the Event Bus
            var empresaPerfilModuloExclusaoEvent = new EmpresaPerfilModuloExclusaoIE(empresaPerfilModuloToDelete.Id_Empresa, empresaPerfilModuloToDelete.Id_Perfil, empresaPerfilModuloToDelete.Id_Modulo )   ;

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.DeleteEventAndOperacaoContextChangesAsync(empresaPerfilModuloExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(empresaPerfilModuloExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirEmpresaPerfilModulo), null);
        }

    }


}