
using System;
using System.ComponentModel.DataAnnotations;

namespace Administrativo.API.Model
{
    public class AcessoEmpresaPerfilTSTAItem
    {   

        /// <summary>
        /// Identificador do acesso (Identity)
        /// </summary>
        [Required]
        public int Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade { get; set; }

        /// <summary>
        /// Identificador do Perfil
        /// </summary>
        [Required]
        public int Id_Perfil { get; set; }

        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int Id_Empresa { get; set; }

        /// <summary>
        /// Identificador do Tipo de Situacao
        /// </summary>
        [Required]
        public int Id_TipoSituacaoAcomodacao { get; set; }


        /// <summary>
        /// Identificador do tipo de atividade.
        /// </summary>
        [Required]
        public int Id_TipoAtividadeAcomodacao { get; set; }


        /// <summary>
        /// Codigo que caracteriza o acesso V visualizar / E executar . (Max 1)
        /// </summary>
        [Required]
        [RegularExpression(@"[E,V]", ErrorMessage = "As opções válidas são E - Execução ou V - Visualização")]
        [StringLength(1)]
        public string Cod_Tipo { get; set; }


    }
}
