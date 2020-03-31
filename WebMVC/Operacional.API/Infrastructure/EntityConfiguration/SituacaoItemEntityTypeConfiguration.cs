using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class SituacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SituacaoItem>
    {
        public void Configure(EntityTypeBuilder<SituacaoItem> builder)
        {
            builder.ToTable("SituacaoAcomodacao");
            builder.HasKey(ci => new { ci.Id_SituacaoAcomodacao });
        }
    }
}
