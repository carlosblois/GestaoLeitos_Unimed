using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class TipoAtividadeAcomodacaoTO
    {
        [Display(Description = "Identificador do Tipo de Atividade de Acomodação (Identity)")]
        public string id_TipoAtividadeAcomodacao { get; set; }
        [Display(Description = "Nome do Tipo de Atividade de Acomodação (Max 50)")]
        public string nome_TipoAtividadeAcomodacao { get;set;}
        public string imagem { get; set; }
        public int ordem { get; set; }
        public string cor_TipoAtividadeAcomodacao { get; set; }

        public TipoAtividadeAcomodacaoTO()
        {
            this.id_TipoAtividadeAcomodacao = "";
            this.nome_TipoAtividadeAcomodacao = "";
            this.imagem = "";
            this.ordem = 0;
            this.cor_TipoAtividadeAcomodacao = "";
    }
    }
}
