using Administrativo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class SetorItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SetorItem>
    {
        public void Configure(EntityTypeBuilder<SetorItem> builder)
        {
            builder.ToTable("Setor");
            //builder.HasKey(ci => new {  ci.id_Empresa, ci.id_Setor });
            builder.HasKey(ci => new {  ci.id_Setor });
        }
    }
}
