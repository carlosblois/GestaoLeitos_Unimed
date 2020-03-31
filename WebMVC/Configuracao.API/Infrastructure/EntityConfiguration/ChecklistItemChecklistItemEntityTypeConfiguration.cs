using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class ChecklistItemChecklistItemEntityTypeConfiguration
        : IEntityTypeConfiguration<ChecklistItemChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItemChecklistItem> builder)
        {
            builder.ToTable("Checklist_ItensChecklist");
            builder.HasKey(ci => new { ci.Id_Checklist , ci.Id_ItemChecklist });
        }
    }
}
