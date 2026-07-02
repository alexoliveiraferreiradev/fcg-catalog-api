using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObterPromocaoPorJogoId
{
    public class ObterPromocaoPorJogoIdQueryHandler : IRequestHandler<ObterPromocaoPorJogoIdQuery, PromocaoResponse>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public ObterPromocaoPorJogoIdQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<PromocaoResponse> Handle(ObterPromocaoPorJogoIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Catalog:Promotion:detalhes:{request.PromotionId}";

            var cachedPromocao = await _cacheService.GetAsync<PromocaoResponse>(cacheKey, cancellationToken);

            if (cachedPromocao != null)
            {
                return cachedPromocao;
            }


            const string sql = @"
            SELECT 
                p.Id AS PromotionId,
                p.GameId,
                p.ValorPromocao,
                j.Name AS NomeJogo,
                j.Description AS DescricaoJogo,
                p.StartDate,
                p.EndDate
            FROM Promotions p
            INNER JOIN Games j ON p.GameId = j.Id
            WHERE p.Id = @PromotionId;";

            var promocaoDetalhe = await _dbConnection.QueryFirstOrDefaultAsync<PromocaoResponse>(sql, new { PromotionId = request.PromotionId });

            if(promocaoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, promocaoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return promocaoDetalhe;
        }
    }
}

