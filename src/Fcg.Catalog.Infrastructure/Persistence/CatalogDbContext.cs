using Fcg.Catalog.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Persistence
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) :base(options){            
        }
        public DbSet<Library> Libraries  { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
