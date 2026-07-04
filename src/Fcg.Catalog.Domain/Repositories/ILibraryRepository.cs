using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface ILibraryRepository
    {
        void Add(UserLibrary library);
        void Update(UserLibrary library);
        Task<UserLibrary?> GetById(Guid id);
        Task<bool> CheckIfUserOwnsGame(Guid userId, Guid gameId);
        Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid userId);

    }
}
