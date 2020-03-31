using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class FluxoAutomaticoCheckItemEntityTypeConfiguration
        : IEntityTypeConfiguration<FluxoAutomaticoCheckItem>
    {
        public void Configure(EntityTypeBuilder<FluxoAutomaticoCheckItem> builder)
        {
            builder.ToTable("FluxoAutomaticoCheck");
            builder.HasKey(ci => new { ci.Id_Checklist, ci.Id_TipoSituacaoAcomodacao, ci.Id_ItemChecklist, ci.Id_TipoAtividadeAcomodacao });
        }
    }
}
