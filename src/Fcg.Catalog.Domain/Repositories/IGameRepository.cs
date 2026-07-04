using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface IGameRepository
    {
        void Add(Game game);
        void Update(Game game);
        Task<bool> GameExistsWithName(string gameName);
        Task<Game?> GetById(Guid id);       
        Task<Promotion?> GetPromotionById(Guid id);        
        Task DeactivateInvalidPromotions();
        Task<IEnumerable<Game>> GetGamesByIds(IEnumerable<Guid> jogosIds);
    }
}
