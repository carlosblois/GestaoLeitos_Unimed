using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class ItemChecklistItemEntityTypeConfiguration
        : IEntityTypeConfiguration<ItemChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ItemChecklistItem> builder)
        {
            builder.ToTable("ItensChecklist");
            builder.HasKey(ci => new { ci.Id_ItemChecklist });
        }
    }
}
