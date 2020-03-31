using System;


namespace Operacional.API.Infrastructure.Exceptions
{
    public class OperacionalDomainExceptions : Exception     
    {
        public OperacionalDomainExceptions()
        { }

        public OperacionalDomainExceptions(string message)
            : base(message)
        { }

        public OperacionalDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
