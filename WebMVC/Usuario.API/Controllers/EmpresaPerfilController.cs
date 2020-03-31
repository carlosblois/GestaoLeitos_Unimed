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
    public class EmpresaPerfilController : ControllerBase
    {

        private const string cachePrefix = "EMPRESAPERFIL#";
        private UsuarioContext _usuarioContext;
        private readonly UsuarioSettings _settings;
        private readonly IUsuarioIntegrationEventService _usuarioIntegrationEventService;
        private readonly IStringLocalizer<EmpresaPerfilController> _localizer;

        public EmpresaPerfilController(UsuarioContext context, IOptionsSnapshot<UsuarioSettings> settings, IUsuarioIntegrationEventService usuarioIntegrationEventService, IStringLocalizer<EmpresaPerfilController> localizer)
        {
            _usuarioContext = context ?? throw new ArgumentNullException(nameof(context));
            _usuarioIntegrationEventService = usuarioIntegrationEventService ?? throw new ArgumentNullException(nameof(usuarioIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Consulta os perfis de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador da empresa
        /// </param>
        [HttpGet]
        [Route("items/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarPerfisPorIdEmpresaTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarPerfisPorIdEmpresa(int idEmpresa)
        {

            List<ConsultarPerfisPorIdEmpresaTO> l_ListEmpresaPerfilTO = new List<ConsultarPerfisPorIdEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarPerfisPorIdEmpresaTO> mycache = new Cache<ConsultarPerfisPorIdEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListEmpresaPerfilTO = await mycache.GetListAsync("ConsultarPerfisPorIdEmpresa_" + cachePrefix + idEmpresa.ToString());
                if (l_ListEmpresaPerfilTO.Count==0)
                {
                    ConsultarPerfisPorIdEmpresaTO sqlClass = new ConsultarPerfisPorIdEmpresaTO();
                    sqlClass.ConsultarPorIdEmpresaTOCommand(idEmpresa,_settings.ConnectionString, ref l_ListEmpresaPerfilTO);

                    if (l_ListEmpresaPerfilTO.Count > 0)
                    {
                        await mycache.SetListAsync ("ConsultarPerfisPorIdEmpresa_" + cachePrefix + idEmpresa.ToString(), l_ListEmpresaPerfilTO);
                    }
                }
            }
            else
            {
                ConsultarPerfisPorIdEmpresaTO sqlClass = new ConsultarPerfisPorIdEmpresaTO();
                sqlClass.ConsultarPorIdEmpresaTOCommand(idEmpresa,_settings.ConnectionString, ref l_ListEmpresaPerfilTO);
            }


             return Ok(l_ListEmpresaPerfilTO);

        }

        /// <summary>
        /// Inclui um perfil para uma determinada empresa.
        /// </summary>
        /// <param name="empresaPerfilToSave">
        /// Objeto que representa um Perfil associado a uma empresa.
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> IncluirEmpresaPerfil([FromBody]EmpresaPerfilItem empresaPerfilToSave)
        {
            string msgRule = "";
            EmpresaPerfilItem objItemEmpresa = _usuarioContext.EmpresaPerfilItems.Where(m => m.id_Empresa == empresaPerfilToSave.id_Empresa && m.id_Perfil == empresaPerfilToSave.id_Perfil).FirstOrDefault();

            if (objItemEmpresa == null)
                _usuarioContext.EmpresaPerfilItems.Add(empresaPerfilToSave);
            else
            {
                objItemEmpresa.cod_Tipo = empresaPerfilToSave.cod_Tipo;
                _usuarioContext.EmpresaPerfilItems.Update(objItemEmpresa);
            }

            //Create Integration Event to be published through the Event Bus
            var empresaPerfilInclusaoEvent = new EmpresaPerfilInclusaoIE(empresaPerfilToSave.id_Empresa, empresaPerfilToSave.id_Perfil);
            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndEmpresaPerfilContextChangesAsync(empresaPerfilInclusaoEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaEmpresaPerfilPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(empresaPerfilInclusaoEvent);


            return CreatedAtAction(nameof(IncluirEmpresaPerfil), null);
        }

        /// <summary>
        /// Inclui um grupo de perfis para uma determinada empresa .
        /// </summary>
        /// <param name="lstEmpresaPerfilToSave">
        /// Uma lista de objetos que representa um Perfil associado a uma empresa.
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items/grupo")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> IncluirEmpresaPerfilGrupo([FromBody]List<EmpresaPerfilItem> lstEmpresaPerfilToSave)
        {
            string msgRule = "";

            foreach (EmpresaPerfilItem item in lstEmpresaPerfilToSave)
            {
                _usuarioContext.EmpresaPerfilItems.Add(item);
            }

            //Create Integration Event to be published through the Event Bus
            var empresaPerfilInclusaoGrupoEvent = new EmpresaPerfilInclusaoGrupoIE(lstEmpresaPerfilToSave);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndEmpresaPerfilContextChangesAsync(empresaPerfilInclusaoGrupoEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaEmpresaPerfilPK(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(empresaPerfilInclusaoGrupoEvent);


            return CreatedAtAction(nameof(IncluirEmpresaPerfilGrupo), null);
        }

        /// <summary>
        /// Exclui um perfil para uma determinada empresa .
        /// </summary>
        /// <param name="id_Empresa">
        /// Identificador da empresa.
        /// </param>
        /// <param name="id_Perfil">
        /// Identificador do perfil.
        /// </param>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirEmpresaPerfil(int id_Empresa, int id_Perfil)
        {
            if (id_Empresa < 1 || id_Perfil < 1)
            {
                return BadRequest();
            }

            var empresaPerfilToDelete = _usuarioContext.EmpresaPerfilItems
                .OfType<EmpresaPerfilItem>()
                .SingleOrDefault(c => c.id_Empresa == id_Empresa && c.id_Perfil == id_Perfil);

            if (empresaPerfilToDelete is null)
            {
                return BadRequest();
            }

            _usuarioContext.EmpresaPerfilItems.Remove(empresaPerfilToDelete);

            //Create Integration Event to be published through the Event Bus
            var empresaPerfilExclusaoEvent = new EmpresaPerfilExclusaoIE(empresaPerfilToDelete.id_Empresa, empresaPerfilToDelete.id_Perfil);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndEmpresaPerfilContextChangesAsync(empresaPerfilExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(empresaPerfilExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirEmpresaPerfil), null);
        }

        private bool ruleValidaEmpresaPerfilPK(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["PK_EMPRESAPERFIL"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["EmpresaPerfilPK"];
                return true;
            }

            return false;

        }
    }
   

}