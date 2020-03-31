using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class RespostasChecklistItemEntityTypeConfiguration
        : IEntityTypeConfiguration<RespostasChecklistItem>
    {
        public void Configure(EntityTypeBuilder<RespostasChecklistItem> builder)
        {
            builder.ToTable("RespostasChecklistSituacaoAcomodacao");
            builder.HasKey(ci => new { ci.Id_RespostasChecklist });
        }
    }
}
