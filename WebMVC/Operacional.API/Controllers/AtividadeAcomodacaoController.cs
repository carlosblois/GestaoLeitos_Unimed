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
using Administrativo.API.TO;

namespace Operacional.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AtividadeAcomodacaoController : ControllerBase
    {
        private const string cachePrefix = "ATIVIDADE#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<AtividadeAcomodacaoController> _localizer;

        public AtividadeAcomodacaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<AtividadeAcomodacaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }


        /// <summary>
        /// Consulta  TODAS as atividades de um usuario nao finalizada de uma empresa por Perfil.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario
        /// </param>
        /// <param name="idPerfil">
        /// Identificador de Perfil
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/usuario/{idUsuario}/perfil/{idPerfil}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeTodasTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesTODASNaoFinalizadasPorEmpresaPorUsuario(int idEmpresa, int idUsuario, int idPerfil)
        {

            List<ConsultarAtividadeTodasTO> l_ListAtividadeTO = new List<ConsultarAtividadeTodasTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeTodasTO> mycache = new Cache<ConsultarAtividadeTodasTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync("ConsultarAtividadesTODASNaoFinalizadasPorEmpresaPorUsuario_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                idUsuario.ToString() + "@" +
                                                                idPerfil.ToString() );
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarAtividadeTodasTO sqlClass = new ConsultarAtividadeTodasTO();
                    sqlClass.ConsultarAtividadeTodasTOCommand(idEmpresa, idUsuario, idPerfil, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesTODASNaoFinalizadasPorEmpresaPorUsuario_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idUsuario.ToString() + "@" +
                                                    idPerfil.ToString(), l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeTodasTO sqlClass = new ConsultarAtividadeTodasTO();
                sqlClass.ConsultarAtividadeTodasTOCommand(idEmpresa, idUsuario, idPerfil, _settings.ConnectionString, ref l_ListAtividadeTO);
            }


                return Ok(l_ListAtividadeTO);

        }


        /// <summary>
        /// Consulta os itens de Checklist cujas as respostas geraram atividade atual.
        /// </summary>
        /// <param name="idAtividadeAcomodacao">
        /// Identificador da Atividade
        /// </param>
        [HttpGet]
        [Route("items/Checklist/atividade/{idAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarCheckListAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarCheckListAtividadeAnterior(int idAtividadeAcomodacao)
        {

            List<ConsultarCheckListAtividadeTO> l_ListAtividadeTO = new List<ConsultarCheckListAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarCheckListAtividadeTO> mycache = new Cache<ConsultarCheckListAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync("ConsultarCheckListAtividadeAnterior_" + cachePrefix +
                                                                idAtividadeAcomodacao.ToString());
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarCheckListAtividadeTO sqlClass = new ConsultarCheckListAtividadeTO();
                    sqlClass.ConsultarCheckListAtividadeTOCommand(idAtividadeAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarCheckListAtividadeAnterior_" + cachePrefix +
                                                    idAtividadeAcomodacao.ToString(), l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarCheckListAtividadeTO sqlClass = new ConsultarCheckListAtividadeTO();
                sqlClass.ConsultarCheckListAtividadeTOCommand(idAtividadeAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);
            }


            return Ok(l_ListAtividadeTO);

        }



        /// <summary>
        /// Consulta  as atividades de um usuario nao finalizada de uma empresa.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de usuario
        /// </param>
        /// <param name="idSetor">
        /// Identificador de setor
        /// </param>
        /// <param name="idCaracteristicaAcomodacao">
        /// Identificador de Caracteristica
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/usuario/{idUsuario}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuario(int idEmpresa, int idUsuario, int idSetor, int idCaracteristicaAcomodacao)
        {
            
            List<ConsultarAtividadeTO> l_ListAtividadeTO = new List<ConsultarAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeTO> mycache = new Cache<ConsultarAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuario_"+ cachePrefix + 
                                                                idEmpresa.ToString() + "@" + 
                                                                idUsuario.ToString() + "@" +
                                                                idSetor.ToString() + "@" +
                                                                idCaracteristicaAcomodacao.ToString());
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                    sqlClass.ConsultarAtividadeNaoFinalizadaPorUsuarioTOCommand(idEmpresa, idUsuario, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuario_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idUsuario.ToString() + "@" + 
                                                    idSetor.ToString() + "@" +
                                                    idCaracteristicaAcomodacao.ToString() , l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                sqlClass.ConsultarAtividadeNaoFinalizadaPorUsuarioTOCommand(idEmpresa, idUsuario, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);
            }


                return Ok(l_ListAtividadeTO);

        }

        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Consulta  as atividades de uma empresa de um tipo de atividade, nao finalizada de uma empresa associados a uma Situacao/Acomodacao.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoAtividadeAcomodacao">
        /// Identificador de Tipo de Atividade
        /// </param>
        /// <param name="idSetor">
        /// Identificador de setor
        /// </param>
        /// <param name="idCaracteristicaAcomodacao">
        /// Identificador de Caracteristica
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/tipoatividade/{idTipoAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesNaoFinalizadasPorEmpresa(int idEmpresa, int idTipoAtividadeAcomodacao, int idSetor, int idCaracteristicaAcomodacao)
        {

            List<ConsultarAtividadeTO> l_ListAtividadeTO = new List<ConsultarAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeTO> mycache = new Cache<ConsultarAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresa_" + cachePrefix + 
                                                                idEmpresa.ToString() + "@" + 
                                                                idTipoAtividadeAcomodacao.ToString() + "@" + 
                                                                idSetor.ToString() + "@" +
                                                                idCaracteristicaAcomodacao.ToString());
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                    sqlClass.ConsultarAtividadeNaoFinalizadaTOCommand(idEmpresa, idTipoAtividadeAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresa_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idTipoAtividadeAcomodacao.ToString() + "@" + 
                                                    idSetor.ToString() + "@" +
                                                    idCaracteristicaAcomodacao.ToString() , l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                sqlClass.ConsultarAtividadeNaoFinalizadaTOCommand(idEmpresa, idTipoAtividadeAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);
            }


                return Ok(l_ListAtividadeTO);

        }


        /// <summary>
        /// Consulta  as atividades de uma empresa de um usuário e um tipo de ação, nao finalizada.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de Usuario
        /// </param>
        /// <param name="idTipoAcaoAcomodacao">
        /// Identificador do tipo de ação
        ///--5-> SOLICITOU MAS NAO ACEITOU
        ///--1-> ACEITOU MAS NAO INICIOU
        ///--2-> INICIOU MAS NAO FINALIZOU
        /// </param>
        /// <param name="idSetor">
        /// Identificador de setor
        /// </param>
        /// <param name="idCaracteristicaAcomodacao">
        /// Identificador de Caracteristica
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/usuario/{idUsuario}/tipoacao/{idTipoAcaoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuarioPorTipoAcao(int idEmpresa, int idUsuario, int idTipoAcaoAcomodacao, int idSetor, int idCaracteristicaAcomodacao)
        {

            List<ConsultarAtividadeTO> l_ListAtividadeTO = new List<ConsultarAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeTO> mycache = new Cache<ConsultarAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeTO = await mycache.GetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuarioPorTipoAcao_" + cachePrefix +
                                                                idEmpresa.ToString() + "@" +
                                                                idUsuario.ToString() + "@" +
                                                                idTipoAcaoAcomodacao.ToString() + "@" +
                                                                idSetor.ToString() + "@" +
                                                                idCaracteristicaAcomodacao.ToString());
                if (l_ListAtividadeTO.Count == 0)
                {
                    ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                    sqlClass.ConsultarAtividadeAcaoNaoFinalizadaPorUsuarioPorTipoAcaoTOCommand(idEmpresa, idUsuario, idTipoAcaoAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);

                    if (l_ListAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesNaoFinalizadasPorEmpresaPorUsuarioPorTipoAcao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idUsuario.ToString() + "@" +
                                                    idTipoAcaoAcomodacao.ToString() + "@" +
                                                    idSetor.ToString() + "@" +
                                                    idCaracteristicaAcomodacao.ToString(), l_ListAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeTO sqlClass = new ConsultarAtividadeTO();
                sqlClass.ConsultarAtividadeAcaoNaoFinalizadaPorUsuarioPorTipoAcaoTOCommand(idEmpresa, idUsuario, idTipoAcaoAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListAtividadeTO);
            }


            return Ok(l_ListAtividadeTO);

        }





        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Consulta o detalhamento de acoes de uma atividade de uma situacao (Situacao/Acomodacao).
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idSituacao">
        /// Identificador de situacao
        /// </param>
        [HttpGet]
        [Route("items/empresa/{idEmpresa}/situacao/{idSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeAcaoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesAcoesPorEmpresaPorSituacao(int idEmpresa, int idSituacao)
        {
            List<ConsultarAtividadeAcaoTO> l_ListAtividadeAcaoTO = new List<ConsultarAtividadeAcaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeAcaoTO> mycache = new Cache<ConsultarAtividadeAcaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListAtividadeAcaoTO = await mycache.GetListAsync("ConsultarAtividadesAcoesPorEmpresaPorSituacao_"+cachePrefix + 
                                                                    idEmpresa.ToString() + "@" + 
                                                                    idSituacao.ToString());
                if (l_ListAtividadeAcaoTO.Count == 0)
                {
                    ConsultarAtividadeAcaoTO sqlClass = new ConsultarAtividadeAcaoTO();
                    sqlClass.ConsultarAtividadesAcaoTOCommand(idEmpresa, idSituacao,   _settings.ConnectionString, ref l_ListAtividadeAcaoTO);

                    if (l_ListAtividadeAcaoTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesAcoesPorEmpresaPorSituacao_"+cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idSituacao.ToString() , l_ListAtividadeAcaoTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeAcaoTO sqlClass = new ConsultarAtividadeAcaoTO();
                sqlClass.ConsultarAtividadesAcaoTOCommand(idEmpresa, idSituacao,   _settings.ConnectionString, ref l_ListAtividadeAcaoTO);
            }


                return Ok(l_ListAtividadeAcaoTO);

        }

        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Consulta o detalhamento de acoes de uma atividade de uma situacao (Situacao/Acomodacao).
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idSituacao">
        /// Identificador de situacao
        /// </param>
        [HttpGet]
        [Route("items/det/empresa/{idEmpresa}/situacao/{idSituacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarAtividadeAcoesDetTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarAtividadesAcoesDetPorEmpresaPorSituacao(int idEmpresa, int idSituacao)
        {


            List<ConsultarAtividadeAcoesDetTO> l_ListTO = new List<ConsultarAtividadeAcoesDetTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarAtividadeAcoesDetTO> mycache = new Cache<ConsultarAtividadeAcoesDetTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarAtividadesAcoesPorEmpresaPorSituacao_" + cachePrefix +
                                                                    idEmpresa.ToString() + "@" +
                                                                    idSituacao.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarAtividadeAcoesDetTO sqlClass = new ConsultarAtividadeAcoesDetTO();
                    sqlClass.ConsultarAtividadesAcoesDetTOCommand(idEmpresa, idSituacao, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarAtividadesAcoesPorEmpresaPorSituacao_" + cachePrefix +
                                                    idEmpresa.ToString() + "@" +
                                                    idSituacao.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarAtividadeAcoesDetTO sqlClass = new ConsultarAtividadeAcoesDetTO();
                sqlClass.ConsultarAtividadesAcoesDetTOCommand(idEmpresa, idSituacao, _settings.ConnectionString, ref l_ListTO);
            }


                return Ok(l_ListTO);

        }



        /// <summary>
        /// TELA DE ATIVIDADES (código: 03) Consulta  as QTD de atividades de uma empresa especificamente de um tipo de atividade, nao finalizadas.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idTipoAtividadeAcomodacao">
        /// Identificador de Tipo de Atividade
        /// </param>
        /// <param name="idSetor">
        /// Identificador de setor
        /// </param>
        /// <param name="idCaracteristicaAcomodacao">
        /// Identificador de Caracteristica
        /// </param>
        [HttpGet]
        [Route("items/QTD/empresa/{idEmpresa}/tipoatividade/{idTipoAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarQTDAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult>ConsultarQTDAtividadesPorEmpresaPorTipoAtividade(int idEmpresa, int idTipoAtividadeAcomodacao, int idSetor, int idCaracteristicaAcomodacao)
        {

            List<ConsultarQTDAtividadeTO> l_ListQTDAtividadeTO = new List<ConsultarQTDAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarQTDAtividadeTO> mycache = new Cache<ConsultarQTDAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListQTDAtividadeTO = await mycache.GetListAsync("ConsultarQTDAtividadesPorEmpresaPorTipoAtividade_" + cachePrefix + 
                                                                    idEmpresa.ToString() + "@" + 
                                                                    idTipoAtividadeAcomodacao.ToString() + "@" + 
                                                                    idSetor.ToString() + "@" +
                                                                    idCaracteristicaAcomodacao.ToString());
                if (l_ListQTDAtividadeTO.Count == 0)
                {
                    ConsultarQTDAtividadeTO sqlClass = new ConsultarQTDAtividadeTO();
                    sqlClass.ConsultarQTDAtividadeTOCommand(idEmpresa, idTipoAtividadeAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListQTDAtividadeTO);

                    if (l_ListQTDAtividadeTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarQTDAtividadesPorEmpresaPorTipoAtividade_" + cachePrefix + 
                                                    idEmpresa.ToString() + "@" + 
                                                    idTipoAtividadeAcomodacao.ToString() + "@" + 
                                                    idSetor.ToString() + "@" +
                                                    idCaracteristicaAcomodacao.ToString() , l_ListQTDAtividadeTO);
                    }
                }
            }
            else
            {
                ConsultarQTDAtividadeTO sqlClass = new ConsultarQTDAtividadeTO();
                sqlClass.ConsultarQTDAtividadeTOCommand(idEmpresa, idTipoAtividadeAcomodacao, idSetor, idCaracteristicaAcomodacao, _settings.ConnectionString, ref l_ListQTDAtividadeTO);
            }


                return Ok(l_ListQTDAtividadeTO);

        }


        /// <summary>
        /// ENCAMINHAR LEITOS (código: 04) Encaminha para a execução de outras atividades.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de empresa
        /// </param>
        /// <param name="idUsuario">
        /// Identificador de Usuario
        /// </param>
        /// <param name="IdAtividadeAcomodacao">
        /// Identificador da Atividade Origem
        /// </param>
        /// <param name="LstTipoAtividadeToSave">
        /// Lista de Tipos de Atividades a serem encaminhados
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> EncaminharAtividade(int idEmpresa,int idUsuario, int IdAtividadeAcomodacao,List<TipoAtividade> LstTipoAtividadeToSave)
        {

            if ((idUsuario < 1)|| (IdAtividadeAcomodacao < 1))
            {
                return BadRequest();
            }

            var atividadeToOrigemEncaminhar = _operacionalContext.AtividadeItems
                .OfType<AtividadeItem>()
                .SingleOrDefault(c => c.Id_AtividadeAcomodacao == IdAtividadeAcomodacao && c.dt_FimAtividadeAcomodacao == null);

            if (atividadeToOrigemEncaminhar is null)
            {
                return BadRequest();
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == atividadeToOrigemEncaminhar.Id_SituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;

            //ENCAMINHAR
            EncaminharOut encaminharToSave = await Encaminhar(ConfiguracaoURL, AdministrativoURL, tokenURL, idUsuario, idEmpresa, atividadeToOrigemEncaminhar, LstTipoAtividadeToSave, idTipoAcomodacao, idAcomodacao);


            List<IntegrationEvent> lstEvt = new List<IntegrationEvent>();
            List<AtividadeItem> lstItem = new List<AtividadeItem>();

            lstEvt = encaminharToSave.lstEvt;
            lstItem = encaminharToSave.lstItem;

            //LISTA DE ATIVIDADES EXISTENTES
            IEnumerable<AtividadeItem> query = _operacionalContext.AtividadeItems
            .OfType<AtividadeItem>()
            .Where(c => c.Id_SituacaoAcomodacao == atividadeToOrigemEncaminhar.Id_SituacaoAcomodacao && c.dt_FimAtividadeAcomodacao == null);
            List<AtividadeItem> atividadeAcomodacaoExistentes = query.ToList();


            List<int> lstInt = new List<int>();
            foreach (AtividadeItem Atividade in encaminharToSave.lstItem)
            {
                var res = atividadeAcomodacaoExistentes.Find(item => item.Id_TipoAtividadeAcomodacao == Atividade.Id_TipoAtividadeAcomodacao);
                if (res == null)
                {
                    _operacionalContext.AtividadeItems.Add(Atividade);

                }
                else
                {
                    int myindex = lstItem.IndexOf(Atividade);
                    lstInt.Add(myindex);
                }
            }

            var sortedList = lstInt.OrderByDescending(x=>x).ToList();
            foreach (int i in sortedList)
            {
                lstItem.RemoveAt(i);
                lstEvt.RemoveAt(i);
            }



            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAtividadeContextChangesAsync(lstEvt, lstItem);
            }
            catch (Exception e)
            {
                    return BadRequest(e.Message);
            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(lstEvt);


            return CreatedAtAction(nameof(EncaminharAtividade), "Ok");
        }

        /// <summary>
        /// Inclui uma atividade.
        /// </summary>
        /// <param name="atividadeToSave">
        /// Objeto que representa a atividade da acomodação
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/atividade")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IncluirAtividade(int idEmpresa,[FromBody]AtividadeItem atividadeToSave)
        {
            string msgRule = "";

            if ((idEmpresa < 1))
            {
                return BadRequest();
            }

            var atividadeToValidate = _operacionalContext.AtividadeItems
                        .OfType<AtividadeItem>()
                        .SingleOrDefault(e => e.Id_SituacaoAcomodacao == atividadeToSave.Id_SituacaoAcomodacao && e.Id_TipoAtividadeAcomodacao == atividadeToSave.Id_TipoAtividadeAcomodacao && e.dt_FimAtividadeAcomodacao == null);

            if (atividadeToValidate != null)
            {

                    string msgStatus = _localizer["VALIDA_SITUACAOATIVA"];
                    return BadRequest(msgStatus);           
            }

            if (atividadeToSave.Cod_Plus == "S" && atividadeToSave.Id_TipoAtividadeAcomodacao != (int)TipoAtividade.HIGIENIZAÇÃO )
            {

                string msgStatus = _localizer["VALIDA_ATIVIDADEHIGIENIZACAO"];
                return BadRequest(msgStatus);
            }

            string AdministrativoURL = this._settings.AdministrativoURL;
            string tokenURL = this._settings.TokenURL;

            var situacaoAcomodacaoToConsultar = _operacionalContext.SituacaoItems
                .OfType<SituacaoItem>()
                .SingleOrDefault(c => c.Id_SituacaoAcomodacao == atividadeToSave.Id_SituacaoAcomodacao);

            int idAcomodacao = situacaoAcomodacaoToConsultar.Id_Acomodacao;

            List<ConsultarTipoAcomodacaoPorIdAcomodacaoTO> tipoAcomodacaoToView = await ConsultaTipoAcomodacaoPorIdAcomodacaoAsync(AdministrativoURL, tokenURL, idEmpresa, idAcomodacao);

            int idTipoAcomodacao = tipoAcomodacaoToView[0].Id_TipoAcomodacao;

            string ConfiguracaoURL = this._settings.ConfiguracaoURL;


            List<Configuracao.API.TO.ConsultarSLATO> SLAItem = await ConsultaSLAAsync(ConfiguracaoURL,
                                                                        tokenURL,
                                                                        idEmpresa,
                                                                        atividadeToSave.Id_TipoSituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoAtividadeAcomodacao,
                                                                        (int)TipoAcao.SOLICITAR, idTipoAcomodacao);

            AcaoItem acaoToSave = new AcaoItem();
            acaoToSave.Id_TipoAcaoAcomodacao = (int)TipoAcao.SOLICITAR ;
            acaoToSave.dt_InicioAcaoAtividade = DateTime.Now;
            acaoToSave.Id_UsuarioExecutor = atividadeToSave.Id_UsuarioSolicitante.ToString ();
            if ((SLAItem != null) && (SLAItem.Count > 0)) { acaoToSave.Id_SLA = SLAItem[0].Id_SLA; }

            List<AcaoItem> lst = new List<AcaoItem>();
            atividadeToSave.AcaoItems = lst;
            atividadeToSave.AcaoItems.Add(acaoToSave);

            _operacionalContext.AtividadeItems.Add(atividadeToSave);


            List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, idEmpresa, atividadeToSave.Id_TipoSituacaoAcomodacao, atividadeToSave.Id_TipoAtividadeAcomodacao);

            //Create Integration Event to be published through the Event Bus
            var atividadeSaveEvent = new AtividadeSaveIE(atividadeToSave.Id_AtividadeAcomodacao,
                                                                        atividadeToSave.Id_SituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoSituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoAtividadeAcomodacao,
                                                                        atividadeToSave.dt_InicioAtividadeAcomodacao,
                                                                        atividadeToSave.dt_FimAtividadeAcomodacao,
                                                                        atividadeToSave.Id_UsuarioSolicitante,
                                                                        atividadeToSave.AcaoItems,
                                                                        atividadeToSave.Cod_Prioritario,
                                                                        atividadeToSave.Cod_Plus,
                                                                        Perfis,
                                                                        idAcomodacao);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAtividadeContextChangesAsync(atividadeSaveEvent, atividadeToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(atividadeSaveEvent);


            return CreatedAtAction(nameof(IncluirAtividade), atividadeToSave.Id_AtividadeAcomodacao);
        }

        /// <summary>
        /// Prioriza uma atividade.
        /// </summary>
        /// <param name="idAtividade">
        /// Identificador da Atividade da Acomodação.
        /// </param>
        /// <param name="prioridade">
        /// Indica se é para priorizar ou não S/N
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/atividade/prioridade/{prioridade}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> PriorizarAtividade(int idAtividade ,Priorizar prioridade)
        {
            string msgRule = "";

            var atividadeToSave = _operacionalContext.AtividadeItems
                        .OfType<AtividadeItem>()
                        .SingleOrDefault(e => e.Id_AtividadeAcomodacao == idAtividade);

            if (atividadeToSave == null)
            {

                string msgStatus = _localizer["VALIDA_EXISTENCIAATIVIDADE"];
                return BadRequest(msgStatus);
            }

            var situacaoToExit = _operacionalContext.SituacaoItems
                       .OfType<SituacaoItem>()
                       .SingleOrDefault(e => e.Id_SituacaoAcomodacao == atividadeToSave.Id_SituacaoAcomodacao);

            if (situacaoToExit == null)
            {

                string msgStatus = _localizer["VALIDA_EXISTENCIAATIVIDADE"];
                return BadRequest(msgStatus);
            }


            atividadeToSave.Cod_Prioritario = prioridade.ToString();

            _operacionalContext.AtividadeItems.Update(atividadeToSave);

            string AdministrativoURL = _settings.AdministrativoURL;
            string tokenURL = _settings.TokenURL;

            List<Administrativo.API.TO.ConsultarAcomodacaoDetalhePorIdAcomodacaoTO> AcomodacaoItem = new List<ConsultarAcomodacaoDetalhePorIdAcomodacaoTO>();
            AcomodacaoItem = await ConsultaAcomodacaoDetalhePorIdAcomodacaoAsync(AdministrativoURL, tokenURL, situacaoToExit.Id_Acomodacao);

            List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = new List<ConsultarAcessoAtividadeEmpresaPerfilTO>();
            Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, AcomodacaoItem[0].Id_Empresa, atividadeToSave.Id_TipoSituacaoAcomodacao, atividadeToSave.Id_TipoAtividadeAcomodacao);

            //Create Integration Event to be published through the Event Bus
            var atividadeSaveEvent = new AtividadePriorizadaIE(atividadeToSave.Id_AtividadeAcomodacao,
                                                        atividadeToSave.Id_SituacaoAcomodacao,
                                                        atividadeToSave.Id_TipoSituacaoAcomodacao,
                                                        atividadeToSave.Id_TipoAtividadeAcomodacao,
                                                        atividadeToSave.dt_InicioAtividadeAcomodacao,
                                                        atividadeToSave.dt_FimAtividadeAcomodacao,
                                                        atividadeToSave.Id_UsuarioSolicitante,
                                                        atividadeToSave.AcaoItems,
                                                        atividadeToSave.Cod_Prioritario,
                                                        atividadeToSave.Cod_Plus,
                                                        Perfis,
                                                        AcomodacaoItem.Where(m => m.Id_AtividadeAcomodacao==idAtividade).FirstOrDefault());


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAtividadePriorizadaContextChangesAsync(atividadeSaveEvent, atividadeToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(atividadeSaveEvent);


            return CreatedAtAction(nameof(PriorizarAtividade),"OK");
        }

        /// <summary>
        /// Indica que é uma atividade PLUS (Utilizada somente para Higienização).
        /// </summary>
        /// <param name="idAtividade">
        /// Identificador da Atividade da Acomodação.
        /// </param>
        /// <param name="plus">
        /// Indica se é para priorizar ou não S/N
        /// </param>
        ////POST api/v1/[controller]/items
        [Route("items/atividade/plus/{plus}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> PlusAtividade(int idAtividade, Priorizar plus)
        {
            string msgRule = "";

            var atividadeToSave = _operacionalContext.AtividadeItems
                        .OfType<AtividadeItem>()
                        .SingleOrDefault(e => e.Id_AtividadeAcomodacao == idAtividade);

            if (atividadeToSave == null)
            {

                string msgStatus = _localizer["VALIDA_EXISTENCIAATIVIDADE"];
                return BadRequest(msgStatus);
            }

            if (atividadeToSave.Id_TipoAtividadeAcomodacao != (int)TipoAtividade.HIGIENIZAÇÃO)
            {

                string msgStatus = _localizer["VALIDA_ATIVIDADEHIGIENIZACAO"];
                return BadRequest(msgStatus);
            }

            var situacaoToExit = _operacionalContext.SituacaoItems
                       .OfType<SituacaoItem>()
                       .SingleOrDefault(e => e.Id_SituacaoAcomodacao == atividadeToSave.Id_SituacaoAcomodacao);

            if (situacaoToExit == null)
            {

                string msgStatus = _localizer["VALIDA_EXISTENCIAATIVIDADE"];
                return BadRequest(msgStatus);
            }

            atividadeToSave.Cod_Plus = plus.ToString();

            _operacionalContext.AtividadeItems.Update(atividadeToSave);

            string AdministrativoURL = _settings.AdministrativoURL;
            string tokenURL = _settings.TokenURL;

            List<Administrativo.API.TO.ConsultarAcomodacaoDetalhePorIdAcomodacaoTO> AcomodacaoItem = await ConsultaAcomodacaoDetalhePorIdAcomodacaoAsync(AdministrativoURL, tokenURL, situacaoToExit.Id_Acomodacao);
            List<ConsultarAcessoAtividadeEmpresaPerfilTO> Perfis = await ConsultaAcessoPerfilPorTipoSituacaoTipoAtividadeAsync(AdministrativoURL, tokenURL, AcomodacaoItem[0].Id_Empresa, atividadeToSave.Id_TipoSituacaoAcomodacao, atividadeToSave.Id_TipoAtividadeAcomodacao);

            //Create Integration Event to be published through the Event Bus
            var atividadeSaveEvent = new AtividadeSaveIE(atividadeToSave.Id_AtividadeAcomodacao,
                                                                        atividadeToSave.Id_SituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoSituacaoAcomodacao,
                                                                        atividadeToSave.Id_TipoAtividadeAcomodacao,
                                                                        atividadeToSave.dt_InicioAtividadeAcomodacao,
                                                                        atividadeToSave.dt_FimAtividadeAcomodacao,
                                                                        atividadeToSave.Id_UsuarioSolicitante,
                                                                        atividadeToSave.AcaoItems,
                                                                        atividadeToSave.Cod_Prioritario,
                                                                        atividadeToSave.Cod_Plus,
                                                                        Perfis,
                                                                        situacaoToExit.Id_Acomodacao);


            try
            {
                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _operacionalIntegrationEventService.SaveEventAndAtividadeContextChangesAsync(atividadeSaveEvent, atividadeToSave);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            // Publish through the Event Bus and mark the saved event as published
            await _operacionalIntegrationEventService.PublishThroughEventBusAsync(atividadeSaveEvent);


            return CreatedAtAction(nameof(PriorizarAtividade), "OK");
        }
    }
}