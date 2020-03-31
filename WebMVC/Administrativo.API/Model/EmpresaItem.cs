
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class EmpresaItem
    {

          


        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int id_Empresa { get; set; }

        /// <summary>
        /// Nome da Empresa (Max 60)
        /// </summary>
        [Required]
        [StringLength(60)]
        public string Nome_Empresa { get; set; }

        /// <summary>
        /// Codigo externo da empresa, com proposito de integração. (Max 6)
        /// </summary>
        [Required]
        [StringLength(6)]
        public string CodExterno_Empresa { get; set; }


        /// <summary>
        /// Endereço da empresa. (Max 150)
        /// </summary>
        [StringLength(150)]
        public string Endereco_Empresa { get; set; }

        /// <summary>
        /// Complemento do endereço da empresa. (Max 30)
        /// </summary>
        [StringLength(30)]
        public string Complemento_Empresa { get; set; }

        /// <summary>
        /// Numero do endereço da empresa. (Max 10)
        /// </summary>
        [StringLength(10)]
        public string Numero_Empresa { get; set; }

        /// <summary>
        /// Bairro do endereço da empresa. (Max 30)
        /// </summary>
        [StringLength(30)]
        public string Bairro_Empresa { get; set; }

        /// <summary>
        /// Cidade do endereço da empresa. (Max 80)
        /// </summary>
        [StringLength(80)]
        public string Cidade_Empresa { get; set; }

        /// <summary>
        /// Estado do endereço da empresa. (Max 2)
        /// </summary>
        [StringLength(2)]
        public string Estado_Empresa { get; set; }

        /// <summary>
        /// Cep do endereço da empresa. (Max 8)
        /// </summary>
        [StringLength(8)]
        public string Cep_Empresa { get; set; }

        /// <summary>
        /// Telefone da empresa. (Max 20)
        /// </summary>
        [StringLength(20)]
        public string Fone_Empresa { get; set; }


        /// <summary>
        /// Contato da empresa. (Max 30)
        /// </summary>
        [StringLength(30)]
        public string Contato_Empresa { get; set; }

        /// <summary>
        /// CGC da empresa. (Max 19)
        /// </summary>
        [StringLength(19)]
        public string CGC_Empresa { get; set; }

        /// <summary>
        /// Inscricao Municipal da empresa. (Max 12)
        /// </summary>
        [StringLength(12)]
        public string InscricaoMunicipal_Empresa { get; set; }

        /// <summary>
        /// Inscricao Estadual da empresa. (Max 14)
        /// </summary>
        [StringLength(14)]
        public string InscricaoEstadual_Empresa { get; set; }

        /// <summary>
        /// CNES da empresa. (Max 7)
        /// </summary>
        [StringLength(7)]
        public string CNES_Empresa { get; set; }
       
        public EmpresaItem() { }

    }
}
