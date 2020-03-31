using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Administrativo.API.Model;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class AcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<AcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<AcomodacaoItem> builder)
        {
            builder.ToTable("Acomodacao");
            builder.HasKey(ci => new { ci.Id_Acomodacao });
        }
    }
}
