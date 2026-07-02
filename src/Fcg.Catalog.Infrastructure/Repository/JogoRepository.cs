using Dapper;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repository
{
    public class JogoRepository : IJogoRepository
    {
        private readonly CatalogDbContext _dbContext;

        public JogoRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Adicionar(Jogo jogo)
        {
            _dbContext.Jogos.Add(jogo);
        }

        public void Atualizar(Jogo jogo)
        {
            _dbContext.Update(jogo);
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

        public async Task<IEnumerable<Jogo>> ObterJogosPorIds(IEnumerable<Guid> jogosIds)
        {
            var connection = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT  
                                j.Id, 
                                j.Nome, 
                                j.Descricao, 
                                j.PrecoBase, 
                                j.Ativo,j.DataCadastro, j.DataAlteracao, j.Genero
                                FROM Jogos j where j.Id IN @jogosIds ";
            return await connection.QueryAsync<Jogo>(sql, new { jogosIds });
        }
    }
}
