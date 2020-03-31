using Microsoft.EntityFrameworkCore;
using Produto.API.Infrastructure.EntityConfiguration;
using Produto.API.Model;


namespace Produto.API.Infrastructure
{
    public class ProdutoContext :DbContext
    {
        public ProdutoContext(DbContextOptions<ProdutoContext> options) : base(options)
        {
        }
        public DbSet<ProdutoItem> ProdutoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProdutoItemEntityTypeConfiguration());
        }

    }
}
