using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Dame.Infrastructure
{
    public class DameContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "ordering";


        private readonly IMediator _mediator;

        private DameContext(DbContextOptions<DameContext> options) : base (options) { }

        public DameContext(DbContextOptions<DameContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("OrderingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

      
    }

}
