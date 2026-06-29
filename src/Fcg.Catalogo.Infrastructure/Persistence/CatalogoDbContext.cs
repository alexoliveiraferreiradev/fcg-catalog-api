using Fcg.Catalogo.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Persistence
{
    public class CatalogoDbContext : DbContext
    {
        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) :base(options){            
        }
        public DbSet<Biblioteca> Bibliotecas  { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Promocao> Promocoes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
