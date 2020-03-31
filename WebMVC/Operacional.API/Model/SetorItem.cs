
using System;

namespace Operacional.API.Model
{
    public class SetorItem
    {
        public Guid id_Empresa { get; set; }

        public Guid id_Setor { get; set; }

        public string nome_Setor { get; set; }

        public SetorItem() { }

    }
}
