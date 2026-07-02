using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface ILibraryRepository
    {
        void Add(Library Library);
        void Update(Library Library);
        Task<Library?> GetById(Guid id);
        Task<bool> CheckIfUserOwnsGame(Guid UserId, Guid GameId);
        Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid UserId);

    }
}
