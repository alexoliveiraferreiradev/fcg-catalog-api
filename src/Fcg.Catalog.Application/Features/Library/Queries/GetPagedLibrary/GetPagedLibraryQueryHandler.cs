using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary
{
    public class GetPagedLibraryQueryHandler : IRequestHandler<GetPagedLibraryQuery, PagedResult<BibliotecaItemResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICacheService _cacheService;
        public GetPagedLibraryQueryHandler(IDbConnection dbConnection, ICacheService cacheService)
        {
            _dbConnection = dbConnection;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<BibliotecaItemResponse>> Handle(GetPagedLibraryQuery request, CancellationToken cancellationToken)
        {
            var cachaKey= $"Library:u_{request.UserId}:p{request.Page}:t{request.TamanhoPagina}";
            var bibliotecaEmCache = await _cacheService.GetAsync<PagedResult<BibliotecaItemResponse>>(cachaKey, cancellationToken);
            if (bibliotecaEmCache != null)
            {
                return bibliotecaEmCache;
            }

            var offset = (request.Page - 1) * request.TamanhoPagina;

            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            const string sql = @"            
            SELECT COUNT(1) 
            FROM Libraries 
            WHERE UserId = @UserId;
            
            SELECT 
                b.GameId AS GameId,
                j.Name AS GameName,
                j.Description as Description,    
                j.Genre AS Genre,
                b.CreatedAt AS DataAquisicao
            FROM Libraries b
            INNER JOIN Games j ON b.GameId = j.Id
            WHERE b.UserId = @UserId AND b.IsActive = 1
            ORDER BY b.CreatedAt DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @TamanhoPagina ROWS ONLY;";

            
            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                UserId = request.UserId,
                Offset = offset,
                TamanhoPagina = request.TamanhoPagina
            });

            
            var totalItems = await multi.ReadFirstOrDefaultAsync<int>();

            
            var items = await multi.ReadAsync<BibliotecaItemResponse>();

            var bibliotecaPaginada = new PagedResult<BibliotecaItemResponse>(
                items,
                request.Page,
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
