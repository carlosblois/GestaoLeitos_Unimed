using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class FluxoAutomaticoAcaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<FluxoAutomaticoAcaoItem>
    {
        public void Configure(EntityTypeBuilder<FluxoAutomaticoAcaoItem> builder)
        {
            builder.ToTable("FluxoAutomaticoTipoSituacao_TipoAtividade");
            builder.HasKey(ci => new { ci.Id_TipoAcaoAcomodacaoOrigem,ci.Id_TipoAtividadeAcomodacaoOrigem,ci.Id_TipoSituacaoAcomodacaoOrigem,ci.Id_TipoAtividadeAcomodacaoDestino,ci.Id_TipoSituacaoAcomodacaoDestino,ci.Id_Empresa });
        }
    }
}
