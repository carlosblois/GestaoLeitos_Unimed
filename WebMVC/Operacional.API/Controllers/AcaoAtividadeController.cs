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

namespace Operacional.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AcaoAtividadeAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "ACAO#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<AtividadeAcomodacaoController> _localizer;

        public AcaoAtividadeAcomodacaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<AtividadeAcomodacaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Consulta as acoes de uma atividade de um usuario de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/usuario/{idUsuario}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesNaoFinalizadasPorIdEmpresaPorUsuario(int idEmpresa, int idUsuario)
        {
            List<ConsultarAtividadeTO> l_ListAtividadeTO = new List<ConsultarAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeTO> mycache = new Cache<ConsultarAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync(cachePrefix + idEmpresa.ToString() + "@" + idUsuario.ToString());
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                    sqlClass.ConsultarAtividadeAcomodacaoTOCommand(idEmpresa, idUsuario, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync(cachePrefix + idEmpresa.ToString() + "@" + idUsuario.ToString(), l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                sqlClass.ConsultarAtividadeAcomodacaoTOCommand(idEmpresa, idUsuario, _settings.ConnectionString, ref l_ListAtividadeTO);
            }

            if (l_ListAtividadeTO.Count > 0)
            {
                return Ok(l_ListAtividadeTO);
            }

            return NotFound();
        }


        private bool ruleValidaNomeEmpresaUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_EMPRESANOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["EmpresaNomeUnique"];
                return true;
            }

            return false;

        }
    }
}