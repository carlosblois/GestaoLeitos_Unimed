using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class UsuarioEmpresaPerfilController : ControllerBase
    {

        private const string cachePrefix = "USUARIOEMPRESAPERFIL#";
        private UsuarioContext _usuarioContext;
        private readonly UsuarioSettings _settings;
        private readonly IUsuarioIntegrationEventService _usuarioIntegrationEventService;
        private readonly IStringLocalizer<UsuarioEmpresaPerfilController> _localizer;

        public UsuarioEmpresaPerfilController(UsuarioContext context, IOptionsSnapshot<UsuarioSettings> settings, IUsuarioIntegrationEventService usuarioIntegrationEventService, IStringLocalizer<UsuarioEmpresaPerfilController> localizer)
        {
            _usuarioContext = context ?? throw new ArgumentNullException(nameof(context));
            _usuarioIntegrationEventService = usuarioIntegrationEventService ?? throw new ArgumentNullException(nameof(usuarioIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui um perfil especifico para um usuário de uma empresa.
        /// </summary>
        /// <param name="usuarioEmpresaPerfilToSave">
        /// Objeto que representa o Perfil de um usuario em uma empresa
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest )]
        public async Task<IActionResult> IncluirEmpresaPerfil([FromBody]UsuarioEmpresaPerfilItem usuarioEmpresaPerfilToSave)
        {
            string msgRule = "";

            _usuarioContext.UsuarioEmpresaPerfilItems.Add(usuarioEmpresaPerfilToSave);

            //Create Integration Event to be published through the Event Bus
            var usuarioEmpresaPerfilInclusaoEvent = new UsuarioEmpresaPerfilInclusaoIE(usuarioEmpresaPerfilToSave.id_Empresa, usuarioEmpresaPerfilToSave.id_Usuario , usuarioEmpresaPerfilToSave.id_Perfil);
            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndUsuarioEmpresaPerfilContextChangesAsync(usuarioEmpresaPerfilInclusaoEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaUsuarioEmpresaPerfilPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioEmpresaPerfilInclusaoEvent);


            return CreatedAtAction(nameof(IncluirEmpresaPerfil), null);
        }

        /// <summary>
        /// Inclui um grupo de perfis para um usuário de uma empresa.
        /// </summary>
        /// <param name="lstUsuarioEmpresaPerfilToSave">
        /// Uma lista de objetos que representa um perfil de um usuario associado a uma empresa.
        /// </param>
        //POST api/v1/[controller]/items/grupo
        [Route("items/grupo")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> IncluirEmpresaPerfilGrupo([FromBody]List<UsuarioEmpresaPerfilItem> lstUsuarioEmpresaPerfilToSave)
        {

            string msgRule = "";

            foreach (UsuarioEmpresaPerfilItem item in lstUsuarioEmpresaPerfilToSave)
            {
                _usuarioContext.UsuarioEmpresaPerfilItems.Add(item);
            }

            //Create Integration Event to be published through the Event Bus
            var usuarioEmpresaPerfilInclusaoGrupoEvent = new UsuarioEmpresaPerfilInclusaoGrupoIE(lstUsuarioEmpresaPerfilToSave);

            try
            { 
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndUsuarioEmpresaPerfilContextChangesAsync(usuarioEmpresaPerfilInclusaoGrupoEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaUsuarioEmpresaPerfilPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioEmpresaPerfilInclusaoGrupoEvent);


            return CreatedAtAction(nameof(IncluirEmpresaPerfilGrupo), null);
        }

        /// <summary>
        /// Exclui um perfil especifico de um usuário de uma empresa.
        /// </summary>
        /// <param name="id_Empresa">
        /// Identificador da empresa.
        /// </param>
        /// <param name="id_Usuario">
        /// Identificador do Usuario.
        /// </param>
        /// <param name="id_Perfil">
        /// Identificador do perfil.
        /// </param>
        //DELETE api/v1/[controller]/items
        [HttpDelete]
        [Route("items/empresa/{id_Empresa}/usuario/{id_Usuario}/perfil/{id_Perfil}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirUsuarioEmpresaPerfil(int id_Empresa, int id_Usuario , int id_Perfil)
        {
            if (id_Empresa < 1 || id_Usuario < 1 || id_Perfil < 1)
            {
                return BadRequest();
            }

            var usuarioEmpresaPerfilToDelete = _usuarioContext.UsuarioEmpresaPerfilItems
                .OfType<UsuarioEmpresaPerfilItem>()
                .SingleOrDefault(c => c.id_Empresa  == id_Empresa && c.id_Usuario == id_Usuario && c.id_Perfil == id_Perfil);

            if (usuarioEmpresaPerfilToDelete is null)
            {
                return BadRequest();
            }
            
            _usuarioContext.UsuarioEmpresaPerfilItems.Remove(usuarioEmpresaPerfilToDelete);

            //Create Integration Event to be published through the Event Bus
            var usuarioEmpresaPerfilExclusaoEvent = new UsuarioEmpresaPerfilExclusaoIE(usuarioEmpresaPerfilToDelete.id_Empresa, usuarioEmpresaPerfilToDelete.id_Perfil, usuarioEmpresaPerfilToDelete.id_Usuario );

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndUsuarioEmpresaPerfilContextChangesAsync(usuarioEmpresaPerfilExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioEmpresaPerfilExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirUsuarioEmpresaPerfil), null);
        }

        /// <summary>
        /// Exclui um grupo de perfis de um usuário de uma empresa.
        /// Nesse grupo de perfis podem existir usuários e empresas diferentes.
        /// </summary>
        /// <param name="lstUsuarioEmpresaPerfilToDelete">
        /// Uma lista de objetos que representa um perfil de um usuario associado a uma empresa.
        /// </param>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirUsuarioEmpresaPerfilGrupo([FromBody]List<UsuarioEmpresaPerfilItem> lstUsuarioEmpresaPerfilToDelete)
        {
            foreach (UsuarioEmpresaPerfilItem item in lstUsuarioEmpresaPerfilToDelete)
            {
                _usuarioContext.UsuarioEmpresaPerfilItems.Remove(item);
            }

            //Create Integration Event to be published through the Event Bus
            var usuarioEmpresaPerfilExclusaoGrupoEvent = new UsuarioEmpresaPerfilExclusaoGrupoIE(lstUsuarioEmpresaPerfilToDelete);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndUsuarioEmpresaPerfilContextChangesAsync(usuarioEmpresaPerfilExclusaoGrupoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioEmpresaPerfilExclusaoGrupoEvent);


            return CreatedAtAction(nameof(ExcluirUsuarioEmpresaPerfilGrupo), null);
        }

        /// <summary>
        /// Exclui todos os perfis de um usuário de uma empresa.
        /// </summary>
        /// <param name="id_Usuario">
        /// Identificador de usuario.
        /// </param>
        //DELETE api/v1/[controller]/items
        [Route("items/{id_Usuario}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirUsuarioEmpresaPerfilTodos(int id_Usuario)
        {
            if ( id_Usuario < 1 )
            {
                return BadRequest();
            }

            IEnumerable<UsuarioEmpresaPerfilItem> query = _usuarioContext.UsuarioEmpresaPerfilItems
                .OfType<UsuarioEmpresaPerfilItem>()
                .Where(c =>c.id_Usuario == id_Usuario);

            if (query.Count<UsuarioEmpresaPerfilItem>() ==0)
            {
                return BadRequest();
            }

            foreach (UsuarioEmpresaPerfilItem item in query)
            {
                _usuarioContext.UsuarioEmpresaPerfilItems.Remove(item);
            }

            //Create Integration Event to be published through the Event Bus
            var usuarioEmpresaPerfilExclusaoTodosEvent = new UsuarioEmpresaPerfilExclusaoTodosIE(query);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndUsuarioEmpresaPerfilContextChangesAsync(usuarioEmpresaPerfilExclusaoTodosEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioEmpresaPerfilExclusaoTodosEvent);


            return CreatedAtAction(nameof(ExcluirUsuarioEmpresaPerfilTodos), null);
        }

        /// <summary>
        /// Consulta os perfis de um usuário.
        /// </summary>
        /// <param name="idUsuario">
        /// Identificador de usuario.
        /// </param>
        [HttpGet]
        [Route("items/usuario/{idUsuario}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarPerfisPorIdUsuarioTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarPerfisPorIdUsuario(int idUsuario)
        {

            List<ConsultarPerfisPorIdUsuarioTO> l_ListUsuarioPerfilTO = new List<ConsultarPerfisPorIdUsuarioTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarPerfisPorIdUsuarioTO> mycache = new Cache<ConsultarPerfisPorIdUsuarioTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListUsuarioPerfilTO = await mycache.GetListAsync(cachePrefix + idUsuario.ToString());
                if (l_ListUsuarioPerfilTO.Count == 0)
                {
                    ConsultarPerfisPorIdUsuarioTO sqlClass = new ConsultarPerfisPorIdUsuarioTO();
                    sqlClass.ConsultarPerfisPorIdUsuarioTOCommand(idUsuario, _settings.ConnectionString, ref l_ListUsuarioPerfilTO);

                    if (l_ListUsuarioPerfilTO.Count > 0)
                    {
                        await mycache.SetListAsync(cachePrefix + idUsuario.ToString(), l_ListUsuarioPerfilTO);
                    }
                }
            }
            else
            {
                ConsultarPerfisPorIdUsuarioTO sqlClass = new ConsultarPerfisPorIdUsuarioTO();
                sqlClass.ConsultarPerfisPorIdUsuarioTOCommand(idUsuario, _settings.ConnectionString, ref l_ListUsuarioPerfilTO);
            }


                return Ok(l_ListUsuarioPerfilTO);

        }

        /// <summary>
        /// Consulta o perfil de um usuário.
        /// </summary>
        /// <param name="idUsuario">
        /// Identificador de usuario.
        /// </param>
        /// <param name="idPerfil">
        /// Identificador de perfil.
        /// </param>
        [HttpGet]
        [Route("items/usuario/{idUsuario}/perfil/{idPerfil}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarPerfisPorIdUsuarioTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarPerfisPorIdUsuarioIdPerfil(int idUsuario, int idPerfil)
        {

            List<ConsultarPerfisPorIdUsuarioTO> l_ListUsuarioPerfilTO = new List<ConsultarPerfisPorIdUsuarioTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarPerfisPorIdUsuarioTO> mycache = new Cache<ConsultarPerfisPorIdUsuarioTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListUsuarioPerfilTO = await mycache.GetListAsync(cachePrefix + idUsuario.ToString() +"@"+ idPerfil.ToString());
                if (l_ListUsuarioPerfilTO.Count == 0)
                {
                    ConsultarPerfisPorIdUsuarioTO sqlClass = new ConsultarPerfisPorIdUsuarioTO();
                    sqlClass.ConsultarPerfisPorIdUsuarioTOCommand(idUsuario, idPerfil, _settings.ConnectionString, ref l_ListUsuarioPerfilTO);

                    if (l_ListUsuarioPerfilTO.Count > 0)
                    {
                        await mycache.SetListAsync(cachePrefix + idUsuario.ToString(), l_ListUsuarioPerfilTO);
                    }
                }
            }
            else
            {
                ConsultarPerfisPorIdUsuarioTO sqlClass = new ConsultarPerfisPorIdUsuarioTO();
                sqlClass.ConsultarPerfisPorIdUsuarioTOCommand(idUsuario, idPerfil, _settings.ConnectionString, ref l_ListUsuarioPerfilTO);
            }


                return Ok(l_ListUsuarioPerfilTO);

        }

        private bool ruleValidaUsuarioEmpresaPerfilPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_USUARIOEMPRESAPERFIL"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["UsuarioEmpresaPerfilPK"];
                return true;
            }

            return false;

        }
    }
}