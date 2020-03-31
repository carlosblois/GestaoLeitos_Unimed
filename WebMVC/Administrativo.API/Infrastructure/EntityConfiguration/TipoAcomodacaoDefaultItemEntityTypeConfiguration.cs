using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TipoAcomodacao.API.Model;


namespace TipoAcomodacao.API.Infrastructure.EntityConfiguration
{
    class TipoAcomodacaoDefaultItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoAcomodacaoDefaultItem>
    {
        public void Configure(EntityTypeBuilder<TipoAcomodacaoDefaultItem> builder)
        {
            builder.ToTable("DefaultTipoAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoAcomodacao });
        }
    }
}

