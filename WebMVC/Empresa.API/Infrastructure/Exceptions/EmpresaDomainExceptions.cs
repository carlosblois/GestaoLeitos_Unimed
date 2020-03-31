using System;


namespace Empresa.API.Infrastructure.Exceptions
{
    public class EmpresaDomainExceptions : Exception     
    {
        public EmpresaDomainExceptions()
        { }

        public EmpresaDomainExceptions(string message)
            : base(message)
        { }

        public EmpresaDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
