using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Query.GetOrderHistory
{
    public class GetOrderHistoryQueryHandler : IRequestHandler<GetOrderHistoryQuery, PagedResult<OrderHistoryResponse>>
    {
        private readonly ICacheService _cacheService;
        private readonly IOrderQueryRepository _orderQueryRepository;

        public GetOrderHistoryQueryHandler(IOrderQueryRepository orderQueryRepository, ICacheService cacheService)
        {
            _orderQueryRepository = orderQueryRepository;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<OrderHistoryResponse>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"order_u:{request.UserId}_p:{request.Page}_s:{request.PageSize}";
            var cachedOrderHistory = await _cacheService.GetAsync<PagedResult<OrderHistoryResponse>>(cacheKey, cancellationToken);

            if (cachedOrderHistory != null)
            {
                return cachedOrderHistory;
            }

            var pagedOrder = await _orderQueryRepository.GetOrderHistoryAsync(request.UserId, request.Page, request.PageSize, cancellationToken);

            if (pagedOrder.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedOrder, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return pagedOrder;
        }
    }
}
