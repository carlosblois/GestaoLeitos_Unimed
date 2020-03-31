using System;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class BaseView
    {
        public string mensagem { get; set; }
        public string erro { get; set; }

        public string token_modulo { get; set; }
        public string url_getAcomodacao { get; set; }
        public string url_getHistoricoAcomodacao { get; set; }
        public string url_priorizaAcomodacao { get; set; }
        public string url_isolaAcomodacao { get; set; }
        public string url_limpezaPlusAcomodacao { get; set; }
        public string url_EncaminharAtividade { get; set; }
        public string url_GerarAtividade { get; set; }
        public string url_CancelarAtividade { get; set; }
        public string tkn_operacional { get; set; }
        public string tkn_administrativo { get; set; }
        public string tkn_configuracao { get; set; }
        public string url_listatipoatividade { get; set; }
        public string url_listatiposituacao { get; set; }
        public string url_EncaminharSituacao { get; set; }
        public string url_getMensagens { get; set; }
        public string url_sendMensagem { get; set; }
        public string IdUsuarioLogado { get; set; }

    }
}