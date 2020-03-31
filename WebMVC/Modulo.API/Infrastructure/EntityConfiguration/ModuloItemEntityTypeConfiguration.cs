using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modulo.API.Model;



namespace Modulo.API.Infrastructure.EntityConfiguration
{
    class ModuloItemEntityTypeConfiguration
        : IEntityTypeConfiguration<ModuloItem>
    {
        public void Configure(EntityTypeBuilder<ModuloItem> builder)
        {
            builder.ToTable("Modulo");
            builder.HasKey(ci => new { ci.Id_Modulo });

        }
}
}
