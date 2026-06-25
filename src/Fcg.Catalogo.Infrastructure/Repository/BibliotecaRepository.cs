using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Repository
{
    public class BibliotecaRepository : IBibliotecaRepository
    {
        private readonly CatalgoDbContext _dbContext;

        public BibliotecaRepository(CatalgoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Adicionar(Biblioteca biblioteca)
        {            
            _dbContext.Bibliotecas.Add(biblioteca);
        }

        public void Atualizar(Biblioteca biblioteca)
        {
            _dbContext.Update(biblioteca);
        }

        public async Task<Biblioteca?> ObterPorId(Guid id)
        {
           return await _dbContext.Bibliotecas.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId)
        {
            return await _dbContext.Bibliotecas.AnyAsync(x => x.UsuarioId == usuarioId && x.JogoId == jogoId && x.Ativo);
        }

      
    }
}
