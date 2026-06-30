using Dapper;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoPorGeneroPaginado
{
    public class ObtemCatalogoPorGeneroQueryHandler : IRequestHandler<ObtemCatalogoPorGeneroQuery, PagedResult<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;

        public ObtemCatalogoPorGeneroQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<PagedResult<JogoResponse>> Handle(ObtemCatalogoPorGeneroQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.Pagina - 1) * request.TamanhoPagina;

            const string sql = @"
                SELECT COUNT(1) 
                FROM Jogos j
                WHERE j.Ativo = 1
                  AND (@Genero IS NULL OR j.Genero = @Genero);

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
                WHERE j.Ativo = 1
                  AND (@Genero IS NULL OR j.Genero = @Genero)
                ORDER BY j.DataCadastro DESC
                OFFSET @Offset ROWS 
                FETCH NEXT @TamanhoPagina ROWS ONLY;";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                Genero = request.Genero.HasValue ? (int?)request.Genero.Value : null,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });

            var totalItens = await multi.ReadFirstAsync<int>();
            var itens = await multi.ReadAsync<JogoResponse>();

            return new PagedResult<JogoResponse>(
                itens,
                totalItens,
                request.Pagina,
                request.TamanhoPagina
            );
        }
    }
}
