using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuario.API.Model;


namespace Usuario.API.Infrastructure.EntityConfiguration
{
    class LogAcessoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<LogAcessoItem>
    {
        public void Configure(EntityTypeBuilder<LogAcessoItem> builder)
        {
            builder.ToTable("LogAcesso");
            builder.HasKey(ci => new { ci.id_Log });

        }
    }
}
