using Dapper;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>Lê do SQL os jogos no formato do índice (inclusive desativados, que ficam no índice com IsActive = false).</summary>
    internal class GameDocumentQuery : IGameDocumentQuery
    {
        // Promoção ativa em vigor ou ainda por começar; a que vence primeiro ganha, como em Game.GetCurrentPrice.
        private const string SelectGames = @"
            SELECT
                j.Id,
                j.Name,
                j.Description,
                j.Genre,
                j.CoverUrl,
                j.BasePrice AS OriginalPrice,
                promo.ValorPromocao AS PromotionPrice,
                promo.StartDate AS PromotionStartDate,
                promo.EndDate AS PromotionEndDate,
                j.IsActive,
                CASE WHEN j.RatingCount = 0 THEN NULL ELSE ROUND(j.RatingSum / j.RatingCount, 1) END AS AverageRating,
                j.RatingCount
            FROM Games j
            OUTER APPLY (
                SELECT TOP 1 p.ValorPromocao, p.StartDate, p.EndDate
                FROM Promotions p
                WHERE p.GameId = j.Id
                  AND p.IsActive = 1
                  AND p.EndDate >= GETUTCDATE()
                ORDER BY p.StartDate
            ) promo";

        private readonly IDbConnection _dbConnection;

        public GameDocumentQuery(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<GameDocument?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken) =>
            _dbConnection.QueryFirstOrDefaultAsync<GameDocument>(
                new CommandDefinition($"{SelectGames} WHERE j.Id = @GameId;", new { GameId = gameId }, cancellationToken: cancellationToken));

        public Task<IEnumerable<GameDocument>> GetAllAsync(CancellationToken cancellationToken) =>
            _dbConnection.QueryAsync<GameDocument>(new CommandDefinition($"{SelectGames};", cancellationToken: cancellationToken));
    }
}
