using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class TipoAcaoAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoAcaoAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<TipoAcaoAcomodacaoItem> builder)
        {
            builder.ToTable("TipoAcaoAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoAcaoAcomodacao });
        }
    }
}
