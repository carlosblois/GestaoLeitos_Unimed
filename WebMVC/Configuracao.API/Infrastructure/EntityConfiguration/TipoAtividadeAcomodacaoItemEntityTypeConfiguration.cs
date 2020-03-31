using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class TipoAtividadeAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoAtividadeAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<TipoAtividadeAcomodacaoItem> builder)
        {
            builder.ToTable("TipoAtividadeAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoAtividadeAcomodacao });
        }
    }
}
