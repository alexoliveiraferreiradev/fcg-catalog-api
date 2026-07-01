using Dapper;
using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoJogosPromovidosPaginado
{
    public class ObtemCatalogoJogosPromovidosQueryHandler : IRequestHandler<ObtemCatalogoJogosPromovidosQuery, PagedResult<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;
        public ObtemCatalogoJogosPromovidosQueryHandler(IDbConnection dbConnection,
            ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<JogoResponse>> Handle(ObtemCatalogoJogosPromovidosQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genero.HasValue ? request.Genero.Value.ToString() : "todos";
            string p = request.ApenasPromovidos.GetValueOrDefault() ? "sim" : "nao";
            var cacheKey = $"catalogo:pag:p{request.Pagina}:t{request.TamanhoPagina}:prom_{p}:g_{g}";

            var jogosEmCache = await _cacheService.GetAsync<PagedResult<JogoResponse>>(cacheKey, cancellationToken);

            if(jogosEmCache != null)
            {
                return jogosEmCache;
            }

            var offset = (request.Pagina - 1) * request.TamanhoPagina;

            const string sql = @"
                SELECT COUNT(1) 
                FROM Jogos j
                WHERE j.Ativo = 1
                  AND (@Genero IS NULL OR j.Genero = @Genero)
                  AND EXISTS (
                      SELECT 1 FROM Promocoes p 
                      WHERE p.JogoId = j.Id 
                        AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim
                  );

                SELECT 
                    j.Id,
                    j.Nome,    
                    j.Descricao,
                    j.PrecoBase AS PrecoOriginal,
                    (SELECT TOP 1 p.ValorPromocao 
                     FROM Promocoes p 
                     WHERE p.JogoId = j.Id 
                       AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim) AS PrecoAtual,
                    j.Genero,
                    j.Ativo
                FROM Jogos j
                WHERE j.Ativo = 1
                  AND (@Genero IS NULL OR j.Genero = @Genero)
                  AND EXISTS (
                      SELECT 1 FROM Promocoes p 
                      WHERE p.JogoId = j.Id 
                        AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim
                  )
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

            var catalogoPaginado = new PagedResult<JogoResponse>(
                itens,
                totalItens,
                request.Pagina,
                request.TamanhoPagina
            );


            if (catalogoPaginado.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, catalogoPaginado, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return catalogoPaginado;
        }
    }
}
