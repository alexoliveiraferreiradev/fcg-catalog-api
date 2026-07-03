using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface IGameRepository
    {
        void Add(Game Game);
        void Update(Game Game);
        Task<bool> GameExistsWithName(string GameName);
        Task<Game?> GetById(Guid id);       
        Task<Promotion?> GetPromotionById(Guid id);        
        Task DeactivateInvalidPromotions();
        Task<IEnumerable<Game>> GetGamesByIds(IEnumerable<Guid> jogosIds);
    }
}
