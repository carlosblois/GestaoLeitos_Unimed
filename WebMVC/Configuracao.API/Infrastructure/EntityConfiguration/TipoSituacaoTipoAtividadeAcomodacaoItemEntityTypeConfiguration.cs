using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class TipoSituacaoTipoAtividadeAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoSituacaoTipoAtividadeAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<TipoSituacaoTipoAtividadeAcomodacaoItem> builder)
        {
            builder.ToTable("TipoSituacao_TipoAtividadeAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoSituacaoAcomodacao,ci.Id_TipoAtividadeAcomodacao });
        }
    }
}
