using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class PacienteAcomodacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<PacienteAcomodacaoItem>
    {
        public void Configure(EntityTypeBuilder<PacienteAcomodacaoItem> builder)
        {
            builder.ToTable("PacienteAcomodacao");
            builder.HasKey(ci => new { ci.Id_PacienteAcomodacao });
        }
    }
}
