using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Infrastructure.Queries
{
    public class PromotionQueryRepository : IPromotionQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public PromotionQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<PagedResult<GamePromotionResponse>> GetPagedCatalogByPromotionsAsync(GameGenre? gameGenre, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var offset = (pageNumber - 1) * pageSize;

            const string sql = @"                            
                            SELECT COUNT(1) 
                            FROM Games g
                            WHERE g.IsActive = 1 
                              AND (@Genre IS NULL OR g.Genre = @Genre)
                              AND EXISTS (
                                  SELECT 1 FROM Promotions p 
                                  WHERE p.GameId = g.Id 
                                    AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
                              );                       
                            
                            SELECT 
                                g.Id, 
                                g.Name, 
                                g.Description, 
                                g.BasePrice AS OriginalPrice, 
                                p.ValorPromocao AS CurrentPrice, 
                                g.Genre, 
                                g.IsActive, 
                                p.Id AS PromotionId             
                            FROM Games g 
                            INNER JOIN Promotions p 
                                ON g.Id = p.GameId 
                               AND GETUTCDATE() BETWEEN p.StartDate AND p.EndDate
                            WHERE g.IsActive = 1
                              AND (@Genre IS NULL OR g.Genre = @Genre) 
                            ORDER BY g.CreatedAt DESC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @PageSize ROWS ONLY;
                        ";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                Genre = gameGenre,
                Offset = offset,
                PageSize = pageSize
            });

            var totalItens = await multi.ReadFirstAsync<int>();
            var itens = await multi.ReadAsync<GamePromotionResponse>();
            return new PagedResult<GamePromotionResponse>(itens, pageNumber, pageSize, totalItens);
        }

        public async Task<PromotionResponse> GetPromotionByIdAsync(Guid promotionId, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT 
                p.Id AS PromotionId,
                p.GameId,
                p.ValorPromocao,
                j.Name AS GameName,
                j.Description AS GameDescription,
                p.StartDate,
                p.EndDate
            FROM Promotions p
            INNER JOIN Games j ON p.GameId = j.Id
            WHERE p.Id = @PromotionId;";

            return await _dbConnection.QueryFirstOrDefaultAsync<PromotionResponse>(sql, new { PromotionId = promotionId });
        }
    }
}
