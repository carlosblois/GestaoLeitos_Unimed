using System;


namespace Produto.API.Infrastructure.Exceptions
{
    public class ProdutoDomainExceptions : Exception     
    {
        public ProdutoDomainExceptions()
        { }

        public ProdutoDomainExceptions(string message)
            : base(message)
        { }

        public ProdutoDomainExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
