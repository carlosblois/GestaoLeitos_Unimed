using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class SetorTO
    {
     
        public string id_Setor { get;set;}
        [Required(ErrorMessage = "O nome do Setor é obrigatório.")]
        public string nome_Setor { get; set; }
        [Required(ErrorMessage = "O Código externo do Setor é obrigatório.")]
        public string codExterno_Setor { get; set; }
        [Required(ErrorMessage = "O Código da empresa do Setor é obrigatório.")]
        public string id_Empresa { get; set; }

        public SetorTO()
        {
            this.id_Setor = "";
            this.nome_Setor = "";
            this.codExterno_Setor = "";
            this.id_Empresa = "";

        }
    }
}
