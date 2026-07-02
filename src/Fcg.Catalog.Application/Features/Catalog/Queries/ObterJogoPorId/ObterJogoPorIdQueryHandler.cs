using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObterJogoPorId
{
    public class ObterJogoPorIdQueryHandler : IRequestHandler<ObterJogoPorIdQuery, JogoResponse>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public ObterJogoPorIdQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<JogoResponse> Handle(ObterJogoPorIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Catalog:jogo:detalhes:{request.jogoId}";

            var cachedJogo = await _cacheService.GetAsync<JogoResponse>(cacheKey,cancellationToken);

            if(cachedJogo != null)
            {
                return cachedJogo;
            }
            
            const string sql = @"
                SELECT 
                     j.Id,
                     j.Nome,    
                     j.Descricao,
                     j.PrecoBase AS PrecoOriginal,
                     COALESCE(
                         (SELECT TOP 1 p.ValorPromocao 
                          FROM Promocoes p 
                          WHERE p.JogoId = j.Id 
                            AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim), 
                         j.PrecoBase
                     ) AS PrecoAtual,
                     j.Genero,
                     j.Ativo
            FROM Jogos j
            WHERE j.Id = @JogoId;";

            var jogoDetalhe = await _dbConnection.QueryFirstOrDefaultAsync<JogoResponse>(sql, new { JogoId = request.jogoId });

            if (jogoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, jogoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return jogoDetalhe;
        }
    }
}
