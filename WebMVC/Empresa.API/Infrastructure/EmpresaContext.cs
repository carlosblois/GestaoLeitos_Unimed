using Microsoft.EntityFrameworkCore;
using Empresa.API.Infrastructure.EntityConfiguration;
using Empresa.API.Model;


namespace Empresa.API.Infrastructure
{
    public class EmpresaContext :DbContext
    {
        public EmpresaContext(DbContextOptions<EmpresaContext> options) : base(options)
        {
        }
        public DbSet<EmpresaItem> EmpresaItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EmpresaItemEntityTypeConfiguration());
        }

    }
}
