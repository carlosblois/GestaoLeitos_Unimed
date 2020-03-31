using System;


namespace Administrativo.API.Infrastructure.Exceptions
{
    public class AdministrativoDomainExceptions : Exception     
    {
        public AdministrativoDomainExceptions()
        { }

        public AdministrativoDomainExceptions(string message)
            : base(message)
        { }

        public AdministrativoDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
