using Administrativo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class AcessoEmpresaPerfilTSTAItemEntityTypeConfiguration
        : IEntityTypeConfiguration<AcessoEmpresaPerfilTSTAItem>
    {
        public void Configure(EntityTypeBuilder<AcessoEmpresaPerfilTSTAItem> builder)
        {
            builder.ToTable("AcessoEmpresaPerfilTipoSituacaoTipoAtividade");
            builder.HasKey(ci => new { ci.Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade });

        }
    }
}
