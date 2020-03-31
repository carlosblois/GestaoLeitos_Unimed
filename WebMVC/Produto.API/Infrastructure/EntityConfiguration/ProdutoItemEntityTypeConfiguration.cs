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
    }
}
}
