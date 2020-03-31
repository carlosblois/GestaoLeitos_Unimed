using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class AtividadeItemEntityTypeConfiguration
        : IEntityTypeConfiguration<AtividadeItem>
    {
        public void Configure(EntityTypeBuilder<AtividadeItem> builder)
        {
            builder.ToTable("AtividadeAcomodacao");
            builder.HasKey(ci => new { ci.Id_AtividadeAcomodacao });
        }
    }
}
