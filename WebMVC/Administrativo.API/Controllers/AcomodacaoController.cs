using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CacheRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Administrativo.API.Infrastructure;
using Administrativo.API.IntegrationEvents;
using Administrativo.API.IntegrationEvents.Events;
using Administrativo.API.Model;
using Microsoft.Extensions.Localization;
using Administrativo.API.TO;
using Microsoft.AspNetCore.Authorization;
using static Administrativo.API.Enum.ExpoEnum;

namespace Administrativo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "ACOMODACAO#";
        private readonly AdministrativoContext _administrativoContext;
        private readonly AdministrativoSettings _settings;
        private readonly IAdministrativoIntegrationEventService _administrativoIntegrationEventService;
        private readonly IStringLocalizer<AcomodacaoController> _localizer;

        public AcomodacaoController(AdministrativoContext context, IOptionsSnapshot<AdministrativoSettings> settings, IAdministrativoIntegrationEventService administrativoIntegrationEventService, IStringLocalizer<AcomodacaoController> localizer)
        {
            _administrativoContext = context ?? throw new ArgumentNullException(nameof(context));
            _administrativoIntegrationEventService = administrativoIntegrationEventService ?? throw new ArgumentNullException(nameof(administrativoIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// Inclui / atualiza uma Acomodação.
        /// </summary>
        /// <param name="acomodacaoToSave">
        /// Objeto que representa a Acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SalvarAcomodacao([FromBody]AcomodacaoItem acomodacaoToSave)
        {
            //AREA DE VALIDACAO
            string msgRule = "";

            if (!ruleValidaCodExternoAcomodacao(acomodacaoToSave.CodExterno_Acomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            if (!ruleValidaNomeAcomodacao(acomodacaoToSave.Nome_Acomodacao, ref msgRule))
            {
                return BadRequest(msgRule);
            }

            //FIM AREA DE VALIDACAO

            if (_administrativoContext.Set<AcomodacaoItem>().Any(e => e.Id_Acomodacao == acomodacaoToSave.Id_Acomodacao))
            {
                _administrativoContext.AcomodacaoItems.Update(acomodacaoToSave);
            }
            else
            {
                _administrativoContext.AcomodacaoItems.Add(acomodacaoToSave);
            }

            //Create Integration Event to be published through the Event Bus
            var acomodacaoSaveEvent = new AcomodacaoSaveIE(acomodacaoToSave.Nome_Acomodacao, acomodacaoToSave.Id_TipoAcomodacao,acomodacaoToSave.Id_Empresa, acomodacaoToSave.Id_Setor,acomodacaoToSave.CodExterno_Acomodacao, acomodacaoToSave.Cod_Isolamento);

            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndAcomodacaoContextChangesAsync(acomodacaoSaveEvent, acomodacaoToSave);
            }
            catch (Exception e)
            {
                //Validações das CONSTRAINTS do BANCO
                if (ruleValidaNomeAcomodacaoUnique(e.Message, ref msgRule))
                {
                    return BadRequest(msgRule);
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(acomodacaoSaveEvent);


            return CreatedAtAction(nameof(SalvarAcomodacao), acomodacaoToSave.Id_Acomodacao);
        }

        /// <summary>
        /// Isolar uma acomodação.
        /// </summary>
        /// <param name="idAcomodacao">
        /// Identificador da Acomodação.
        /// </param>
        /// <param name="isolar">
        /// Indica se é para isolar ou não S/N
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/atividade/isolar/{isolar}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IsolarAcomodacao(int idAcomodacao, Isolar isolar)
        {
            string msgRule = "";

            var acomodacaoToSave = _administrativoContext.AcomodacaoItems 
                        .OfType<AcomodacaoItem>()
                        .SingleOrDefault(e => e.Id_Acomodacao == idAcomodacao);

            if (acomodacaoToSave == null)
            {

                string msgStatus = _localizer["VALIDA_EXISTENCIAACOMODACAO"];
                return BadRequest(msgStatus);
            }

            acomodacaoToSave.Cod_Isolamento = isolar.ToString();

            _administrativoContext.AcomodacaoItems.Update(acomodacaoToSave);

            //Create Integration Event to be published through the Event Bus
            //Create Integration Event to be published through the Event Bus
            var acomodacaoSaveEvent = new AcomodacaoSaveIE(acomodacaoToSave.Nome_Acomodacao, acomodacaoToSave.Id_TipoAcomodacao, acomodacaoToSave.Id_Empresa, acomodacaoToSave.Id_Setor, acomodacaoToSave.CodExterno_Acomodacao, acomodacaoToSave.Cod_Isolamento);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _administrativoIntegrationEventService.SaveEventAndAcomodacaoContextChangesAsync(acomodacaoSaveEvent, acomodacaoToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(acomodacaoSaveEvent);


            return CreatedAtAction(nameof(IsolarAcomodacao), "OK");
        }

        /// <summary>
        /// Exclui uma acomodação.
        /// </summary>
        /// <param name="id_Acomodacao">
        /// Identificador da acomodação
        /// </param>
        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> ExcluirAcomodacao(int id_Acomodacao)
        {

            if (id_Acomodacao < 1)
            {
                return BadRequest();
            }

            var acomodacaoToDelete = _administrativoContext.AcomodacaoItems
                .OfType<AcomodacaoItem>()
                .SingleOrDefault(c => c.Id_Acomodacao == id_Acomodacao);

            if (acomodacaoToDelete is null)
            {
                return BadRequest();
            }

            _administrativoContext.AcomodacaoItems.Remove(acomodacaoToDelete);

            //Create Integration Event to be published through the Event Bus
            var AcomodacaoExclusaoEvent = new AcomodacaoExclusaoIE(acomodacaoToDelete.Id_Acomodacao);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _administrativoIntegrationEventService.DeleteEventAndAcomodacaoContextChangesAsync(AcomodacaoExclusaoEvent);

            // Publish through the Event Bus and mark the saved event as published
            await _administrativoIntegrationEventService.PublishThroughEventBusAsync(AcomodacaoExclusaoEvent);

            return CreatedAtAction(nameof(ExcluirAcomodacao), null);
        }

        /// <summary>
        /// Consulta acomodação de uma empresa e um setor.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="idSetor">
        /// Identificador de setor.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/setor/{idSetor}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoPorIdEmpresa(int idEmpresa, int idSetor)
        {
            List<ConsultarAcomodacaoPorIdEmpresaIdSetorTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoPorIdEmpresaIdSetorTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoPorIdEmpresaIdSetorTO> mycache = new Cache<ConsultarAcomodacaoPorIdEmpresaIdSetorTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoPorIdEmpresa_" + cachePrefix + 
                                                                idEmpresa.ToString() + "@" + 
                                                                idSetor.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoPorIdEmpresaIdSetorTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSetorTO();
                    sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSetorTOCommand(idEmpresa,idSetor, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoPorIdEmpresa_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idSetor.ToString(), l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoPorIdEmpresaIdSetorTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSetorTO();
                sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSetorTOCommand(idEmpresa,idSetor, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }

        /// <summary>
        /// Consulta acomodação de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoPorIdEmpresa(int idEmpresa)
        {
            List<ConsultarAcomodacaoPorIdEmpresaTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoPorIdEmpresaTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoPorIdEmpresaTO> mycache = new Cache<ConsultarAcomodacaoPorIdEmpresaTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoPorIdEmpresa_" + cachePrefix + 
                                                                idEmpresa.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoPorIdEmpresaTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaTO();
                    sqlClass.ConsultarAcomodacaoPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoPorIdEmpresa_" + cachePrefix + 
                                                    idEmpresa.ToString(), l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoPorIdEmpresaTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaTO();
                sqlClass.ConsultarAcomodacaoPorIdEmpresaTOCommand(idEmpresa, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }


        /// <summary>
        /// Consulta acomodação e o detalhamento da situacao e atividade.
        /// </summary>
        /// <param name="IdAcomodacao">
        /// Identificador da Acomodacao.
        /// </param>
        [HttpGet]
        [Route("items/acomodacaodetalhe/{IdAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_(int IdAcomodacao)
        {
            List<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO> mycache = new Cache<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_" + cachePrefix +
                                                                IdAcomodacao.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO sqlClass = new ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO();
                    sqlClass.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTOCommand(IdAcomodacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_" + cachePrefix +
                                                    IdAcomodacao.ToString(), l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO sqlClass = new ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO();
                sqlClass.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTOCommand(IdAcomodacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }

        /// <summary>
        /// Consulta acomodação e o detalhamento da situacao e atividade.
        /// </summary>
        /// <param name="IdAcomodacao">
        /// Identificador da Acomodacao.
        /// </param>
        /// <param name="IdTipoAtividade">
        /// Identificador do tipo de Atividade na Acomodacao.
        /// </param>
        [HttpGet]
        [Route("items/idacomodacao/{IdAcomodacao}/idtipoatividade/{IdTipoAtividade}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_(int IdAcomodacao, int IdTipoAtividade)
        {
            List<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO> mycache = new Cache<ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_" + cachePrefix +
                                                                IdAcomodacao.ToString() + IdTipoAtividade.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTO sqlClass = new ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTO();
                    sqlClass.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTOCommand(IdAcomodacao, IdTipoAtividade, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacao_" + cachePrefix +
                                                    IdAcomodacao.ToString() + IdTipoAtividade.ToString(), l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTO sqlClass = new ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTO();
                sqlClass.ConsultarAcomodacaoDetalheSituacaoAtividadePorIdAcomodacaoIdTipoAtividadeTOCommand(IdAcomodacao, IdTipoAtividade, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }

        [HttpGet]
        [Route("items/empresa/{idEmpresa}/codexterno/{CodExterno}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AcomodacaoItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoPorIdEmpresaPorCodExterno(int idEmpresa, string CodExterno)
        {
            List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoPorIdEmpresaCodExternoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoPorIdEmpresaCodExternoTO> mycache = new Cache<ConsultarAcomodacaoPorIdEmpresaCodExternoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoPorIdEmpresaPorCodExterno_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                CodExterno.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoPorIdEmpresaCodExternoTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaCodExternoTO();
                    sqlClass.ConsultarAcomodacaoPorIdEmpresaCodExternoTOCommand(idEmpresa, CodExterno, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoPorIdEmpresaPorCodExterno_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    CodExterno, l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoPorIdEmpresaCodExternoTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaCodExternoTO();
                sqlClass.ConsultarAcomodacaoPorIdEmpresaCodExternoTOCommand(idEmpresa, CodExterno, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }


        /// <summary>
        /// Consulta acomodação de uma empresa em determinada situação e sem atividade.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="IdSituacao">
        /// Identificador de Situacao da Acomodacao.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/situacaosematividade/{IdSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoPorIdEmpresaPorIdSituacaoSemAtividade(int idEmpresa, int IdSituacao)
        {
            List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO> mycache = new Cache<ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoPorIdEmpresaPorIdSituacaoSemAtividade_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                IdSituacao.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO();
                    sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTOCommand(idEmpresa, IdSituacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoPorIdEmpresaPorIdSituacaoSemAtividade_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    IdSituacao, l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTO();
                sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSituacaoSemAtividadeTOCommand(idEmpresa, IdSituacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }

        /// <summary>
        /// Consulta acomodação de uma empresa em determinada situação.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa.
        /// </param>
        /// <param name="IdSituacao">
        /// Identificador de Situacao da Acomodacao.
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/situacao/{IdSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAcomodacaoPorIdEmpresaPorIdSituacao(int idEmpresa, int IdSituacao)
        {
            List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO> l_ListAcomodacaoTO = new List<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO> mycache = new Cache<ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAcomodacaoTO = await mycache.GetListAsync("ConsultarAcomodacaoPorIdEmpresaPorIdSituacao_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                IdSituacao.ToString());
                if (l_ListAcomodacaoTO.Count == 0)
                {
                    ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO();
                    sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSituacaoTOCommand(idEmpresa, IdSituacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);

                    if (l_ListAcomodacaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAcomodacaoPorIdEmpresaPorIdSituacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    IdSituacao, l_ListAcomodacaoTO);
                    }
                }
            }
            else
            {
                ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO sqlClass = new ConsultarAcomodacaoPorIdEmpresaIdSituacaoTO();
                sqlClass.ConsultarAcomodacaoPorIdEmpresaIdSituacaoTOCommand(idEmpresa, IdSituacao, _settings.ConnectionString, ref l_ListAcomodacaoTO);
            }


            return Ok(l_ListAcomodacaoTO);

        }



        private bool ruleValidaNomeAcomodacaoUnique(string msgErro, ref string msgRetorno)
        {
            string msgUK = _localizer["UK_ACOMODACAOEMPRESASETORNOME"];
            if (msgErro.ToUpper().Contains(msgUK))
            {
                msgRetorno = _localizer["AcomodacaoEmpresaSetorNomeUnique"];
                return true;
            }

            return false;

        }

        private bool ruleValidaCodExternoAcomodacao(string codExterno_Acomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(codExterno_Acomodacao))
            {
                msgRetorno = _localizer["CodExternoRequerido"];
                return false;
            }

            if ((codExterno_Acomodacao.Length < 3) || (codExterno_Acomodacao.Length > 5))
            {
                msgRetorno = _localizer["CodExternoTamanhoInvalido"];
                return false;
            }

            return true;

        }

        private bool ruleValidaNomeAcomodacao(string nome_Acomodacao, ref string msgRetorno)
        {
            if (string.IsNullOrEmpty(nome_Acomodacao))
            {
                msgRetorno = _localizer["NomeRequerido"];
                return false;
            }

            if ((nome_Acomodacao.Length < 3) || (nome_Acomodacao.Length > 50))
            {
                msgRetorno = _localizer["NomeTamanhoInvalido"];
                return false;
            }

            return true;

        }

    }
}