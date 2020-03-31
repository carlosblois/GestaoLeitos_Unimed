using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure.EntityConfiguration
{
    class PerfilItemEntityTypeConfiguration
        : IEntityTypeConfiguration<PerfilItem>
    {
        public void Configure(EntityTypeBuilder<PerfilItem> builder)
        {
            builder.ToTable("Perfil");
            builder.HasKey(ci => new {ci.id_Perfil });

        }
    }
}
