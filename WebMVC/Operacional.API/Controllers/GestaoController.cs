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

namespace Operacional.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestaoController : ControllerBase
    {

        private const string cachePrefix = "GESTAO#";
        private readonly OperacionalContext _operacionalContext;
        private readonly OperacionalSettings _settings;
        private readonly IOperacionalIntegrationEventService _operacionalIntegrationEventService;
        private readonly IStringLocalizer<GestaoController> _localizer;

        public GestaoController(OperacionalContext context, IOptionsSnapshot<OperacionalSettings> settings, IOperacionalIntegrationEventService operacionalIntegrationEventService, IStringLocalizer<GestaoController> localizer)
        {
            _operacionalContext = context ?? throw new ArgumentNullException(nameof(context));
            _operacionalIntegrationEventService = operacionalIntegrationEventService ?? throw new ArgumentNullException(nameof(operacionalIntegrationEventService));
            _settings = settings.Value;
            _localizer = localizer;
        }

        /// <summary>
        /// TELA DE GESTAO (código: 06) Consulta o Resumo do Dashboard.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        [HttpGet]
        [Route("items/dashboardheader/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarDashBoardHeaderTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarDashboardHeader(int IdEmpresa)
        {
            List<ConsultarDashBoardHeaderTO> l_ListTO = new List<ConsultarDashBoardHeaderTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarDashBoardHeaderTO> mycache = new Cache<ConsultarDashBoardHeaderTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarDashboardHeader_" + cachePrefix + IdEmpresa.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarDashBoardHeaderTO sqlClass = new ConsultarDashBoardHeaderTO();
                    sqlClass.ConsultarDashboardHeaderTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarDashboardHeader_" + cachePrefix + IdEmpresa.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarDashBoardHeaderTO sqlClass = new ConsultarDashBoardHeaderTO();
                sqlClass.ConsultarDashboardHeaderTOCommand(IdEmpresa, _settings.ConnectionString, ref l_ListTO);
            }


                return Ok(l_ListTO);

        }

        /// <summary>
        /// TELA DE GESTAO (código: 06) Consulta o detalhe do Dashboard.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        [HttpGet]
        [Route("items/dashboardbody/empresa/{IdEmpresa}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarDashBoardBodyTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarDashboardBody(int  IdEmpresa)
        {
            List<ConsultarDashBoardBodyTO> l_ListTO = new List<ConsultarDashBoardBodyTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarDashBoardBodyTO> mycache = new Cache<ConsultarDashBoardBodyTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarDashboardBody_" + cachePrefix +
                                                                    IdEmpresa.ToString() );
                if (l_ListTO.Count == 0)
                {
                    ConsultarDashBoardBodyTO sqlClass = new ConsultarDashBoardBodyTO();
                    sqlClass.ConsultarDashBoardBodyTOCommand(IdEmpresa,_settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarDashboardBody_" + cachePrefix + IdEmpresa.ToString() , l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarDashBoardBodyTO sqlClass = new ConsultarDashBoardBodyTO();
                sqlClass.ConsultarDashBoardBodyTOCommand(IdEmpresa,_settings.ConnectionString, ref l_ListTO);
            }


                return Ok(l_ListTO);

        }


        [HttpGet]
        [Route("items/logacesso/")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarLogAcessoPorAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarLogAcessoPorAtividade()
        {
            List<ConsultarLogAcessoPorAtividadeTO> l_ListTO = new List<ConsultarLogAcessoPorAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarLogAcessoPorAtividadeTO> mycache = new Cache<ConsultarLogAcessoPorAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarLogAcessoPorAtividade_" + cachePrefix );
                if (l_ListTO.Count == 0)
                {
                    ConsultarLogAcessoPorAtividadeTO sqlClass = new ConsultarLogAcessoPorAtividadeTO();
                    sqlClass.ConsultarLogAcessoPorAtividadeTOCommand( _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarLogAcessoPorAtividade_" + cachePrefix , l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarLogAcessoPorAtividadeTO sqlClass = new ConsultarLogAcessoPorAtividadeTO();
                sqlClass.ConsultarLogAcessoPorAtividadeTOCommand( _settings.ConnectionString, ref l_ListTO);
            }


            return Ok(l_ListTO);

        }
        /// <summary>
        /// TELA DE GESTAO (código: 07) Consulta o Dashboard de situação.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        /// <param name="IdTipoSituacaoAcomodacao">
        /// Identificador do tipo de situação
        /// </param>
        [HttpGet]
        [Route("items/dashboard/empresa/{IdEmpresa}/tiposituacao/{IdTipoSituacaoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarDashBoardSituacaoTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarDashboardSituacao(int IdEmpresa,int IdTipoSituacaoAcomodacao)
        {
            List<ConsultarDashBoardSituacaoTO> l_ListTO = new List<ConsultarDashBoardSituacaoTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarDashBoardSituacaoTO> mycache = new Cache<ConsultarDashBoardSituacaoTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarDashboardSituacao_" + cachePrefix +
                                                                    IdEmpresa.ToString() + "@" +
                                                                    IdTipoSituacaoAcomodacao.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarDashBoardSituacaoTO sqlClass = new ConsultarDashBoardSituacaoTO();
                    sqlClass.ConsultarDashBoardSituacaoTOCommand(IdEmpresa, IdTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarDashboardSituacao_" + cachePrefix + IdEmpresa.ToString() + "@" +
                                                    IdTipoSituacaoAcomodacao.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarDashBoardSituacaoTO sqlClass = new ConsultarDashBoardSituacaoTO();
                sqlClass.ConsultarDashBoardSituacaoTOCommand(IdEmpresa, IdTipoSituacaoAcomodacao, _settings.ConnectionString, ref l_ListTO);
            }

                return Ok(l_ListTO);

        }

        /// <summary>
        /// TELA DE GESTAO (código: 08) Consulta o Dashboard de atividade.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        /// <param name="IdTipoAtividadeAcomodacao">
        /// Identificador do tipo de atividade
        /// </param>
        [HttpGet]
        [Route("items/dashboard/empresa/{IdEmpresa}/tipoatividade/{IdTipoAtividadeAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarDashBoardAtividadeTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarDashboardAtividade(int IdEmpresa, int IdTipoAtividadeAcomodacao)
        {
            List<ConsultarDashBoardAtividadeTO> l_ListTO = new List<ConsultarDashBoardAtividadeTO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarDashBoardAtividadeTO> mycache = new Cache<ConsultarDashBoardAtividadeTO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarDashboardAtividade_" + cachePrefix +
                                                                    IdEmpresa.ToString() + "@" +
                                                                    IdTipoAtividadeAcomodacao.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarDashBoardAtividadeTO sqlClass = new ConsultarDashBoardAtividadeTO();
                    sqlClass.ConsultarDashBoardAtividadeTOCommand(IdEmpresa, IdTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarDashboardAtividade_" + cachePrefix + IdEmpresa.ToString() + "@" +
                                                    IdTipoAtividadeAcomodacao.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarDashBoardAtividadeTO sqlClass = new ConsultarDashBoardAtividadeTO();
                sqlClass.ConsultarDashBoardAtividadeTOCommand(IdEmpresa, IdTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTO);
            }


                return Ok(l_ListTO);

        }


        /// <summary>
        /// TELA DE GESTAO (código: 09) Consulta o Dashboard de situacao/atividade.
        /// </summary>
        /// <param name="IdEmpresa">
        /// Identificador da Empresa
        /// </param>
        /// <param name="IdTipoSituacaoAcomodacao">
        /// Identificador do tipo de situacao
        /// </param>
        /// <param name="IdTipoAtividadeAcomodacao">
        /// Identificador do tipo de atividade
        /// Enviando o valor 0 retornará todos os tipos de atividade
        /// </param>
        [HttpGet]
        [Route("items/dashboard/empresa/{IdEmpresa}/tipoatividade/{IdTipoAtividadeAcomodacao}/tiposituacao/{IdTipoSituacaoAcomodacao}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ConsultarDashBoardSATO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConsultarDashboardSituacaoAtividade(int IdEmpresa, int IdTipoSituacaoAcomodacao, int IdTipoAtividadeAcomodacao)
        {
            List<ConsultarDashBoardSATO> l_ListTO = new List<ConsultarDashBoardSATO>();
            if (_settings.UseCache)
            {
                Cache<ConsultarDashBoardSATO> mycache = new Cache<ConsultarDashBoardSATO>(_settings.cacheConnection, _settings.cacheTime);

                l_ListTO = await mycache.GetListAsync("ConsultarDashboardSituacaoAtividade_" + cachePrefix +
                                                                    IdEmpresa.ToString() + "@" +
                                                                    IdTipoSituacaoAcomodacao.ToString() + "@" +
                                                                    IdTipoAtividadeAcomodacao.ToString());
                if (l_ListTO.Count == 0)
                {
                    ConsultarDashBoardSATO sqlClass = new ConsultarDashBoardSATO();
                    sqlClass.ConsultarDashBoardSATOCommand(IdEmpresa, IdTipoSituacaoAcomodacao, IdTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTO);

                    if (l_ListTO.Count > 0)
                    {
                        await mycache.SetListAsync("ConsultarDashboardSituacaoAtividade_" + cachePrefix + IdEmpresa.ToString() + "@" +
                                                    IdTipoSituacaoAcomodacao.ToString() + "@" +
                                                    IdTipoAtividadeAcomodacao.ToString(), l_ListTO);
                    }
                }
            }
            else
            {
                ConsultarDashBoardSATO sqlClass = new ConsultarDashBoardSATO();
                sqlClass.ConsultarDashBoardSATOCommand(IdEmpresa, IdTipoSituacaoAcomodacao, IdTipoAtividadeAcomodacao, _settings.ConnectionString, ref l_ListTO);
            }


            return Ok(l_ListTO);

        }

    }
}