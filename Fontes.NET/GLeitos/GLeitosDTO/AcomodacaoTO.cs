using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class AcomodacaoTO
    {

        public string id_Acomodacao { get; set; }
        [Required(ErrorMessage = "A empresa é obrigatória.")]
        public string id_Empresa { get;set;}
        [Required(ErrorMessage = "O nome da acomodação é obrigatório.")]
        public string nome_Acomodacao { get; set; }
        [Required(ErrorMessage = "O Setor é obrigatório.")]
        public string id_Setor { get; set; }
        [Required(ErrorMessage = "O Código externo da acomodação é obrigatório.")]
        public string codExterno_Acomodacao { get; set; }
        [Required(ErrorMessage = "O identificador do tipo da acomodação é obrigatório.")]
        public string id_TipoAcomodacao { get; set; }
        public string cod_Isolamento { get; set; }
        public string nome_Setor { get; set; }
        public string nome_TipoAcomodacao { get; set; }
        public string nome_CaracteristicaAcomodacao { get; set; }
        
        public AcomodacaoTO()
        {
            this.id_TipoAcomodacao = "";
            this.id_Empresa = "";
            this.nome_Acomodacao = "";
            this.id_Setor = "";
            this.codExterno_Acomodacao = "";
            this.id_TipoAcomodacao = "";
            this.cod_Isolamento = "N";
            this.nome_Setor = "";
            this.nome_TipoAcomodacao = "";
            this.nome_CaracteristicaAcomodacao = "";
        }
    }
}
