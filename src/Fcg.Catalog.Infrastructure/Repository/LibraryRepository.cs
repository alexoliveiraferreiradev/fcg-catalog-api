using Dapper;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repository
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly CatalogDbContext _dbContext;

        public LibraryRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(UserLibrary Library)
        {            
            _dbContext.Libraries.Add(Library);
        }

        public void Update(UserLibrary Library)
        {
            _dbContext.Update(Library);
        }

        public async Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid UserId)
        {
            var connecetion = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT GameId FROM Libraries WHERE UserId = @UserId AND IsActive = 1";
            return await connecetion.QueryAsync<Guid>(sql, new { UserId = UserId });
        }

        public async Task<UserLibrary?> GetById(Guid id)
        {
           return await _dbContext.Libraries.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfUserOwnsGame(Guid UserId, Guid GameId)
        {
            return await _dbContext.Libraries.AnyAsync(x => x.UserId == UserId && x.GameId == GameId && x.IsActive);
        }


        

    }
}
