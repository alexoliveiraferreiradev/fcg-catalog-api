using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalogByGenre
{
    public class GetPagedCatalogByGenreQueryHandler : IRequestHandler<GetPagedCatalogByGenreQuery, PagedResult<GameResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public GetPagedCatalogByGenreQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<GameResponse>> Handle(GetPagedCatalogByGenreQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "todos";
            
            var cacheKey = $"Catalog:pag:p{request.Page}:t{request.PageSize}:g_{g}";
            
            var cachedCatalog = await _cacheService.GetAsync<PagedResult<GameResponse>>(cacheKey,cancellationToken);

            if (cachedCatalog != null)
            {
                return cachedCatalog;
            }

            var offset = (request.Page - 1) * request.PageSize;

            const string sql = @"
                SELECT COUNT(1) 
                FROM Games j
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre);

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
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre)
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


            var pagedCatalog = new PagedResult<GameResponse>(
                itens,
                totalItens,
                request.Page,
                request.PageSize
            );


            if (pagedCatalog.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return pagedCatalog;
        }
    }
}
