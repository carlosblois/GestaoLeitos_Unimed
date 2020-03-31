using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class MensagemItemEntityTypeConfiguration
        : IEntityTypeConfiguration<MensagemItem>
    {
        public void Configure(EntityTypeBuilder<MensagemItem> builder)
        {
            builder.ToTable("Mensagem");
            builder.HasKey(ci => new { ci.Id_Mensagem});
        }
    }
}
