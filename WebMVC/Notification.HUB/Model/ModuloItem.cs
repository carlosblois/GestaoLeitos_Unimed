
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notification.HUB.IntegrationEvents.Model
{
    public class ModuloItem
    {
        [Required]
        public int Id_Modulo { get; set; }

        [Required]
        [StringLength(20),MinLength(5)]
        public string Nome_Modulo { get; set; }

        public  List<OperacaoItem> OperacaoItems { get; set; }

        public  List<EmpresaPerfilModuloItem> EmpresaPerfilModuloItems { get; set; }

        public ModuloItem() { }

    }
}
