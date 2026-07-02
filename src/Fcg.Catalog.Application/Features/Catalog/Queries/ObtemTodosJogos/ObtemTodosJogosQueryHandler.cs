using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObtemTodosJogos
{
    public class ObtemTodosJogosQueryHandler : IRequestHandler<ObtemTodosJogosQuery, IEnumerable<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public ObtemTodosJogosQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<JogoResponse>> Handle(ObtemTodosJogosQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "Catalog:todos";

            var cachedCatalog = await _cacheService.GetAsync<IEnumerable<JogoResponse>>(cacheKey,cancellationToken);

            if (cachedCatalog != null) {
                return cachedCatalog;
            }

            const string sql = @"
            SELECT 
                j.Id, 
                j.Nome, 
                j.Descricao, 
                j.PrecoBase as PrecoOriginal,             
                    COALESCE(
                        (SELECT TOP 1 p.ValorPromocao 
                        FROM Promocoes p 
                        WHERE p.JogoId = j.Id 
                        AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim), 
                        j.PrecoBase
                    ) as PrecoAtual,
                j.Ativo,
                j.Genero
            FROM Jogos j ";

            var Catalog = await _dbConnection.QueryAsync<JogoResponse>(sql);

            if (Catalog != null)
            {
                await _cacheService.SetAsync(cacheKey, Catalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return Catalog;
        }
    }
}
