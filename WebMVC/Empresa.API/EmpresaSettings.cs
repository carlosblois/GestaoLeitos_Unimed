
namespace Empresa.API
{
    public class EmpresaSettings
    {

        public string EventBusConnection { get; set; }
        public bool UseCache { get; set; }
        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}
