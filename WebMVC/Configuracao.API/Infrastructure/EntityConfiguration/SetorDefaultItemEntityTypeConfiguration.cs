using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setor.API.Model;


namespace Setor.API.Infrastructure.EntityConfiguration
{
    class SetorDefaultItemEntityTypeConfiguration
        : IEntityTypeConfiguration<SetorDefaultItem>
    {
        public void Configure(EntityTypeBuilder<SetorDefaultItem> builder)
        {
            builder.ToTable("DefaultSetor");
            builder.HasKey(ci => new { ci.Id_Setor });
        }
    }
}

