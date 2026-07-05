using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Queries
{
    public class GameQueryRepository : IGameQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public GameQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<GameResponse>> GetAllGamesAsync(CancellationToken cancellationToken)
        {
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

            return await _dbConnection.QueryAsync<GameResponse>(sql);
        }

        public async Task<GameResponse?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken)
        {
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

            return await _dbConnection.QueryFirstOrDefaultAsync<GameResponse>(sql, new { GameId = gameId });
        }

        public async Task<PagedResult<GameUserResponse>> GetPagedCatalogAsync(GameGenre? gameGenre, bool? promoted, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var offset = (pageNumber - 1) * pageSize;
            var onlyPromoted = promoted ?? false;
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
                Genre = gameGenre,
                OnlyPromoted = onlyPromoted ? 1 : 0,
                Offset = offset,
                PageSize = pageSize
            });


            var totalItems = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<GameUserResponse>();


            var pagedCatalog = new PagedResult<GameUserResponse>(items, pageNumber, pageSize, totalItems);

            return pagedCatalog;
        }

       
    }
}
