
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Usuario.API.Infrastructure.EntityConfiguration;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure
{
    public class UsuarioContext : DbContext
    {

        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {
        }

        public DbSet<LogAcessoItem> LogAcessoItems { get; set; }
        public DbSet<UsuarioItem> UsuarioItems { get; set; }
        public DbSet<EmpresaPerfilItem> EmpresaPerfilItems { get; set; }
        public DbSet<UsuarioEmpresaPerfilItem> UsuarioEmpresaPerfilItems { get; set; }
        public DbSet<PerfilItem> PerfilItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new LogAcessoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new EmpresaPerfilItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new UsuarioEmpresaPerfilItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new PerfilItemEntityTypeConfiguration());

            builder.Entity<UsuarioItem>()
                .HasMany(e => e.UsuarioEmpresaPerfilItems)
                .WithOne(e => e.UsuarioItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PerfilItem>()
                .HasMany(e => e.EmpresaPerfilItems)
                .WithOne(e => e.PerfilItem)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
