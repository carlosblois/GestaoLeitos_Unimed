

namespace Modulo.API
{
    public class ModuloSettings
    {
        
            
        public string CacheConnection { get; set; }
        public double CacheTime { get; set; }
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public bool UseCache { get; set; }
        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}
