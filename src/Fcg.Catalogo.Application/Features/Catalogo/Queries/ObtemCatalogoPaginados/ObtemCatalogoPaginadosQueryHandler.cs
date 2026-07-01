using Dapper;
using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoPaginados
{
    public class ObtemCatalogoPaginadosQueryHandler : IRequestHandler<ObtemCatalogoPaginadosQuery, PagedResult<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;

        public ObtemCatalogoPaginadosQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<JogoResponse>> Handle(ObtemCatalogoPaginadosQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genero.HasValue ? request.Genero.Value.ToString() : "todos";
            string p = request.ApenasPromovidos.GetValueOrDefault() ? "sim" : "nao";

            var cacheKey = $"catalogo:pag:p{request.Pagina}:t{request.TamanhoPagina}:g_{g}:prom_{p}";

            var catalogCache = await _cacheService.GetAsync<PagedResult<JogoResponse>>(cacheKey, cancellationToken);

            var offset = (request.Pagina - 1) * request.TamanhoPagina;
            var apenasPromovidos = request.ApenasPromovidos ?? false;
            
            const string sql = @"            
            SELECT COUNT(1) 
            FROM Jogos j
            WHERE (@Genero IS NULL OR j.Genero = @Genero)
              AND (j.Ativo = 1)
              AND (@ApenasPromovidos = 0 OR EXISTS (
                  SELECT 1 FROM Promocoes p 
                  WHERE p.JogoId = j.Id 
                    AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim
              ));

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
            WHERE (@Genero IS NULL OR j.Genero = @Genero)
              AND (j.Ativo = 1)
              AND (@ApenasPromovidos = 0 OR EXISTS (
                  SELECT 1 FROM Promocoes p 
                  WHERE p.JogoId = j.Id 
                    AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim
              ))
            ORDER BY j.DataCadastro DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @TamanhoPagina ROWS ONLY;";


            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {                
                Genero = request.Genero.HasValue ? (int?)request.Genero.Value : null,
                ApenasPromovidos = apenasPromovidos ? 1 : 0,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });


            var totalItems = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<JogoResponse>();


            var catalogoPaginado = new PagedResult<JogoResponse>(
                items,
                totalItems,
                request.Pagina,
                request.TamanhoPagina);

            if(catalogoPaginado.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, catalogoPaginado, TimeSpan.FromMinutes(5), cancellationToken);
            }


            return catalogoPaginado;
        }
    }
}
