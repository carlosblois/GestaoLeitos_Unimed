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
    public class UsuarioController : ControllerBase
    {

        private const string cachePrefix = "USUARIO#";
        private UsuarioContext _usuarioContext;
        private readonly UsuarioSettings _settings;
        private readonly IUsuarioIntegrationEventService _usuarioIntegrationEventService;
        private readonly IStringLocalizer<UsuarioController> _localizer;

        public UsuarioController(UsuarioContext context, IOptionsSnapshot<UsuarioSettings> settings, IUsuarioIntegrationEventService usuarioIntegrationEventService, IStringLocalizer<UsuarioController> localizer)
        {
            _usuarioContext = context ?? throw new ArgumentNullException(nameof(context));
            _usuarioIntegrationEventService = usuarioIntegrationEventService ?? throw new ArgumentNullException(nameof(usuarioIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Atualiza o Nome do usuario.
        /// </summary>
        /// <param name="id_Usuario">
        /// Identificador do usuario
        /// </param>
        /// <param name="nome_Usuario">
        /// Nome do usuario (Min 5 Max 50)
        /// </param>
        //PUT api/v1/[controller]/items/login
        [Route("items/nome")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AtualizaNomeUsuario(int id_Usuario, string nome_Usuario)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (id_Usuario < 1)
            {
                return BadRequest();
            }

            //if (!ruleValidaLoginUsuario(nome_Usuario, ref msgRule))
            //{
            //    return BadRequest(msgRule);
            //}

            var usuarioToSave = _usuarioContext.Set<UsuarioItem>().SingleOrDefault(c => c.id_Usuario == id_Usuario);

            if (usuarioToSave is null)
            {
                msgRule = _localizer["UsuarioNaoLocalizado"];
                return BadRequest(msgRule);
            }
            //FIM AREA DE VALIDACAO

            usuarioToSave.nome_Usuario = nome_Usuario;

            _usuarioContext.UsuarioItems.Update(usuarioToSave);

            //Create Integration Event to be published through the Event Bus
            var usuarioAtualizaLoginEvent = new UsuarioAtualizaLoginIE(usuarioToSave.id_Usuario, usuarioToSave.login_Usuario);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndUsuarioChangesAsync(usuarioAtualizaLoginEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaLoginUsuarioUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioAtualizaLoginEvent);

            return CreatedAtAction(nameof(AtualizaLoginUsuario), null);
        }

        /// <summary>
        /// Atualiza o Login do usuario.
        /// </summary>
        /// <param name="id_Usuario">
        /// Identificador do usuario
        /// </param>
        /// <param name="login_Usuario">
        /// Login do usuario (Min 5 Max 20)
        /// </param>
        //PUT api/v1/[controller]/items/login
        [Route("items/login")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AtualizaLoginUsuario(int id_Usuario, string login_Usuario)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (id_Usuario < 1)
            {
                return BadRequest();
            }

            if (!ruleValidaLoginUsuario(login_Usuario, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            var usuarioToSave = _usuarioContext.Set<UsuarioItem>().SingleOrDefault(c => c.id_Usuario == id_Usuario);

            if (usuarioToSave is null)
            {
                msgRule = _localizer["UsuarioNaoLocalizado"];
                return BadRequest(msgRule);
            }
            //FIM AREA DE VALIDACAO

            usuarioToSave.login_Usuario = login_Usuario;
      
            _usuarioContext.UsuarioItems.Update(usuarioToSave);

            //Create Integration Event to be published through the Event Bus
            var usuarioAtualizaLoginEvent = new UsuarioAtualizaLoginIE(usuarioToSave.id_Usuario, usuarioToSave.login_Usuario);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndUsuarioChangesAsync(usuarioAtualizaLoginEvent);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaLoginUsuarioUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioAtualizaLoginEvent);

            return CreatedAtAction(nameof(AtualizaLoginUsuario), null);
        }


        /// <summary>
        /// Atualiza a senha do usuario.
        /// </summary>
        /// <param name="id_Usuario">
        /// Identificador do usuario
        /// </param>
        /// <param name="senha_Usuario">
        /// Senha do usuario (Min 5 Max 10)
        /// </param>
        //PUT api/v1/[controller]/items/senha
        [Route("items/senha")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TrocaSenhaUsuario(int  id_Usuario, string senha_Usuario)
        {
            if (id_Usuario < 1)
            {
                return BadRequest();
            }

            var usuarioToSave = _usuarioContext.Set<UsuarioItem>().SingleOrDefault(c => c.id_Usuario == id_Usuario);

            if (usuarioToSave is null)
            {
                string MSG = _localizer["UsuarioNaoLocalizado"];
                return BadRequest(MSG);
            }

            usuarioToSave.senha_Usuario = senha_Usuario;

           _usuarioContext.UsuarioItems.Update(usuarioToSave);
            
            //Create Integration Event to be published through the Event Bus
            var usuarioAtualizaSenhaEvent = new UsuarioAtualizaSenhaIE(usuarioToSave.id_Usuario,  usuarioToSave.senha_Usuario);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.SaveEventAndUsuarioChangesAsync(usuarioAtualizaSenhaEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioAtualizaSenhaEvent);


            return CreatedAtAction(nameof(TrocaSenhaUsuario), null);
        }

        /// <summary>
        /// Inclui um usuario.
        /// Permite a inclusão em conjunto da coleção de perfis do usuario para uma determinada empresa. (opcional)
        /// </summary>
        /// <param name="usuarioToSave">
        /// Objeto que representa o Usuario
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SalvarUsuario([FromBody]UsuarioItem usuarioToSave)
        {

            string msgRule = "";
            UsuarioItem objLstUsuario = new UsuarioItem();

            objLstUsuario = _usuarioContext.UsuarioItems.Where(m => m.id_Usuario == usuarioToSave.id_Usuario).FirstOrDefault();

            if (objLstUsuario != null)
            {
                objLstUsuario.id_Usuario = usuarioToSave.id_Usuario;
                objLstUsuario.Ativo = usuarioToSave.Ativo;
                objLstUsuario.login_Usuario = usuarioToSave.login_Usuario;
                objLstUsuario.nome_Usuario = usuarioToSave.nome_Usuario;
                objLstUsuario.senha_Usuario = usuarioToSave.senha_Usuario;
                objLstUsuario.UsuarioEmpresaPerfilItems = usuarioToSave.UsuarioEmpresaPerfilItems;
                IEnumerable<UsuarioEmpresaPerfilItem> query = _usuarioContext.UsuarioEmpresaPerfilItems
                                                            .OfType<UsuarioEmpresaPerfilItem>()
                                                             .Where(c => c.id_Usuario == usuarioToSave.id_Usuario);

                foreach (UsuarioEmpresaPerfilItem item in query)
                {
                    _usuarioContext.UsuarioEmpresaPerfilItems.Remove(item);
                }

                _usuarioContext.UsuarioItems.Update(objLstUsuario);
            }
            else
            {
                _usuarioContext.UsuarioItems.Add(usuarioToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var usuarioInclusaoEvent = new UsuarioInclusaoIE(usuarioToSave.id_Usuario, usuarioToSave.nome_Usuario, usuarioToSave.login_Usuario,usuarioToSave.senha_Usuario);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _usuarioIntegrationEventService.SaveEventAndUsuarioContextChangesAsync(usuarioInclusaoEvent, usuarioToSave);
            }
            catch (Exception e)
            {
                //ruleValidaNomeUsuarioUnique(e.Message, ref msgRule))
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaLoginUsuarioUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioInclusaoEvent);

            return CreatedAtAction(nameof(SalvarUsuario), usuarioToSave.id_Usuario);
        }

        /// <summary>
        /// Exclui um usuario.
        /// </summary>
        /// <param name="id_Usuario">
        /// Identificador do Usuario
        /// </param>
        //DELETE api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExcluirUsuario(int id_Usuario)
        {
            if (id_Usuario < 1)
            {
                return BadRequest();
            }


            var usuarioToDelete = _usuarioContext.UsuarioItems.Include(b => b.UsuarioEmpresaPerfilItems)
                            .OfType<UsuarioItem>()
                            .SingleOrDefault(c => c.id_Usuario == id_Usuario);

            if (usuarioToDelete is null)
            {
                return BadRequest();
            }

            usuarioToDelete.Ativo = 0;

            // _usuarioContext.UsuarioItems.Remove(usuarioToDelete);
            _usuarioContext.UsuarioItems.Update(usuarioToDelete);

            //Create Integration Event to be published through the Event Bus
            var usuarioExclusaoEvent = new UsuarioExclusaoIE(usuarioToDelete.id_Usuario);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _usuarioIntegrationEventService.DeleteEventAndUsuarioContextChangesAsync(usuarioExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _usuarioIntegrationEventService.PublishThroughEventBusAsync(usuarioExclusaoEvent);


            return CreatedAtAction(nameof(ExcluirUsuario), null);
        }

        /// <summary>
        /// Valida o Login do usuario.
        /// </summary>
        /// <param name="login_Usuario">
        /// Login do Usuario
        /// </param>
        /// <param name="senha_Usuario">
        /// Senha do Usuario
        /// </param>
        [HttpGet]
        [Route("items/")]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<ValidarLoginUsuarioTO>), (int)HttpStatusCode.OK)]
        public IActionResult ValidarLoginUsuario(string login_Usuario, string senha_Usuario)
        {
            List<ValidarLoginUsuarioTO> l_ListValidarLoginUsuarioTO = new List<ValidarLoginUsuarioTO>();

            ValidarLoginUsuarioTO sqlClass = new ValidarLoginUsuarioTO();
            sqlClass.ValidarLoginUsuarioTOCommand(login_Usuario, senha_Usuario, _settings.ConnectionString, ref l_ListValidarLoginUsuarioTO);

            string MSG = "";
            if (l_ListValidarLoginUsuarioTO.Count > 0)
            {
                if (l_ListValidarLoginUsuarioTO[0].Ativo == 1)
                {
                    LogAcessoItem lgit = new LogAcessoItem();
                    lgit.id_Usuario = l_ListValidarLoginUsuarioTO[0].Id_Usuario;
                    lgit.dt_Entrada = DateTime.Now;
                    lgit.cod_sucesso = "S";
                    _usuarioContext.LogAcessoItems.Add(lgit);

                    return Ok(l_ListValidarLoginUsuarioTO);
                }
                else
                {
                    MSG = _localizer["UsuarioInativo"];
                    return BadRequest(MSG);
                }
            }           

            MSG = _localizer["UsuarioInvalido"];
            return BadRequest(MSG);
        }

        /// <summary>
        /// TELA LOGIN (código: 01) ** Valida o Login do usuario e retorna as empresas e perfis associados.
        /// </summary>
        /// <param name="login_Usuario">
        /// Login do Usuario
        /// </param>
        /// <param name="senha_Usuario">
        /// Senha do Usuario
        /// </param>
        [HttpGet]
        [Route("items/perfil")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<LogarUsuarioEmpresaPerfilTO>), (int)HttpStatusCode.OK)]
        public IActionResult LogarUsuarioEmpresaPerfil(string login_Usuario, string senha_Usuario)
        {
            List<LogarUsuarioEmpresaPerfilTO> l_ListLogarUsuarioEmpresaPerfilTO = new List<LogarUsuarioEmpresaPerfilTO>();


            LogarUsuarioEmpresaPerfilTO sqlClass = new LogarUsuarioEmpresaPerfilTO();
            sqlClass.LogarUsuarioEmpresaPerfilTOCommand(login_Usuario, senha_Usuario, _settings.ConnectionString, ref l_ListLogarUsuarioEmpresaPerfilTO);

            string MSG = "";

            if (l_ListLogarUsuarioEmpresaPerfilTO.Count > 0)
            {

                if (l_ListLogarUsuarioEmpresaPerfilTO[0].Ativo == 1)
                {
                    LogAcessoItem lg = new LogAcessoItem();
                    lg.id_Usuario = l_ListLogarUsuarioEmpresaPerfilTO[0].Id_Usuario;
                    lg.dt_Entrada = DateTime.Now;
                    lg.cod_sucesso = "S";
                    _usuarioContext.LogAcessoItems.Add(lg);
                    _usuarioContext.SaveChanges();
                    return Ok(l_ListLogarUsuarioEmpresaPerfilTO);
                }
                else
                {
                    MSG = _localizer["UsuarioInativo"];
                    return BadRequest(MSG); 
                }
            }

            MSG = _localizer["UsuarioInvalido"];
            return BadRequest(MSG);
        }

        /// <summary>
        /// Lista de usuario de uma determinada empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador da Empresa
        /// </param>
        [HttpGet]
        [Route("items/empresa")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<LogarUsuarioEmpresaPerfilTO>), (int)HttpStatusCode.OK)]
        public IActionResult ConsultarUsuarioPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarUsuarioPorIdEmpresaTO> l_ListUsuarioEmpresaTO = new List<ConsultarUsuarioPorIdEmpresaTO>();


            ConsultarUsuarioPorIdEmpresaTO sqlClass = new ConsultarUsuarioPorIdEmpresaTO();
            sqlClass.ConsultarUsuarioPorIdEmpresaTOCommand(idEmpresa,  _settings.ConnectionString, ref l_ListUsuarioEmpresaTO);


            if (l_ListUsuarioEmpresaTO.Count > 0)
            {
                return Ok(l_ListUsuarioEmpresaTO);
            }

            string MSG = _localizer["ListaUsuarioVazia"];
            return Ok(MSG);
        }


        private bool ruleValidaNomeUsuario(string Nome_Usuario, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(Nome_Usuario ))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((Nome_Usuario.Length < 5) || (Nome_Usuario.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaLoginUsuario(string Login_Usuario, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(Login_Usuario))
            {
                msgRetorno = _localizer["LoginRequerido"];
                return false;
            }

            if ((Login_Usuario.Length < 5) || (Login_Usuario.Length > 20))
            {
                msgRetorno =_localizer["LoginTamanhoInvalido"];
                return false;
            }
       
            return true;

        }
        private bool ruleValidaLoginUsuarioUnique( string msgErro,ref string msgRetorno)
        {
            string msgUK = _localizer["UK_LOGINUSUARIO"];
            if (msgErro.ToUpper().Contains(msgUK) )
            {
                msgRetorno = _localizer["UsuarioLoginUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaNomeUsuarioUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_USUARIONOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["UsuarioNomeUnique"];
                return true;
            }

            return false;

        }

    }



}