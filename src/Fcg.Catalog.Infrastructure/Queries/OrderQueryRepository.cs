using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Queries
{
    public class OrderQueryRepository : IOrderQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public OrderQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<PagedResult<OrderHistoryResponse>> GetOrderHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;

            const string countSql = "SELECT COUNT(1) FROM Orders WHERE UserId = @UserId;";
            var totalItems = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { UserId = userId });

            if (totalItems == 0)
            {
                return new PagedResult<OrderHistoryResponse>(Enumerable.Empty<OrderHistoryResponse>(), page, pageSize, 0);
            }

            const string ordersSql = @"
                SELECT 
                    o.Id as OrderId,
                    o.CreatedAt as OrderDate,
                    o.Status,
                    o.TotalAmount as TotalAmount
                FROM Orders o
                WHERE o.UserId = @UserId
                ORDER BY o.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var orders = (await _dbConnection.QueryAsync<OrderHistoryResponse>(ordersSql, new { UserId = userId, Offset = offset, PageSize = pageSize })).ToList();

            if (!orders.Any())
            {
                return new PagedResult<OrderHistoryResponse>(Enumerable.Empty<OrderHistoryResponse>(), page, pageSize, totalItems);
            }

            var orderIds = orders.Select(o => o.OrderId).ToList();

            const string itemsSql = @"
                SELECT 
                    og.OrderId,
                    og.GameId,
                    og.GameName,
                    og.GameAmount as PaidPrice,
                    g.BasePrice as OriginalPrice
                FROM OrderGames og
                INNER JOIN Games g ON og.GameId = g.Id
                WHERE og.OrderId IN @OrderIds;";

            var items = await _dbConnection.QueryAsync<dynamic>(itemsSql, new { OrderIds = orderIds });

            var itemsGrouped = items.GroupBy(i => (Guid)i.OrderId).ToDictionary(g => g.Key, g => g.Select(i => new OrderItemResponse
            {
                GameId = i.GameId,
                GameName = i.GameName,
                PaidPrice = i.PaidPrice,
                OriginalPrice = i.OriginalPrice
            }).ToList());

            foreach (var order in orders)
            {
                if (itemsGrouped.TryGetValue(order.OrderId, out var orderItems))
                {
                    order.Items = orderItems;
                }
            }

            return new PagedResult<OrderHistoryResponse>(orders, page, pageSize, totalItems);
        }
    }
}
