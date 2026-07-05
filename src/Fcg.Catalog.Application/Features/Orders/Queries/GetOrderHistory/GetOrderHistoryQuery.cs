using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Query.GetOrderHistory
{
    public record GetOrderHistoryQuery(Guid UserId, int Page =1, int PageSize = 10) : IRequest<PagedResult<OrderHistoryResponse>>;
}
