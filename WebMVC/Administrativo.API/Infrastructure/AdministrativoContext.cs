using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Administrativo.API.Infrastructure.EntityConfiguration;
using Administrativo.API.Model;

namespace Administrativo.API.Infrastructure
{
    public class AdministrativoContext : DbContext
    {
        public AdministrativoContext(DbContextOptions<AdministrativoContext> options) : base(options)
        {
        }

        public DbSet<AcomodacaoItem> AcomodacaoItems { get; set; }

        public DbSet<TipoAcomodacaoItem> TipoAcomodacaoItems { get; set; }

        public DbSet<SetorItem> SetorItems { get; set; }

        public DbSet<EmpresaItem> EmpresaItems { get; set; }

        public DbSet<CaracteristicaAcomodacaoItem> CaracteristicaAcomodacaoItems { get; set; }

        public DbSet<AcessoEmpresaPerfilTSTAItem> AcessoEmpresaPerfilTSTAItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TipoAcomodacaoItemEntityTypeConfiguration());

            builder.ApplyConfiguration(new SetorItemEntityTypeConfiguration());

            builder.ApplyConfiguration(new CaracteristicaAcomodacaoItemEntityTypeConfiguration());

            builder.ApplyConfiguration(new EmpresaItemEntityTypeConfiguration());

            builder.ApplyConfiguration(new AcomodacaoItemEntityTypeConfiguration());

            builder.ApplyConfiguration(new AcessoEmpresaPerfilTSTAItemEntityTypeConfiguration());
        }

    }
}
