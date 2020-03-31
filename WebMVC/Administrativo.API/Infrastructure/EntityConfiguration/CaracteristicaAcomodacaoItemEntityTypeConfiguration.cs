using Administrativo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Administrativo.API.Infrastructure.EntityConfiguration
{
    class CaracteristicaAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<CaracteristicaAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<CaracteristicaAcomodacaoItem> builder)
        {
            builder.ToTable("CaracteristicaAcomodacao");
            builder.HasKey(ci => new { ci.id_CaracteristicaAcomodacao });
        }
    }
}
