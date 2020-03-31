using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class EmpresaTO
    {
       
        public string id_Empresa {get;set;}
        [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
        public string nome_Empresa { get; set; }
        [Required(ErrorMessage = "O Código externo da empresa é obrigatório.")]
        public string codExterno_Empresa { get; set; }
        [Required(ErrorMessage = "O endereço da empresa é obrigatório.")]
        public string endereco_Empresa { get; set; }
        [Required(ErrorMessage = "O complemento da empresa é obrigatório.")]
        public string complemento_Empresa { get; set; }
        [Required(ErrorMessage = "O número da empresa é obrigatório.")]
        public string numero_Empresa { get; set; }
        [Required(ErrorMessage = "O bairro da empresa é obrigatório.")]
        public string bairro_Empresa { get; set; }
        [Required(ErrorMessage = "O cidade da empresa é obrigatório.")]
        public string cidade_Empresa { get; set; }
        [Required(ErrorMessage = "O estado da empresa é obrigatório.")]
        public string estado_Empresa { get; set; }
        public string cep_Empresa { get; set; }
        public string fone_Empresa { get; set; }
        public string contato_Empresa { get; set; }
        public string cgC_Empresa { get; set; }
        public string inscricaoMunicipal_Empresa { get; set; }
        public string inscricaoEstadual_Empresa { get; set; }
        public string cneS_Empresa { get; set; }

        public EmpresaTO()
        {

            this.id_Empresa = "";
            this.nome_Empresa = "";
            this.codExterno_Empresa = "";
            this.endereco_Empresa = "";
            this.complemento_Empresa = "";
            this.numero_Empresa = "";
            this.bairro_Empresa = "";
            this.cidade_Empresa = "";
            this.estado_Empresa = "";
            this.cep_Empresa = "";
            this.fone_Empresa = "";
            this.contato_Empresa = "";
            this.cgC_Empresa = "";
            this.inscricaoMunicipal_Empresa = "";
            this.inscricaoEstadual_Empresa = "";
            this.cneS_Empresa = "";
        }
    }
}
