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
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "todos";
            string p = request.OnlyPromoted.GetValueOrDefault() ? "sim" : "nao";

            var cacheKey = $"Catalog:pag:p{request.Page}:t{request.TamanhoPagina}:g_{g}:prom_{p}";

            var catalogCache = await _cacheService.GetAsync<PagedResult<GameResponse>>(cacheKey, cancellationToken);

            if (catalogCache != null)
            {
                return catalogCache;
            }

            var offset = (request.Page - 1) * request.TamanhoPagina;
            var OnlyPromoted = request.OnlyPromoted ?? false;
            
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
            FETCH NEXT @TamanhoPagina ROWS ONLY;";


            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {                
                Genre = request.Genre.HasValue ? (int?)request.Genre.Value : null,
                OnlyPromoted = OnlyPromoted ? 1 : 0,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });


            var totalItems = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<GameResponse>();


            var CatalogPaginado = new PagedResult<GameResponse>(
                items,
                totalItems,
                request.Page,
                request.TamanhoPagina);

            if(CatalogPaginado.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, CatalogPaginado, TimeSpan.FromMinutes(5), cancellationToken);
            }


            return CatalogPaginado;
        }
    }
}
