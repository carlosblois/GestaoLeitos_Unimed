using System;


namespace Configuracao.API.Infrastructure.Exceptions
{
    public class ConfiguracaoDomainExceptions : Exception     
    {
        public ConfiguracaoDomainExceptions()
        { }

        public ConfiguracaoDomainExceptions(string message)
            : base(message)
        { }

        public ConfiguracaoDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
