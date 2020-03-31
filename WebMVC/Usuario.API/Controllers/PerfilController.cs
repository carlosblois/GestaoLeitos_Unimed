using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Usuario.API.Infrastructure;
using Usuario.API.IntegrationEvents;
using Usuario.API.IntegrationEvents.Events;
using Usuario.API.Model;
using Usuario.API.TO;

namespace Usuario.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {

        private const string cachePrefix = "PERFIL#";
        private UsuarioContext _usuarioContext;
        private readonly UsuarioSettings _settings;
        private readonly IUsuarioIntegrationEventService _usuarioIntegrationEventService;
        private readonly IStringLocalizer<PerfilController> _localizer;

        public PerfilController(UsuarioContext context, IOptionsSnapshot<UsuarioSettings> settings, IUsuarioIntegrationEventService usuarioIntegrationEventService, IStringLocalizer<PerfilController> localizer)
        {
            _usuarioContext = context ?? throw new ArgumentNullException(nameof(context));
            _usuarioIntegrationEventService = usuarioIntegrationEventService ?? throw new ArgumentNullException(nameof(usuarioIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Inclui / atualiza um perfil.
        /// Permite a inclusão em conjunto da coleção de perfis para uma determinada empresa. (opcional)
        /// Durante a atualização apenas o perfil é considerado.
        /// </summary>
        /// <param name="perfilToSave">
        /// Objeto que representa o Perfil
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SalvarPerfil([FromBody]PerfilItem perfilToSave)
        {

            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaNomePerfil(perfilToSave.nome_Perfil, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_usuarioContext.Set<PerfilItem>().Any(e => e.id_Perfil == perfilToSave.id_Perfil))
            {
                _usuarioContext.PerfilItems.Update(perfilToSave);
            }
            else
            {
                _usuarioContext.PerfilItems.Add(perfilToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var perfilSaveEvent = new PerfilSaveIE(perfilToSave.id_Perfil, perfilToSave.nome_Perfil);

            try
            { 
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndPerfilContextChangesAsync(perfilSaveEvent, perfilToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomePerfilUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(perfilSaveEvent);


            return CreatedAtAction(nameof(SalvarPerfil), perfilToSave.id_Perfil);
        }

        /// <summary>
        /// Consulta os perfis.
        /// </summary>
        [HttpGet]
        [Route("items/")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarPerfisTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarPerfis()
        {
            List<ConsultarPerfisTO> l_ListPerfilTO = new List<ConsultarPerfisTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarPerfisTO> mycache = new Cache<ConsultarPerfisTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListPerfilTO = await mycache.GetListAsync("ConsultarPerfis_" + cachePrefix);
                if (l_ListPerfilTO.Count == 0)
                {
                    ConsultarPerfisTO sqlClass = new ConsultarPerfisTO();
                    sqlClass.ConsultarPerfisTOCommand( _settings.ConnectionString, ref l_ListPerfilTO);

                    if (l_ListPerfilTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarPerfis_" + cachePrefix , l_ListPerfilTO);
                    }
                }
            }
            else
            {
                ConsultarPerfisTO sqlClass = new ConsultarPerfisTO();
                sqlClass.ConsultarPerfisTOCommand( _settings.ConnectionString, ref l_ListPerfilTO);
            }


                return Ok(l_ListPerfilTO);

        }

        /// <summary>
        /// Exclui um perfil.
        /// </summary>
        /// <param name="id_Perfil">
        /// Identificador do Perfil
        /// </param>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirPerfil(int id_Perfil)
        {
            if (id_Perfil < 1)
            {
                return BadRequest();
            }


            var perfilToDelete = _usuarioContext.PerfilItems.Include(b => b.EmpresaPerfilItems)
                            .OfType<PerfilItem>()
                            .SingleOrDefault(c => c.id_Perfil == id_Perfil);

            if (perfilToDelete is null)
            {
                return BadRequest();
            }

            _usuarioContext.PerfilItems.Remove(perfilToDelete);

            //Create Integration Event to be published through the Event Bus
            var usuarioExclusaoEvent = new UsuarioExclusaoIE(perfilToDelete.id_Perfil);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndUsuarioContextChangesAsync(usuarioExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirPerfil), null);
        }

        private bool ruleValidaNomePerfilUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_PERFILNOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["PerfilNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomePerfil(string nome_Perfil, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Perfil ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Perfil.Length < 3) || (nome_Perfil.Length >20))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;
        }
    }

}