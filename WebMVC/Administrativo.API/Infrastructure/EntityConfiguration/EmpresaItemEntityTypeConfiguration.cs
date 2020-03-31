using Administrativo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class EmpresaItemEntityTypeConfiguration
        : IEntityTypeConfiguration<EmpresaItem>
    {
        public void Configure(EntityTypeBuilder<EmpresaItem> builder)
        {
            builder.ToTable("Empresa");
            builder.HasKey(ci => new { ci.id_Empresa });
        }
    }
}
