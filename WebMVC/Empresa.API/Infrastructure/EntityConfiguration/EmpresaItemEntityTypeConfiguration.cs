using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Empresa.API.Model;


namespace Empresa.API.Infrastructure.EntityConfiguration
{
    class EmpresaItemEntityTypeConfiguration
        : IEntityTypeConfiguration<EmpresaItem>
    {
        public void Configure(EntityTypeBuilder<EmpresaItem> builder)
        {
            builder.ToTable("Empresa");
            builder.HasKey(ci => ci.Id_Empresa);
        }
}
}
