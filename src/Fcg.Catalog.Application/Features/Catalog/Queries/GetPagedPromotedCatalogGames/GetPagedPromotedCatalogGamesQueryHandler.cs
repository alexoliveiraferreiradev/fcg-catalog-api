using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames
{
    public class GetPagedPromotedCatalogGamesQueryHandler : IRequestHandler<GetPagedPromotedCatalogGamesQuery, PagedResult<GameResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;
        public GetPagedPromotedCatalogGamesQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<GameResponse>> Handle(GetPagedPromotedCatalogGamesQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "genres";
            string p = request.OnlyPromoted.GetValueOrDefault() ? "yes" : "no";
            var cacheKey = $"catalog:pag:p{request.Page}:t{request.PageSize}:prom_{p}:g_{g}";

            var cachedGames = await _cacheService.GetAsync<PagedResult<GameResponse>>(cacheKey, cancellationToken);

            if(cachedGames != null)
            {
                return cachedGames;
            }

            var offset = (request.Page - 1) * request.PageSize;

            const string sql = @"
                SELECT COUNT(1) 
                FROM Games j
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre)
                  AND EXISTS (
                      SELECT 1 FROM Promotions p 
                      WHERE p.GameId = j.Id 
                        AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
                  );

                SELECT 
                    j.Id,
                    j.Name,    
                    j.Description,
                    j.BasePrice AS OriginalPrice,
                    (SELECT TOP 1 p.ValorPromocao 
                     FROM Promotions p 
                     WHERE p.GameId = j.Id 
                       AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate) AS CurrentPrice,
                    j.Genre,
                    j.IsActive
                FROM Games j
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre)
                  AND EXISTS (
                      SELECT 1 FROM Promotions p 
                      WHERE p.GameId = j.Id 
                        AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
                  )
                ORDER BY j.CreatedAt DESC
                OFFSET @Offset ROWS 
                FETCH NEXT @PageSize ROWS ONLY;";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                Genre = request.Genre.HasValue ? (int?)request.Genre.Value : null,
                Offset = offset,
                PageSize = request.PageSize
            });

            var totalItens = await multi.ReadFirstAsync<int>();
            var itens = await multi.ReadAsync<GameResponse>();

            var pagedCatalog = new PagedResult<GameResponse>(itens, request.Page, request.PageSize, totalItens);


            if (pagedCatalog.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return pagedCatalog;
        }
    }
}
