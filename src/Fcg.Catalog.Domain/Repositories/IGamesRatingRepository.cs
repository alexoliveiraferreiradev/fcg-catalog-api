using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface IGamesRatingRepository
    {
        void Add(GamesRating gamesRating);
        void Update(GamesRating gamesRating);
        Task<GamesRating?> GetByGameAndUser(Guid gameId, Guid userId, CancellationToken cancellationToken);
    }
}
