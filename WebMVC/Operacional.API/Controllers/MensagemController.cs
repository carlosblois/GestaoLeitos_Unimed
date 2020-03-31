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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using static Operacional.API.Enum.ExpoEnum;
using static Operacional.API.Utilitarios.Util;
using EventBus.Events;
using Configuracao.API.TO;
using Administrativo.API.TO;
using Operacional.API.Utilitarios;

namespace Operacional.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MensagemController : ControllerBase
    {
      

        private const string cachePrefix = "MENSAGEM#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<MensagemController> _localizer;

        public MensagemController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<MensagemController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui uma mensagem.
        /// </summary>
        /// <param name="mensagemToSave">
        /// Objeto que representa a mensagem da acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/mensagem")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirMensagem([FromBody]MensagemItem mensagemToSave)
        {

            var atividadeToValidate = _operacionalContext.AtividadeItems
                        .OfType<AtividadeItem>()
                        .SingleOrDefault(e => e.Id_AtividadeAcomodacao == mensagemToSave.Id_AtividadeAcomodacao && e.dt_FimAtividadeAcomodacao == null);

            if (atividadeToValidate == null)
            {

                string msgStatus = _localizer["VALIDA_ATIVIDADEATIVA"];
                return BadRequest(msgStatus);
            }

            _operacionalContext.MensagemItems.Add(mensagemToSave);

            //Create Integration Event to be published through the Event Bus
            var mensagemSaveEvent = new MensagemSaveIE(mensagemToSave.Id_Mensagem,
                                                                        mensagemToSave.Id_AtividadeAcomodacao,
                                                                        mensagemToSave.dt_EnvioMensagem,
                                                                        mensagemToSave.dt_RecebimentoMensagem,
                                                                        mensagemToSave.Id_Empresa,
                                                                        mensagemToSave.Id_Usuario_Emissor,
                                                                        mensagemToSave.Id_Usuario_Destinatario,
                                                                        mensagemToSave.TextoMensagem);
            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndMensagemContextChangesAsync(mensagemSaveEvent, mensagemToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(mensagemSaveEvent);


            return CreatedAtAction(nameof(IncluirMensagem), mensagemToSave.Id_Mensagem);
        }


        /// <summary>
        /// MENSAGEM Listar mensagens de uma Atividade.
        /// </summary>
        /// <param name="IdAtividade">
        /// Identificador da Atividade
        /// </param>
        [HttpGet]
        [Route("items/mensagem/idatividade/{IdAtividade}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarMensagemTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListarMensagensPorIdAtividade(int IdAtividade)
        {
            List<ConsultarMensagemTO> l_ListTO = new List<ConsultarMensagemTO>();

            if (_settings.UseCache)
            {
                Cache<ConsultarMensagemTO> mycache = new Cache<ConsultarMensagemTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ListarMensagensPorIdAtividade_" + cachePrefix + IdAtividade.ToString());

                if (l_ListTO.Count == 0)
                {
                    ConsultarMensagemTO sqlClass = new ConsultarMensagemTO();
                    sqlClass.ConsultaMensagemPorIdAtividadeTOCommand(IdAtividade.ToString(), _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ListarMensagensPorIdAtividade_" + cachePrefix + IdAtividade.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarMensagemTO sqlClass = new ConsultarMensagemTO();
                sqlClass.ConsultaMensagemPorIdAtividadeTOCommand(IdAtividade.ToString(), _settings.ConnectionString, ref l_ListTO);
            }
            
            return Ok(l_ListTO);

        }


        /// <summary>
        /// Inclui uma mensagem.
        /// </summary>
        /// <param name="idMensagem">
        /// Id da mensagem lida
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/mensagem/idMensagem/{idMensagem}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AtualizaRetornoMensagem(int idMensagem)
        {

            var mensagemToValidate = _operacionalContext.MensagemItems
                        .OfType<MensagemItem>()
                        .SingleOrDefault(e => e.Id_Mensagem == idMensagem && e.dt_RecebimentoMensagem == null);

            if (mensagemToValidate == null)
            {

                string msgStatus = _localizer["VALIDA_MENSAGEMCOMRETORNO"];
                return BadRequest(msgStatus);
            }

            MensagemItem ObjMensagem = _operacionalContext.MensagemItems.Find(idMensagem);

            ObjMensagem.dt_RecebimentoMensagem = DateTime.Now;

            _operacionalContext.MensagemItems.Update(ObjMensagem);

            //Create Integration Event to be published through the Event Bus
            var mensagemRetornoEvent = new MensagemRetornoIE(mensagemToValidate.Id_Mensagem,
                                                                        mensagemToValidate.Id_AtividadeAcomodacao,
                                                                        mensagemToValidate.dt_EnvioMensagem,
                                                                        mensagemToValidate.dt_RecebimentoMensagem,
                                                                        mensagemToValidate.Id_Empresa,
                                                                        mensagemToValidate.Id_Usuario_Emissor,
                                                                        mensagemToValidate.Id_Usuario_Destinatario,
                                                                        mensagemToValidate.TextoMensagem);
            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndMensagemRetornoContextChangesAsync(mensagemRetornoEvent, ObjMensagem);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }

            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(mensagemRetornoEvent);


            return CreatedAtAction(nameof(AtualizaRetornoMensagem), mensagemToValidate.Id_Mensagem);
        }


    }




}