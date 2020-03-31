using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modulo.API.Model;


namespace Modulo.API.Infrastructure.EntityConfiguration
{
    class EmpresaPerfilModuloItemEntityTypeConfiguration
        : IEntityTypeConfiguration<EmpresaPerfilModuloItem>
    {
        public void Configure(EntityTypeBuilder<EmpresaPerfilModuloItem> builder)
        {
            builder.ToTable("EmpresaPerfilModulo");
            builder.HasKey(ci => new { ci.Id_Empresa,ci.Id_Perfil ,ci.Id_Modulo });

        }
    }
}
