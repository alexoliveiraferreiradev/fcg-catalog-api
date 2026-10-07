using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface IPromotionQueryRepository
    {
        Task<IEnumerable<GamePromotionResponse>> GetAllPromotionGames();    
        Task<PagedResult<GamePromotionResponse>> GetPagedCatalogByPromotionsAsync(GameGenre? gameGenre, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PromotionResponse> GetPromotionByIdAsync(Guid promotionId, CancellationToken cancellationToken);
    }
}
