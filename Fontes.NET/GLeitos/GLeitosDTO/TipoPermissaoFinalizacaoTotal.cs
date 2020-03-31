using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class TipoPermissaoFinalizacaoTotalTO
    {
        public string cod_tipo { get; set; }
        public string nome_tipo { get; set; }

        public TipoPermissaoFinalizacaoTotalTO(string pcod_tipo, string pnome_tipo)
        {
            this.cod_tipo = pcod_tipo;
            this.nome_tipo = pnome_tipo;
        }
    }
}
