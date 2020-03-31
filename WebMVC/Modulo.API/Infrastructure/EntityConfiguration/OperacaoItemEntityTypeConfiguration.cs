using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modulo.API.Model;


namespace Modulo.API.Infrastructure.EntityConfiguration
{
    class OperacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<OperacaoItem>
    {
        public void Configure(EntityTypeBuilder<OperacaoItem> builder)
        {
            builder.ToTable("Operacao");
            builder.HasKey(ci => new {ci.Id_Operacao });

        }
    }
}
