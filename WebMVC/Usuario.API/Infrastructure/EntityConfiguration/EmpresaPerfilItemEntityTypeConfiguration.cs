using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure.EntityConfiguration
{
    class EmpresaPerfilItemEntityTypeConfiguration
        : IEntityTypeConfiguration<EmpresaPerfilItem>
    {
        public void Configure(EntityTypeBuilder<EmpresaPerfilItem> builder)
        {
            builder.ToTable("EmpresaPerfil");
            builder.HasKey(ci => new { ci.id_Empresa, ci.id_Perfil });

        }
    }
}
