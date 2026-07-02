using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames
{
    public class ObtemCatalogJogosPromovidosQueryHandler : IRequestHandler<ObtemCatalogJogosPromovidosQuery, PagedResult<GameResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;
        public ObtemCatalogJogosPromovidosQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<GameResponse>> Handle(ObtemCatalogJogosPromovidosQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "todos";
            string p = request.OnlyPromoted.GetValueOrDefault() ? "sim" : "nao";
            var cacheKey = $"Catalog:pag:p{request.Page}:t{request.TamanhoPagina}:prom_{p}:g_{g}";

            var jogosEmCache = await _cacheService.GetAsync<PagedResult<GameResponse>>(cacheKey, cancellationToken);

            if(jogosEmCache != null)
            {
                return jogosEmCache;
            }

            var offset = (request.Page - 1) * request.TamanhoPagina;

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
                FETCH NEXT @TamanhoPagina ROWS ONLY;";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                Genre = request.Genre.HasValue ? (int?)request.Genre.Value : null,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });

            var totalItens = await multi.ReadFirstAsync<int>();
            var itens = await multi.ReadAsync<GameResponse>();

            var CatalogPaginado = new PagedResult<GameResponse>(
                itens,
                totalItens,
                request.Page,
                request.TamanhoPagina
            );


            if (CatalogPaginado.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, CatalogPaginado, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return CatalogPaginado;
        }
    }
}
