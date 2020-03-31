using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure.EntityConfiguration
{
    class UsuarioEmpresaPerfilItemEntityTypeConfiguration
        : IEntityTypeConfiguration<UsuarioEmpresaPerfilItem>
    {
        public void Configure(EntityTypeBuilder<UsuarioEmpresaPerfilItem> builder)
        {
            builder.ToTable("UsuarioEmpresaPerfil");
            builder.HasKey(ci => new { ci.id_Empresa, ci.id_Usuario,ci.id_Perfil  });

        }
    }
}
