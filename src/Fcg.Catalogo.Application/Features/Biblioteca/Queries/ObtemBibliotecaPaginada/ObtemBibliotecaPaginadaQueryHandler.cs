using Dapper;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada
{
    public class ObtemBibliotecaPaginadaQueryHandler : IRequestHandler<ObtemBibliotecaPaginadaQuery, PagedResult<BibliotecaItemResponse>>
    {
        private readonly IDbConnection _dbConnection;
        public ObtemBibliotecaPaginadaQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;   
        }
        public async Task<PagedResult<BibliotecaItemResponse>> Handle(ObtemBibliotecaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.Pagina - 1) * request.TamanhoPagina;

            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            const string sql = @"            
            SELECT COUNT(1) 
            FROM Bibliotecas 
            WHERE UsuarioId = @UsuarioId;
            
            SELECT 
                b.JogoId AS JogoId,
                j.Nome AS NomeJogo,
                j.Descricao as Descricao,    
                j.Genero AS Genero,
                b.DataCadastro AS DataAquisicao
            FROM Bibliotecas b
            INNER JOIN Jogos j ON b.JogoId = j.Id
            WHERE b.UsuarioId = @UsuarioId AND b.Ativo = 1
            ORDER BY b.DataCadastro DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @TamanhoPagina ROWS ONLY;";

            
            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                UsuarioId = request.UsuarioId,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });

            
            var totalItems = await multi.ReadFirstOrDefaultAsync<int>();

            
            var items = await multi.ReadAsync<BibliotecaItemResponse>();

            
            return new PagedResult<BibliotecaItemResponse>(
                items,
                totalItems,
                request.Pagina,
                request.TamanhoPagina);
        }
    }
}
