using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface IOrderQueryRepository
    {
        Task<PagedResult<OrderHistoryResponse>> GetOrderHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    }
}
