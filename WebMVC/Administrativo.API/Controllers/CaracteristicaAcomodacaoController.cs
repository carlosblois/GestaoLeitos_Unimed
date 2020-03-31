using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Administrativo.API.Infrastructure;
using Administrativo.API.IntegrationEvents;
using Administrativo.API.IntegrationEvents.Events;
using Administrativo.API.Model;
using Administrativo.API.TO;
using CacheRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Administrativo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CaracteristicaAcomodacaoController : ControllerBase
    {

        private const string cachePrefix = "CARACTERISTICAACOMODACAO#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<CaracteristicaAcomodacaoController> _localizer;

        public CaracteristicaAcomodacaoController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<CaracteristicaAcomodacaoController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza uma Caracteristica de Acomodação.
        /// </summary>
        /// <param name="caracteristicaAcomodacaoToSave">
        /// Objeto que representa uma Caracteristica de Acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarCaracteristicaAcomodacao([FromBody]CaracteristicaAcomodacaoItem caracteristicaAcomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomeCaracteristica(caracteristicaAcomodacaoToSave.nome_CaracteristicaAcomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<CaracteristicaAcomodacaoItem>().Any(e => e.id_CaracteristicaAcomodacao == caracteristicaAcomodacaoToSave.id_CaracteristicaAcomodacao))
            {
                _administrativoContext.CaracteristicaAcomodacaoItems.Update(caracteristicaAcomodacaoToSave);
            }
            else
            {
                _administrativoContext.CaracteristicaAcomodacaoItems.Add(caracteristicaAcomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var caracteristicaAcomodacaoSaveEvent = new CaracteristicaAcomodacaoSaveIE(caracteristicaAcomodacaoToSave.id_CaracteristicaAcomodacao, caracteristicaAcomodacaoToSave.nome_CaracteristicaAcomodacao);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndCaracteristicaAcomodacaoContextChangesAsync(caracteristicaAcomodacaoSaveEvent, caracteristicaAcomodacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeCaracteristicaAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(caracteristicaAcomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarCaracteristicaAcomodacao), caracteristicaAcomodacaoToSave.id_CaracteristicaAcomodacao);
        }

        /// <summary>
        /// Exclui uma caracteristica de acomodação.
        /// </summary>
        /// <param name="id_caracteristicaAcomodacao">
        /// Identificador da Caracteristica de Acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirCaracteristicaAcomodacao(int id_caracteristicaAcomodacao)
        {

            if (id_caracteristicaAcomodacao < 1)
            {
                return BadRequest();
            }

            var caracteristicaAcomodacaoToDelete = _administrativoContext.CaracteristicaAcomodacaoItems
                .OfType<CaracteristicaAcomodacaoItem>()
                .SingleOrDefault(c => c.id_CaracteristicaAcomodacao== id_caracteristicaAcomodacao);

            if (caracteristicaAcomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.CaracteristicaAcomodacaoItems.Remove(caracteristicaAcomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var caracteristicaAcomodacaoExclusaoEvent = new CaracteristicaAcomodacaoExclusaoIE(caracteristicaAcomodacaoToDelete.id_CaracteristicaAcomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndCaracteristicaAcomodacaoContextChangesAsync(caracteristicaAcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(caracteristicaAcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirCaracteristicaAcomodacao), null);
        }

        /// <summary>
        /// Consulta Caracteristicas de Acomodação.
        /// </summary>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<CaracteristicaAcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarCaracteristicaAcomodacao()
        {
            List<ConsultarCaracteristicaAcomodacaoTO> l_ListCaracteristicaAcomodacaoTO = new List<ConsultarCaracteristicaAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarCaracteristicaAcomodacaoTO> mycache = new Cache<ConsultarCaracteristicaAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListCaracteristicaAcomodacaoTO = await mycache.GetListAsync("ConsultarCaracteristicaAcomodacao_" + cachePrefix);
                if (l_ListCaracteristicaAcomodacaoTO.Count == 0)
                {
                    ConsultarCaracteristicaAcomodacaoTO sqlClass = new ConsultarCaracteristicaAcomodacaoTO();
                    sqlClass.ConsultarCaracteristicaAcomodacaoTOCommand( _settings.ConnectionString, ref l_ListCaracteristicaAcomodacaoTO);

                    if (l_ListCaracteristicaAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarCaracteristicaAcomodacao_" + cachePrefix, l_ListCaracteristicaAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarCaracteristicaAcomodacaoTO sqlClass = new ConsultarCaracteristicaAcomodacaoTO();
                sqlClass.ConsultarCaracteristicaAcomodacaoTOCommand( _settings.ConnectionString, ref l_ListCaracteristicaAcomodacaoTO);
            }


            return Ok(l_ListCaracteristicaAcomodacaoTO);

        }

        private bool ruleValidaNomeCaracteristicaAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_CARACTERISTICAACOMODACAONOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["CaracteristicaAcomodacaoNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomeCaracteristica(string nome_Caracteristica, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Caracteristica ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Caracteristica.Length < 3) || (nome_Caracteristica.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }

    }
}