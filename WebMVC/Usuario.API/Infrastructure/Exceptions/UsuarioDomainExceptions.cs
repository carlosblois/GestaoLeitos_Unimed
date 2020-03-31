using System;


namespace Usuario.API.Infrastructure.Exceptions
{
    public class UsuarioDomainExceptions : Exception     
    {
        public UsuarioDomainExceptions()
        { }

        public UsuarioDomainExceptions(string message)
            : base(message)
        { }

        public UsuarioDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
