using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Operacional.API.Enum
{
    public class ExpoEnum
    {

        public enum Ocupado
        {
            S = 1,
            N = 2

        }

        public enum TipoPacienteLeito
        {
            OCUPADO = 1,
            NAOOCUPADO = 2,
            TODOS = 3

        }

        public enum TipoAcao
        {
            ACEITAR = 1,
            INICIAR = 2,
            FINALIZAR_TOTALMENTE = 3,
            FINALIZAR_PARCIALMENTE = 4,
            SOLICITAR = 5

        }

        public enum TipoAtividade
        {
            MENSAGEIRO = 1,
            HIGIENIZAÇÃO = 2,
            MANUTENÇÃO = 3,
            ENGENHARIA = 4,
            AGUARDAFAMILIAR = 5,
            AMBULANCIA = 6,
            ENFERMAGEM = 7,
            CAMAREIRAPRE = 8,
            CAMAREIRAPOS = 9,
            ALTAPROGRAMADA = 10,
            ALTAADMINISTRATIVA = 11,
            INTERDICAO = 12
        }

        public enum TipoSituacao
        {
            ALTAMEDICA = 1,
            RESERVADO = 2,
            OCUPADO = 3,
            INTERDITADO = 5,
            VAGO = 6,
            LIBERADO = 7
        }

        public enum Priorizar
        {
            S = 1,
            N = 2

        }

    }
}