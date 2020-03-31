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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Administrativo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcessoEmpresaPerfilTSTAController : ControllerBase
    {
        private const string cachePrefix = "ACESSOEMPRESAPERFILTSTA#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<AcessoEmpresaPerfilTSTAController> _localizer;

        public AcessoEmpresaPerfilTSTAController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<AcessoEmpresaPerfilTSTAController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Consulta acessos de uma empresa / perfil.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="idPerfil">
        /// Identificador de perfil.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/perfil/{idPerfil}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfil(int idEmpresa,int idPerfil)
        {
            List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> l_ListAcessoTO = new List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> mycache = new Cache<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcessoTO = await mycache.GetListAsync("ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfil_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                idPerfil.ToString());
                if (l_ListAcessoTO.Count == 0)
                {
                    ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO sqlClass = new ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO();
                    sqlClass.ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTOCommand(idEmpresa,idPerfil, _settings.ConnectionString, ref l_ListAcessoTO);

                    if (l_ListAcessoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfil_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idPerfil.ToString(), l_ListAcessoTO);
                    }
                }
            }
            else
            {
                ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO sqlClass = new ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO();
                sqlClass.ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTOCommand(idEmpresa, idPerfil, _settings.ConnectionString, ref l_ListAcessoTO);
            }


            return Ok(l_ListAcessoTO);

        }


        /// <summary>
        /// Consulta de perfil cujo os acessos sejam para o tipo de situação, tipo de atividade e empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="idTipoSituacaoAcomodacao">
        /// Identificador do tipo de situação da acomodação.
        /// </param>
        /// <param name="idTipoAtividadeAcomodacao">
        /// Identificador de tipo de atividade da acomodação.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tiposituacao/{idTipoSituacaoAcomodacao}/tiposituacao/{idTipoAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividade(int idEmpresa, int idTipoSituacaoAcomodacao, int idTipoAtividadeAcomodacao)
        {
            List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> l_ListAcessoTO = new List<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO> mycache = new Cache<ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcessoTO = await mycache.GetListAsync("ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividade_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                idTipoSituacaoAcomodacao.ToString() + "@" +
                                                                idTipoAtividadeAcomodacao.ToString());
                if (l_ListAcessoTO.Count == 0)
                {
                    ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO sqlClass = new ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO();
                    sqlClass.ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividadeCommand(idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListAcessoTO);

                    if (l_ListAcessoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividade_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idTipoSituacaoAcomodacao.ToString() + "@" +
                                                    idTipoAtividadeAcomodacao.ToString(), l_ListAcessoTO);
                    }
                }
            }
            else
            {
                ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO sqlClass = new ConsultarAcessoEmpresaPerfilTSTAPorIdEmpresaIdPerfilTO();
                sqlClass.ConsultarAcessoEmpresaPerfilTSTAPorTipoSituacaoTipoAtividadeCommand(idEmpresa, idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListAcessoTO);
            }


            return Ok(l_ListAcessoTO);

        }


        /// <summary>
        /// Inclui / atualiza um acesso.
        /// </summary>
        /// <param name="acessoToSave">
        /// Objeto que representa o Acesso
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarAcessoEmpresaPerfilTSTA([FromBody]AcessoEmpresaPerfilTSTAItem acessoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<AcessoEmpresaPerfilTSTAItem>().Any(e => e.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade== acessoToSave.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade))
            {
                _administrativoContext.AcessoEmpresaPerfilTSTAItems.Update(acessoToSave);
            }
            else
            {
                _administrativoContext.AcessoEmpresaPerfilTSTAItems.Add(acessoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var acessoSaveEvent = new AcessoEmpresaPerfilTSTASaveIE(acessoToSave.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade ,
                                                                                    acessoToSave.Id_Perfil ,
                                                                                    acessoToSave.Id_Empresa,
                                                                                    acessoToSave.Id_TipoSituacaoAcomodacao, 
                                                                                    acessoToSave.Id_TipoAtividadeAcomodacao , 
                                                                                    acessoToSave.Cod_Tipo );

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndAcessoEmpresaPerfilTSTAContextChangesAsync(acessoSaveEvent, acessoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaAcessoEmpresaPerfilTipoSituacaoTipoAtividadeUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(acessoSaveEvent);


            return CreatedAtAction(nameof(SalvarAcessoEmpresaPerfilTSTA), acessoToSave.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade);
        }

        private bool ruleValidaAcessoEmpresaPerfilTipoSituacaoTipoAtividadeUnique(string msgErro, ref string msgRetorno)
        {
           
            string msgUK = _localizer["UK_ACESSOEMPRESAPERFILTIPOSITUACAOTIPOATIVIDADE"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["AcessoEmpresaPerfilTipoSituacaoTipoAtividadeUnique"];
                return true;
            }

            return false;

        }

        /// <summary>
        /// Exclui um Acesso.
        /// </summary>
        /// <param name="Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade">
        /// Identificador do acesso
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirAcessoEmpresaPerfilTSTA(int Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade)
        {

            if (Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade < 1)
            {
                return BadRequest();
            }

            var acessoToDelete = _administrativoContext.AcessoEmpresaPerfilTSTAItems
                .OfType<AcessoEmpresaPerfilTSTAItem>()
                .SingleOrDefault(c => c.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade  == Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade);

            if (acessoToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.AcessoEmpresaPerfilTSTAItems.Remove(acessoToDelete);

            //Create Integration Event to be published through the Event Bus
            var AcessoExclusaoEvent = new AcessoEmpresaPerfilTSTAExclusaoIE(acessoToDelete.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndAcessoEmpresaPerfilTSTContextChangesAsync(AcessoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(AcessoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirAcessoEmpresaPerfilTSTA), null);
        }

        /// <summary>
        /// Excluir todos os acessos de uma empresa / perfil.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="idPerfil">
        /// Identificador de perfil.
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items/empresa/{idEmpresa}/perfil/{idPerfil}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirAcessoEmpresaPerfilTSTATodos(int idEmpresa, int idPerfil)
        {

            if (idEmpresa < 1 || idPerfil < 1)
            {
                return BadRequest();
            }

            var acessoToDelete = _administrativoContext.AcessoEmpresaPerfilTSTAItems
                .OfType<AcessoEmpresaPerfilTSTAItem>()
                .Where(c => c.Id_Empresa == idEmpresa && c.Id_Perfil==idPerfil);

            if (acessoToDelete is null)
            {
                //return BadRequest();
                return CreatedAtAction(nameof(ExcluirAcessoEmpresaPerfilTSTATodos), null);
            }

            foreach (AcessoEmpresaPerfilTSTAItem item in acessoToDelete)
            {
                _administrativoContext.AcessoEmpresaPerfilTSTAItems.Remove(item);

                //Create Integration Event to be published through the Event Bus
                var AcessoExclusaoEvent = new AcessoEmpresaPerfilTSTAExclusaoTodosIntegrationEvent(item.Id_Empresa, item.Id_Perfil);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.DeleteEventAndAcessoEmpresaPerfilTSTContextChangesAsync(AcessoExclusaoEvent);

                // Publish through the Event Bus and mark the saved event as published
                await _administrativoIntegrationEventService.PublishThroughEventBusAsync(AcessoExclusaoEvent);

            }



            return CreatedAtAction(nameof(ExcluirAcessoEmpresaPerfilTSTATodos), null);
        }



    }


}