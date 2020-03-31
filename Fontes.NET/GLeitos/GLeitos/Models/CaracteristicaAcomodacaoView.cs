using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GLeitos.GLeitosTO;

namespace GLeitos.Models
{
    public class CaracteristicaAcomodacaoView : BaseView
    {

        public CaracteristicaAcomodacaoTO CaracteristicaAcomodacao { get; set; }
        public List<CaracteristicaAcomodacaoTO> ListaCaracteristicaAcomodacao { get; set; } 

        public CaracteristicaAcomodacaoView()
        {
            this.ListaCaracteristicaAcomodacao = new List<CaracteristicaAcomodacaoTO>();
            this.CaracteristicaAcomodacao = new CaracteristicaAcomodacaoTO();
            this.erro = "";
            this.mensagem = "";
        }

    }
    
}