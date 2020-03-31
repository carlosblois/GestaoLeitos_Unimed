using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Configuracao.API.Model;


namespace Configuracao.API.Infrastructure.EntityConfiguration
{
    class SLASituacaoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SLASituacaoItem>
    {
        public void Configure(EntityTypeBuilder<SLASituacaoItem> builder)
        {
            builder.ToTable("SLASituacao");
            builder.HasKey(ci => new { ci.Id_SLA });
        }
    }
}
