using System;
using System.Collections.Generic;
using System.Text;

namespace Dame.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class DameDomainException : Exception
    {
        public DameDomainException()
        { }

        public DameDomainException(string message)
            : base(message)
        { }

        public DameDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
