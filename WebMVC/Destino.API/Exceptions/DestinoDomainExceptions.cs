using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    namespace Destino.API.Infrastructure.Exceptions
    {
        public class DestinoDomainExceptions : Exception
        {
            public DestinoDomainExceptions()
            { }

            public DestinoDomainExceptions(string message)
                : base(message)
            { }

            public DestinoDomainExceptions(string message, Exception innerException)
                : base(message, innerException)
            { }
        }
    }

