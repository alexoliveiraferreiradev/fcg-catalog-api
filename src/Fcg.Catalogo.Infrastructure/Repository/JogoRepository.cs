using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Catalogo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Repository
{
    public class JogoRepository : IJogoRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public JogoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Adicionar(Jogo jogo)
        {
            _dbContext.Jogos.Add(jogo);
            await SaveChanges();
        }

        public async Task Atualizar(Jogo jogo)
        {
            _dbContext.Update(jogo);
            await SaveChanges();
        }

        public async Task DesativaPromocoesInvalidas()
        {
            var agora = DateTime.UtcNow;
            await _dbContext.Jogos
                .SelectMany(j => j.Promocoes)
                .Where(p => p.Ativo && p.Periodo.DataFim <= agora)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Ativo, false).SetProperty(p => p.DataAlteracao, DateTime.UtcNow));
        }

        public async Task<bool> ExisteJogoComNome(string nomeJogo)
        {
            return await _dbContext.Jogos.AnyAsync(x=>x.Nome.Valor.ToUpper().Equals(nomeJogo.ToUpper()));   
        }

        public async Task<Jogo?> ObterPorId(Guid id)
        {
            return await _dbContext.Jogos.Include(x => x.Promocoes).Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Promocao?> ObterPromocaoPorId(Guid id)
        {
            var jogo = await _dbContext.Jogos.Include(j => j.Promocoes)
                .FirstOrDefaultAsync(j => j.Promocoes.Any(p => p.Id == id));

            return jogo?.Promocoes.FirstOrDefault(p => p.Id == id);
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();    
        }
    }
}
