using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalog
{
    public class GetPagedCatalogQueryHandler : IRequestHandler<GetPagedCatalogQuery, PagedResult<GameResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public GetPagedCatalogQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<GameResponse>> Handle(GetPagedCatalogQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "genres";
            string p = request.OnlyPromoted.GetValueOrDefault() ? "yes" : "no";

            var cacheKey = $"catalog:pag:p{request.Page}:t{request.PageSize}:g_{g}:prom_{p}";

            var cachedCatalog = await _cacheService.GetAsync<PagedResult<GameResponse>>(cacheKey, cancellationToken);

            if (cachedCatalog != null)
            {
                return cachedCatalog;
            }

            var offset = (request.Page - 1) * request.PageSize;
            var onlyPromoted = request.OnlyPromoted ?? false;
            
            const string sql = @"            
            SELECT COUNT(1) 
            FROM Games j
            WHERE (@Genre IS NULL OR j.Genre = @Genre)
              AND (j.IsActive = 1)
              AND (@OnlyPromoted = 0 OR EXISTS (
                  SELECT 1 FROM Promotions p 
                  WHERE p.GameId = j.Id 
                    AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
              ));

            SELECT 
                j.Id,
                j.Name,    
                j.Description,
                j.BasePrice AS OriginalPrice,
                COALESCE(
                    (SELECT TOP 1 p.ValorPromocao 
                     FROM Promotions p 
                     WHERE p.GameId = j.Id 
                       AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate), 
                    j.BasePrice
                ) AS CurrentPrice,
                j.Genre,
                j.IsActive
            FROM Games j
            WHERE (@Genre IS NULL OR j.Genre = @Genre)
              AND (j.IsActive = 1)
              AND (@OnlyPromoted = 0 OR EXISTS (
                  SELECT 1 FROM Promotions p 
                  WHERE p.GameId = j.Id 
                    AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
              ))
            ORDER BY j.CreatedAt DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY;";


            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {                
                Genre = request.Genre.HasValue ? (int?)request.Genre.Value : null,
                OnlyPromoted = onlyPromoted ? 1 : 0,
                Offset = offset,
                PageSize = request.PageSize
            });


            var totalItems = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<GameResponse>();


            var pagedCatalog = new PagedResult<GameResponse>(items,request.Page, request.PageSize,totalItems);

            if(pagedCatalog.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }


            return pagedCatalog;
        }
    }
}
