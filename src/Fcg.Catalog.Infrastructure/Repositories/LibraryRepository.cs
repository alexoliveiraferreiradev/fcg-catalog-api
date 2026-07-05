using Dapper;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly CatalogDbContext _dbContext;

        public LibraryRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(UserLibrary library)
        {            
            _dbContext.Libraries.Add(library);
        }

        public void Update(UserLibrary library)
        {
            _dbContext.Update(library);
        }

        public async Task<UserLibrary?> GetById(Guid id)
        {
           return await _dbContext.Libraries.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfUserOwnsGame(Guid userId, Guid gameId)
        {
            return await _dbContext.Libraries.AnyAsync(x => x.UserId == userId && x.GameId == gameId && x.IsActive);
        }


        

    }
}
