using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface ILibraryRepository
    {
        void Add(UserLibrary Library);
        void Update(UserLibrary Library);
        Task<UserLibrary?> GetById(Guid id);
        Task<bool> CheckIfUserOwnsGame(Guid UserId, Guid GameId);
        Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid UserId);

    }
}
