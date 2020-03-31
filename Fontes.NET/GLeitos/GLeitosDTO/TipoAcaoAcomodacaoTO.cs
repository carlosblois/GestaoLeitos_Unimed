using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class TipoAcaoAcomodacaoTO
    {
        public string id_TipoAcaoAcomodacao { get; set; }
        public string nome_TipoAcaoAcomodacao { get;set;}
        public string nome_Status { get; set; }
        public string nome_Status_Label { get; set; }
        public string imagem { get; set; }
        public string cor_TipoAcaoAcomodacao { get; set; }
        public int ordem { get; set; }

        public TipoAcaoAcomodacaoTO()
        {
            this.id_TipoAcaoAcomodacao = "";
            this.nome_TipoAcaoAcomodacao = "";
            this.nome_Status = "";
            this.nome_Status_Label = "";
            this.imagem = "";
            this.cor_TipoAcaoAcomodacao = "";
            this.ordem = 0;
        }
    }
}
