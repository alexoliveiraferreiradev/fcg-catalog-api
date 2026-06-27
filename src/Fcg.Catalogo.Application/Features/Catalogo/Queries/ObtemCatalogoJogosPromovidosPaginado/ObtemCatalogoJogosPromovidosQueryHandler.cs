using Dapper;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoJogosPromovidosPaginado
{
    public class ObtemCatalogoJogosPromovidosQueryHandler : IRequestHandler<ObtemCatalogoJogosPromovidosQuery, PagedResult<JogosResponse>>
    {
        private readonly IDbConnection _dbConnection;
        public ObtemCatalogoJogosPromovidosQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<PagedResult<JogosResponse>> Handle(ObtemCatalogoJogosPromovidosQuery request, CancellationToken cancellationToken)
        {
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
            var itens = await multi.ReadAsync<JogosResponse>();

            return new PagedResult<JogosResponse>(
                itens,
                totalItens,
                request.Pagina,
                request.TamanhoPagina
            );
        }
    }
}
