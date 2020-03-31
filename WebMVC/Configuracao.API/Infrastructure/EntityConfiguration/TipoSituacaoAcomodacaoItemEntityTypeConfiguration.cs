using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class TipoSituacaoAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoSituacaoAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<TipoSituacaoAcomodacaoItem> builder)
        {
            builder.ToTable("TipoSituacaoAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoSituacaoAcomodacao });
        }
    }
}
