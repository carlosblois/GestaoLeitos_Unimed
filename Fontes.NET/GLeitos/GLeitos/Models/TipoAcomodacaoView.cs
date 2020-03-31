using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class TipoAcomodacaoView : BaseView
    {

        public TipoAcomodacaoTO TipoAcomodacao { get; set; }
        public List<EmpresaTO> ListaEmpresas { get; set; }
        public List<CaracteristicaAcomodacaoTO> ListaCaracteristicas { get; set; }

        public TipoAcomodacaoView()
        {
            this.ListaEmpresas = new List<EmpresaTO>();
            this.ListaCaracteristicas = new List<CaracteristicaAcomodacaoTO>();
            this.TipoAcomodacao = new TipoAcomodacaoTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}