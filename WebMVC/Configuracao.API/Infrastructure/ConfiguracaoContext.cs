using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Configuracao.API.Infrastructure.EntityConfiguration;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure
{
    public class ConfiguracaoContext : DbContext
    {
        public ConfiguracaoContext(DbContextOptions<ConfiguracaoContext> options) : base(options)
        {
        }

        public DbSet<FluxoAutomaticoCheckItem> FluxoAutomaticoCheckItems { get; set; }

        public DbSet<FluxoAutomaticoSituacaoItem> FluxoAutomaticoSituacaoItems { get; set; }

        public DbSet<FluxoAutomaticoAcaoItem> FluxoAutomaticoAcaoItems { get; set; }

        public DbSet<SLAItem> SLAItems { get; set; }

        public DbSet<SLASituacaoItem> SLASituacaoItems { get; set; }

        public DbSet<TipoSituacaoAcomodacaoItem> TipoSituacaoAcomodacaoItems { get; set; }

        public DbSet<TipoSituacaoTipoAtividadeAcomodacaoItem> TipoSituacaoTipoAtividadeAcomodacaoItems { get; set; }

        public DbSet<TipoAcaoAcomodacaoItem> TipoAcaoAcomodacaoItems { get; set; }

        public DbSet<TipoAtividadeAcomodacaoItem> TipoAtividadeAcomodacaoItems { get; set; }

        public DbSet<ChecklistItem> ChecklistItems { get; set; }

        public DbSet<ItemChecklistItem> ItemChecklistItems { get; set; }

        public DbSet<ChecklistItemChecklistItem> ChecklistItemChecklistItems { get; set; }

        public DbSet<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem> ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TipoAtividadeAcomodacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new TipoSituacaoAcomodacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new TipoSituacaoTipoAtividadeAcomodacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new TipoAcaoAcomodacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new SLASituacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new SLAItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChecklistItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new ItemChecklistItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChecklistItemChecklistItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoEntityTypeConfiguration());
            builder.ApplyConfiguration(new FluxoAutomaticoAcaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new FluxoAutomaticoSituacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new FluxoAutomaticoCheckItemEntityTypeConfiguration());
        }

    }
}
