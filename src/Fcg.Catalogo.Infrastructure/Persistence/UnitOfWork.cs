using Fcg.Catalogo.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
