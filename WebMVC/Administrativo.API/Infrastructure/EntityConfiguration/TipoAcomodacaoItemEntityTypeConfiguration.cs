using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Administrativo.API.Model;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class TipoAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<TipoAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<TipoAcomodacaoItem> builder)
        {
            builder.ToTable("TipoAcomodacao");
            builder.HasKey(ci => new { ci.Id_TipoAcomodacao, ci.Id_Empresa });
        }
    }
}
