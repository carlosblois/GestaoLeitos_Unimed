using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GLeitos.GLeitosTO
{
    public class UfTO
    {
        public string sigla { get; set; }
        public string descricao { get; set; }

        public UfTO(string psigla, string pdescricao){
            this.descricao = pdescricao;
            this.sigla = psigla;
        }
    }
}
