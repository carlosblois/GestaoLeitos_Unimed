using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class TipoSituacaoAcomodacaoTO
    {
        [Display(Description = "Identificador do Tipo de Situação de Acomodação (Identity)")]
        public string id_TipoSituacaoAcomodacao { get; set; }
        [Display(Description = "Nome do Tipo de situação de Acomodação (Max 50)")]
        public string nome_TipoSituacaoAcomodacao { get;set;}
        public string imagem { get; set; }
        public int ordem { get; set; }
        public string cor_TipoSituacaoAcomodacao { get; set; }

        public TipoSituacaoAcomodacaoTO()
        {
            this.id_TipoSituacaoAcomodacao = "";
            this.nome_TipoSituacaoAcomodacao = "";
            this.imagem = "";
            this.ordem = 0;
            this.cor_TipoSituacaoAcomodacao = "";
        }
    }
}
