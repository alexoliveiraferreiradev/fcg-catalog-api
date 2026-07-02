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
            var cacheKey = $"Catalog:promocao:detalhes:{request.promocaoId}";

            var cachedPromocao = await _cacheService.GetAsync<PromocaoResponse>(cacheKey, cancellationToken);

            if (cachedPromocao != null)
            {
                return cachedPromocao;
            }


            const string sql = @"
            SELECT 
                p.Id AS PromocaoId,
                p.JogoId,
                p.ValorPromocao,
                j.Nome AS NomeJogo,
                j.Descricao AS DescricaoJogo,
                p.DataInicio,
                p.DataFim
            FROM Promocoes p
            INNER JOIN Jogos j ON p.JogoId = j.Id
            WHERE p.Id = @PromocaoId;";

            var promocaoDetalhe = await _dbConnection.QueryFirstOrDefaultAsync<PromocaoResponse>(sql, new { PromocaoId = request.promocaoId });

            if(promocaoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, promocaoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return promocaoDetalhe;
        }
    }
}

