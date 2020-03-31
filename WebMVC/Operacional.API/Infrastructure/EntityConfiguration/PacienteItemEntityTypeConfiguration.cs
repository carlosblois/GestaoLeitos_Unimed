using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operacional.API.Model;


namespace Operacional.API.Infrastructure.EntityConfiguration
{
    class PacienteItemEntityTypeConfiguration
        : IEntityTypeConfiguration<PacienteItem>
    {
        public void Configure(EntityTypeBuilder<PacienteItem> builder)
        {
            builder.ToTable("Paciente");
            builder.HasKey(ci => new { ci.Id_Paciente });
        }
    }
}
