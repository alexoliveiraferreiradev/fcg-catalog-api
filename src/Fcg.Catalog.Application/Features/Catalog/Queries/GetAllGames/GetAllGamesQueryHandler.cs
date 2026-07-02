using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetAllGames
{
    public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, IEnumerable<GameResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public GetAllGamesQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<GameResponse>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "Catalog:todos";

            var cachedCatalog = await _cacheService.GetAsync<IEnumerable<GameResponse>>(cacheKey,cancellationToken);

            if (cachedCatalog != null) {
                return cachedCatalog;
            }

            const string sql = @"
            SELECT 
                j.Id, 
                j.Name, 
                j.Description, 
                j.BasePrice as OriginalPrice,             
                    COALESCE(
                        (SELECT TOP 1 p.ValorPromocao 
                        FROM Promotions p 
                        WHERE p.GameId = j.Id 
                        AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate), 
                        j.BasePrice
                    ) as CurrentPrice,
                j.IsActive,
                j.Genre
            FROM Games j ";

            var Catalog = await _dbConnection.QueryAsync<GameResponse>(sql);

            if (Catalog != null)
            {
                await _cacheService.SetAsync(cacheKey, Catalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return Catalog;
        }
    }
}
