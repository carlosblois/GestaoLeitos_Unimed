using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoEntityTypeConfiguration
        : IEntityTypeConfiguration<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem> builder)
        {
            builder.ToTable("ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao");
            builder.HasKey(ci => new { ci.Id_CheckTSTAT });
        }
    }
}
