using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class FluxoAutomaticoSituacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<FluxoAutomaticoSituacaoItem>
    {
        public void Configure(EntityTypeBuilder<FluxoAutomaticoSituacaoItem> builder)
        {
            builder.ToTable("FluxoAutomaticoTipoSituacaoTransicao");
            builder.HasKey(ci => new { ci.Id_TipoSituacaoAcomodacaoOrigem, ci.Id_TipoAtividadeAcomodacaoDestino, ci.Id_TipoSituacaoAcomodacaoDestino, ci.Id_Empresa });
        }
    }
}
