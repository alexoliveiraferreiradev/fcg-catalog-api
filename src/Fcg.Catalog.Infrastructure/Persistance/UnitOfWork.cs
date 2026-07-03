using Fcg.Core.Abstractions.Interfaces;

namespace Fcg.Catalog.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _dbContext;

        public UnitOfWork(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
