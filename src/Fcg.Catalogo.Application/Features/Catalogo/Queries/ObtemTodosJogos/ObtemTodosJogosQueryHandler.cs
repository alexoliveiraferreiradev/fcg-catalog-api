using Dapper;
using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemTodosJogos
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
            var cacheKey = "catalogo:todos";

            var cachedCatalogo = await _cacheService.GetAsync<IEnumerable<JogoResponse>>(cacheKey,cancellationToken);

            if (cachedCatalogo != null) {
                return cachedCatalogo;
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

            var catalogo = await _dbConnection.QueryAsync<JogoResponse>(sql);

            if (catalogo != null)
            {
                await _cacheService.SetAsync(cacheKey, catalogo, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return catalogo;
        }
    }
}
