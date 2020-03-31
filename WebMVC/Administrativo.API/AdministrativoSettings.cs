
namespace Administrativo.API
{
    public class AdministrativoSettings
    {

        public string cacheConnection { get; set; }
        public double cacheTime { get; set; }
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public bool UseCache { get; set; }
        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}

