using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repositories
{
    internal class GamesRatingRepository : IGamesRatingRepository
    {
        private readonly CatalogDbContext _dbContext;

        public GamesRatingRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(GamesRating gamesRating)
        {
            _dbContext.GamesRatings.Add(gamesRating);
        }

        public async Task<GamesRating?> GetByGameAndUser(Guid gameId, Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.GamesRatings
                .FirstOrDefaultAsync(r => r.GameId == gameId && r.UserId == userId, cancellationToken);
        }

        public void Update(GamesRating gamesRating)
        {
            _dbContext.GamesRatings.Update(gamesRating);
        }
    }
}
