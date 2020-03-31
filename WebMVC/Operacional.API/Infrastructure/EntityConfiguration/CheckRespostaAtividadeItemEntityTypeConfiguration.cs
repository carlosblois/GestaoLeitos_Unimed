using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class CheckRespostaAtividadeItemEntityTypeConfiguration
        : IEntityTypeConfiguration<CheckRespostaAtividadeItem>
    {
        public void Configure(EntityTypeBuilder<CheckRespostaAtividadeItem> builder)
        {
            builder.ToTable("CheckRespostaAtividade");
            builder.HasKey(ci => new { ci.Id_RespostasChecklist , ci.Id_AtividadeAcomodacao });
        }
    }
}
