using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Operacional.API.Infrastructure.EntityConfiguration;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure
{
    public class OperacionalContext : DbContext
    {
        public OperacionalContext(DbContextOptions<OperacionalContext> options) : base(options)
        {
        }

        //public DbSet<CheckRespostaAtividadeItem> CheckRespostaAtividadeItems { get; set; }

        public DbSet<SituacaoItem> SituacaoItems {  get; set; }

        public DbSet<AtividadeItem> AtividadeItems { get; set; }

        public DbSet<AcaoItem> AcaoItems { get; set; }

        public DbSet<MensagemItem> MensagemItems { get; set; }

        public DbSet<RespostasChecklistItem> RespostasChecklistItems { get; set; }

        public DbSet<PacienteItem> PacienteItems { get; set; }

        public DbSet<PacienteAcomodacaoItem> PacienteAcomodacaoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MensagemItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new SituacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new AtividadeItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new AcaoAtividadeItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new RespostasChecklistItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new PacienteItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new PacienteAcomodacaoItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new CheckRespostaAtividadeItemEntityTypeConfiguration());

            builder.Entity<SituacaoItem>()
                .HasMany(e => e.AtividadeItems)
                .WithOne(e => e.SituacaoItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AtividadeItem>()
                .HasMany(e => e.AcaoItems)
                .WithOne(e => e.AtividadeItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AtividadeItem>()
               .HasMany(e => e.MensagemItems)
               .WithOne(e => e.AtividadeItem)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AtividadeItem>()
                .HasMany(e => e.CheckRespostaAtividadeItems )
                .WithOne(e => e.AtividadeItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RespostasChecklistItem>()
                .HasMany(e => e.CheckRespostaAtividadeItems)
                .WithOne(e => e.RespostasChecklistItem)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PacienteItem>()
                .HasMany(e => e.PacienteAcomodacaoItems)
                .WithOne(e => e.PacienteItem)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
