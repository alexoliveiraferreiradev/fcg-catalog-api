using Dapper;
using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada
{
    public class ObtemBibliotecaPaginadaQueryHandler : IRequestHandler<ObtemBibliotecaPaginadaQuery, PagedResult<BibliotecaItemResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;
        public ObtemBibliotecaPaginadaQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<BibliotecaItemResponse>> Handle(ObtemBibliotecaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var cachaKey= $"biblioteca:u_{request.UsuarioId}:p{request.Pagina}:t{request.TamanhoPagina}";
            var bibliotecaEmCache = await _cacheService.GetAsync<PagedResult<BibliotecaItemResponse>>(cachaKey, cancellationToken);
            if (bibliotecaEmCache != null)
            {
                return bibliotecaEmCache;
            }

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

            var bibliotecaPaginada = new PagedResult<BibliotecaItemResponse>(
                items,
                request.Pagina,
                request.TamanhoPagina,
                totalItems);
            
            if(bibliotecaPaginada.Items.Any())
            {
                await _cacheService.SetAsync(cachaKey, bibliotecaPaginada, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return bibliotecaPaginada;
        }
    }
}
