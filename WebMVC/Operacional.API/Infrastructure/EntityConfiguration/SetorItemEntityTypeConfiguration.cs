using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class SetorItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SetorItem>
    {
        public void Configure(EntityTypeBuilder<SetorItem> builder)
        {
            builder.ToTable("Setor");
            builder.HasKey(ci => new { ci.id_Setor, ci.id_Empresa  });
        }
}
}
