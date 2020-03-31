using System;

namespace Framework.Util
{

    public class Utilitarios
    {

        public static string FormataDataDDMMYYYY(string data_YYYYMMDD)
        {
            try
            {
                string aux = "";
                data_YYYYMMDD = data_YYYYMMDD.Trim();

                data_YYYYMMDD = data_YYYYMMDD.PadLeft(8, '0');

                if (data_YYYYMMDD.Length == 8)
                    aux = data_YYYYMMDD.Substring(6, 2) + "/" + data_YYYYMMDD.Substring(4, 2) + "/" + data_YYYYMMDD.Substring(0, 4);

                return aux;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string FormataStringParaMoeda(string valorString)
        {
            decimal retorno;
            decimal.TryParse(valorString, out retorno);

            return string.Format("{0:C2}", retorno);
        }

        public static string FormataStringParaMoeda(Object valorStringObject)
        {
            return FormataStringParaMoeda(valorStringObject.ToString());
        }

        public static string FormataStringParaDecimal(string valorString)
        {
            decimal retorno;
            decimal.TryParse(valorString, out retorno);

            return string.Format("{0:N}", retorno);
        }

        public static string FormataStringParaDecimal(string valorString, int nDecimais)
        {
            decimal retorno;
            decimal.TryParse(valorString, out retorno);

            return retorno.ToString("N" + nDecimais.ToString());
        }

        public static decimal PrecisaoDecimalSemArrendondar(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            int tmp = (int)Math.Truncate(step * value);
            return tmp / step;
        }

        public static decimal ConvertStringToDecimal(string strValue)
        {
            decimal lret = 0;
            strValue = strValue.Replace("R", "").Replace("$", "").Replace(" ", "");
            if (decimal.TryParse(strValue, out lret))
            {
                return lret;
            }
            else
            {
                return 0;
            }
        }

        public static string nomeMes(int mes)
        {
            // retorna o nome do mes por extenço
            string nome;

            switch (mes)
            {
                case 1: nome = "Janeiro"; break;
                case 2: nome = "Fevereiro"; break;
                case 3: nome = "Março"; break;
                case 4: nome = "Abril"; break;
                case 5: nome = "Maio"; break;
                case 6: nome = "Junho"; break;
                case 7: nome = "Julho"; break;
                case 8: nome = "Agosto"; break;
                case 9: nome = "Setembro"; break;
                case 10: nome = "Outubro"; break;
                case 11: nome = "Novembro"; break;
                case 12: nome = "Dezembro"; break;
                default: throw new Exception("Mes inválido.");
            }

            return nome;
        }

        public static bool isNumberic(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

    }


}
