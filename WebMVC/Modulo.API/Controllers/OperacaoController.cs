using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class OperacaoController : ControllerBase
    {

        private const string cachePrefix = "OPERACAO#";
        private ModuloContext _moduloContext;
        private readonly ModuloSettings _settings;
        private readonly IModuloIntegrationEventService _moduloIntegrationEventService;

        public OperacaoController(ModuloContext context, IOptionsSnapshot<ModuloSettings> settings, IModuloIntegrationEventService moduloIntegrationEventService)
        {
            _moduloContext = context ?? throw new ArgumentNullException(nameof(context));
            _moduloIntegrationEventService = moduloIntegrationEventService ?? throw new ArgumentNullException(nameof(moduloIntegrationEventService));
            _settings = settings.Value;

            //((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Inclui / atualiza uma operação.
        /// </summary>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SalvarOperacao([FromBody]OperacaoItem operacaoToSave)
        {

            if (_moduloContext.Set<OperacaoItem>().Any(e => e.Id_Operacao == operacaoToSave.Id_Operacao))
            {
                _moduloContext.OperacaoItems.Update(operacaoToSave);
            }
            else
            {
                _moduloContext.OperacaoItems.Add(operacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var operacaoInclusaoEvent = new OperacaoInclusaoIE(operacaoToSave.Id_Operacao, operacaoToSave.Nome_Operacao, operacaoToSave.Id_Operacao );

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.SaveEventAndOperacaoContextChangesAsync(operacaoInclusaoEvent, operacaoToSave);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(operacaoInclusaoEvent);


            return CreatedAtAction(nameof(SalvarOperacao), operacaoToSave.Id_Operacao);
        }

        /// <summary>
        /// Exclui uma operação.
        /// </summary>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirOperacao(int id_Operacao)
        {
            if (id_Operacao < 1)
            {
                return BadRequest();
            }


            var operacaoToDelete = _moduloContext.OperacaoItems
                            .OfType<OperacaoItem>()
                            .SingleOrDefault(c => c.Id_Operacao == id_Operacao);

            if (operacaoToDelete is null)
            {
                return BadRequest();
            }

            _moduloContext.OperacaoItems.Remove(operacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var operacaoExclusaoEvent = new OperacaoExclusaoIE(operacaoToDelete.Id_Operacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _moduloIntegrationEventService.DeleteEventAndOperacaoContextChangesAsync(operacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _moduloIntegrationEventService.PublishThroughEventBusAsync(operacaoExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirOperacao), null);
        }

    }


}