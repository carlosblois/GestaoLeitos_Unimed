
using Microsoft.EntityFrameworkCore;
using Modulo.API.Infrastructure.EntityConfiguration;
using Modulo.API.Model;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Modulo.API.Infrastructure
{
    public class ModuloContext : DbContext
    {

        public ModuloContext(DbContextOptions<ModuloContext> options) : base(options)
        {
        }

        public DbSet<ModuloItem> ModuloItems { get; set; }
        public DbSet<OperacaoItem> OperacaoItems { get; set; }
        public DbSet<EmpresaPerfilModuloItem> EmpresaPerfilModuloItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ModuloItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new OperacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new EmpresaPerfilModuloItemEntityTypeConfiguration());

            builder.Entity<ModuloItem>()
                .HasMany(e => e.OperacaoItems)
                .WithOne(e => e.ModuloItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModuloItem>()
                .HasMany(e => e.EmpresaPerfilModuloItems)
                .WithOne(e => e.ModuloItem)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
