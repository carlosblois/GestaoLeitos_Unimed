using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure.EntityConfiguration
{
    class UsuarioItemEntityTypeConfiguration
        : IEntityTypeConfiguration<UsuarioItem>
    {
        public void Configure(EntityTypeBuilder<UsuarioItem> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(ci => new { ci.id_Usuario });

        }
}
}
