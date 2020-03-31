using System;


namespace Modulo.API.Infrastructure.Exceptions
{
    public class ModuloDomainExceptions : Exception     
    {
        public ModuloDomainExceptions()
        { }

        public ModuloDomainExceptions(string message)
            : base(message)
        { }

        public ModuloDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
