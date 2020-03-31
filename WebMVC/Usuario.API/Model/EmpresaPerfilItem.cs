
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Usuario.API.Model
{
    public class EmpresaPerfilItem
    {
        /// <summary>
        /// Identificador da Empresa
        /// </summary>
        [Required]
        public int id_Empresa { get; set; }
        /// <summary>
        /// Identificador do Perfil 
        /// </summary>
        [Required]
        public int id_Perfil { get; set; }

        /// <summary>
        /// Codigo do Tipo de Perfil - (Min 1 Max 1)
        /// Determina a caracteristica do perfil
        /// G - Gestão
        /// O - Operação
        /// </summary>
        [Required]
        [RegularExpression(@"[O,G]",ErrorMessage="As opções válidas são O - Operação ou G - Gestão")]
        [StringLength(1)]
        public string cod_Tipo { get; set; }

        [JsonIgnore]
        public PerfilItem PerfilItem { get; set; }

        public EmpresaPerfilItem() { }

    }
}
