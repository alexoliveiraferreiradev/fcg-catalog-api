using Dapper;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly CatalogDbContext _dbContext;

        public GameRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Game Game)
        {
            _dbContext.Games.Add(Game);
        }

        public void Update(Game Game)
        {
            _dbContext.Update(Game);
        }

        public async Task DeactivateInvalidPromotions()
        {
            var agora = DateTime.UtcNow;
            await _dbContext.Games
                .SelectMany(j => j.Promotions)
                .Where(p => p.IsActive && p.Period.EndDate <= agora)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, false).SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
        }

        public async Task<bool> GameExistsWithName(string GameName)
        {
            return await _dbContext.Games.AnyAsync(x=>x.Name.Value.ToUpper().Equals(GameName.ToUpper()));   
        }

        public async Task<Game?> GetById(Guid id)
        {
            return await _dbContext.Games.Include(x => x.Promotions).Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Promotion?> GetPromotionById(Guid id)
        {
            var Game = await _dbContext.Games.Include(j => j.Promotions)
                .FirstOrDefaultAsync(j => j.Promotions.Any(p => p.Id == id));

            return Game?.Promotions.FirstOrDefault(p => p.Id == id);
        }

        public async Task<IEnumerable<Game>> GetGamesByIds(IEnumerable<Guid> jogosIds)
        {
            var connection = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT  
                                j.Id, 
                                j.Name, 
                                j.Description, 
                                j.BasePrice, 
                                j.IsActive,j.CreatedAt, j.UpdatedAt, j.Genre
                                FROM Games j where j.Id IN @jogosIds ";
            return await connection.QueryAsync<Game>(sql, new { jogosIds });
        }
    }
}
