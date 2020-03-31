using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class AcaoAtividadeItemEntityTypeConfiguration
        : IEntityTypeConfiguration<AcaoItem>
    {
        public void Configure(EntityTypeBuilder<AcaoItem> builder)
        {
            builder.ToTable("AcaoAtividadeAcomodacao");
            builder.HasKey(ci => new { ci.Id_AcaoAtividadeAcomodacao });
        }
    }
}
