using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogPorGeneroPaginado
{
    public class ObtemCatalogPorGeneroQueryHandler : IRequestHandler<ObtemCatalogPorGeneroQuery, PagedResult<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public ObtemCatalogPorGeneroQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<JogoResponse>> Handle(ObtemCatalogPorGeneroQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "todos";
            
            var cacheKey = $"Catalog:pag:p{request.Pagina}:t{request.TamanhoPagina}:g_{g}";
            
            var CatalogCache = await _cacheService.GetAsync<PagedResult<JogoResponse>>(cacheKey,cancellationToken);

            if (CatalogCache != null)
            {
                return CatalogCache;
            }

            var offset = (request.Pagina - 1) * request.TamanhoPagina;

            const string sql = @"
                SELECT COUNT(1) 
                FROM Games j
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre);

                SELECT 
                    j.Id,
                    j.Name,    
                    j.Description,
                    j.BasePrice AS PrecoOriginal,
                    COALESCE(
                        (SELECT TOP 1 p.ValorPromocao 
                         FROM Promotions p 
                         WHERE p.GameId = j.Id 
                           AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate), 
                        j.BasePrice
                    ) AS PrecoAtual,
                    j.Genre,
                    j.IsActive
                FROM Games j
                WHERE j.IsActive = 1
                  AND (@Genre IS NULL OR j.Genre = @Genre)
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
            var itens = await multi.ReadAsync<JogoResponse>();


            var CatalogPaginado = new PagedResult<JogoResponse>(
                itens,
                totalItens,
                request.Pagina,
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
