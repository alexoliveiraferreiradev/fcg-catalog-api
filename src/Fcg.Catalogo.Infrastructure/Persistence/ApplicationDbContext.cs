using Fcg.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options){            
        }
        public DbSet<Biblioteca> Bibliotecas  { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
