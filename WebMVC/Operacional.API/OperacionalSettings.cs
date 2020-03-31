using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operacional.API
{
    public class OperacionalSettings
    {
        public string ConfiguracaoURL { get; set; }
        public string AdministrativoURL { get; set; }
        public string TokenURL { get; set; }

        public string cacheConnection { get; set; }
        public double cacheTime { get; set; }
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public bool UseCache { get; set; }
        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
        public string urlApiLiberacao { get; set; }
        public string rotaApiLiberacao { get; set; }
        public string userApiLiberacao { get; set; }
        public string passwordApiLiberacao { get; set; }
        public string hostApiLiberacao { get; set; }

    }
}
