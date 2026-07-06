using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface IPromotionQueryRepository
    {
        Task<PagedResult<GamePromotionResponse>> GetPagedCatalogByPromotionsAsync(GameGenre? gameGenre, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PromotionResponse> GetPromotionByIdAsync(Guid promotionId, CancellationToken cancellationToken);
    }
}
