using Fcg.Core.Abstractions.Interfaces;

namespace Fcg.Catalogo.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalgoDbContext _dbContext;

        public UnitOfWork(CatalgoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
