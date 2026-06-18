using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Repository
{
    public class BibliotecaRepository : IBibliotecaRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BibliotecaRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Adicionar(Biblioteca biblioteca)
        {
            _dbContext.Bibliotecas.Add(biblioteca);
            await SaveChanges();
        }

        public async Task Atualizar(Biblioteca biblioteca)
        {
            _dbContext.Update(biblioteca);
            await SaveChanges();
        }

        public async Task<Biblioteca?> ObterPorId(Guid id)
        {
           return await _dbContext.Bibliotecas.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId)
        {
            return await _dbContext.Bibliotecas.AnyAsync(x => x.UsuarioId == usuarioId && x.JogoId == jogoId && x.Ativo);
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
