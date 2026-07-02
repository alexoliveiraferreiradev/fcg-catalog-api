using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetGameById
{
    public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GameResponse>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public GetGameByIdQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<GameResponse> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Catalog:Game:detalhes:{request.GameId}";

            var cachedJogo = await _cacheService.GetAsync<GameResponse>(cacheKey,cancellationToken);

            if(cachedJogo != null)
            {
                return cachedJogo;
            }
            
            const string sql = @"
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
            WHERE j.Id = @GameId;";

            var jogoDetalhe = await _dbConnection.QueryFirstOrDefaultAsync<GameResponse>(sql, new { GameId = request.GameId });

            if (jogoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, jogoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return jogoDetalhe;
        }
    }
}
