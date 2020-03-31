using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class SLAItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SLAItem>
    {
        public void Configure(EntityTypeBuilder<SLAItem> builder)
        {
            builder.ToTable("SLA");
            builder.HasKey(ci => new { ci.Id_SLA });
        }
    }
}
