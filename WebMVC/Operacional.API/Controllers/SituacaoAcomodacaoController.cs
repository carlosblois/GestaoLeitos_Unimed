using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Operacional.API.Infrastructure;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Operacional.API.IntegrationEvents;

using Operacional.API.Model;
using Microsoft.Extensions.Localization;
using Operacional.API.TO;
using Microsoft.AspNetCore.Authorization;
using Operacional.API.IntegrationEvents.Events;
using EventBus.Events;
using static Operacional.API.Enum.ExpoEnum;
using static Operacional.API.Utilitarios.Util;

namespace Operacional.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SituacaoAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "SITUACAO#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<SituacaoAcomodacaoController> _localizer;

        public SituacaoAcomodacaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<SituacaoAcomodacaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Consulta a situação ativa de uma acomodação utilizando o seu codigo de integração.
        /// </summary>
        /// <param name="CodExterno">
        /// Codigo de integracao da identificação da Acomodação
        /// </param>
        [HttpGet]
        [Route("items/situacao/{CodExterno}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeAcaoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarSituacaoPorCodExterno(string CodExterno)
        {
            List<ConsultarSituacaoTO> l_ListSituacaoTO = new List<ConsultarSituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarSituacaoTO> mycache = new Cache<ConsultarSituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListSituacaoTO = await mycache.GetListAsync("ConsultarSituacaoPorCodExterno_" + cachePrefix +
                                                                    CodExterno.ToString());
                if (l_ListSituacaoTO.Count == 0)
                {
                    ConsultarSituacaoTO sqlClass = new ConsultarSituacaoTO();
                    sqlClass.ConsultarSituacaoPorCodExternoTOCommand(CodExterno, _settings.ConnectionString, ref l_ListSituacaoTO);

                    if (l_ListSituacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarSituacaoPorCodExterno_" + cachePrefix +
                                                    CodExterno.ToString(), l_ListSituacaoTO);
                    }
                }
            }
            else
            {
                ConsultarSituacaoTO sqlClass = new ConsultarSituacaoTO();
                sqlClass.ConsultarSituacaoPorCodExternoTOCommand(CodExterno, _settings.ConnectionString, ref l_ListSituacaoTO);
            }


                return Ok(l_ListSituacaoTO);

        }


        /// <summary>
        /// Inclui uma situação.
        /// </summary>
        /// <param name="situacaoToSave">
        /// Objeto que representa a situação da acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirSituacao([FromBody]SituacaoItem situacaoToSave)
        {
            string msgRule = "";

            var situacaoToValidate = _operacionalContext.SituacaoItems
                        .OfType<SituacaoItem>()
                        .SingleOrDefault(e => e.Id_SituacaoAcomodacao == situacaoToSave.Id_Acomodacao && e.dt_FimSituacaoAcomodacao == null);

            if (situacaoToValidate != null)
            {
                if (situacaoToValidate.Id_TipoSituacaoAcomodacao == situacaoToSave.Id_TipoSituacaoAcomodacao)
                {
                    string msgStatus = _localizer["VALIDA_SITUACAOATIVA"];
                    return BadRequest(msgStatus);
                }
                else
                {
                    situacaoToValidate.dt_FimSituacaoAcomodacao = DateTime.Now;
                    _operacionalContext.SituacaoItems.Update(situacaoToValidate);
                }
            }

            _operacionalContext.SituacaoItems.Add(situacaoToSave);

            //Create Integration Event to be published through the Event Bus
            var situacaoSaveEvent = new SituacaoSaveIE(situacaoToSave.Id_SituacaoAcomodacao,
                                                                        situacaoToSave.Id_Acomodacao,
                                                                        situacaoToSave.Id_TipoSituacaoAcomodacao,
                                                                        situacaoToSave.dt_InicioSituacaoAcomodacao,
                                                                        situacaoToSave.dt_FimSituacaoAcomodacao,
                                                                        situacaoToSave.cod_NumAtendimento,
                                                                        situacaoToSave.Id_SLA,
                                                                        situacaoToSave.Cod_Prioritario,
                                                                        situacaoToSave.AtividadeItems
                                                                        );

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndSituacaoContextChangesAsync(situacaoSaveEvent, situacaoToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(situacaoSaveEvent);


            return CreatedAtAction(nameof(IncluirSituacao), situacaoToSave.Id_SituacaoAcomodacao);
        }

    }
}