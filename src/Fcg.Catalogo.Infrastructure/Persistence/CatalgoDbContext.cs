using Fcg.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Persistence
{
    public class CatalgoDbContext : DbContext
    {
        public CatalgoDbContext(DbContextOptions<CatalgoDbContext> options) :base(options){            
        }
        public DbSet<Biblioteca> Bibliotecas  { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalgoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
