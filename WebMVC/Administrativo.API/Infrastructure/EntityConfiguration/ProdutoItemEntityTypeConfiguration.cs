using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Produto.API.Model;


namespace Produto.API.Infrastructure.EntityConfiguration
{
    class ProdutoItemEntityTypeConfiguration
        : IEntityTypeConfiguration<ProdutoItem>
    {
        public void Configure(EntityTypeBuilder<ProdutoItem> builder)
        {
            builder.ToTable("Produto");

            //builder.Property(ci => ci.Id)
            //    .ForSqlServerUseSequenceHiLo("produto_hilo")
            //    .IsRequired();

            //builder.Property(ci => ci.Name)
            //    .IsRequired(true)
            //    .HasMaxLength(50);

            //builder.Property(ci => ci.Price)
            //    .IsRequired(true);

            //builder.Property(ci => ci.PictureFileName)
            //    .IsRequired(false);

            //builder.Ignore(ci => ci.PictureUri);
    }
}
}
