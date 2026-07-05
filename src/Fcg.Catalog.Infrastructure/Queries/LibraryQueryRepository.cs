using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary;
using Fcg.Core.Abstractions.Common;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Queries
{
    public class LibraryQueryRepository : ILibraryQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public LibraryQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<PagedResult<BibliotecaItemResponse>> GetPagedLibraryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;

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
                                FETCH NEXT @PageSize ROWS ONLY;";


            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                UserId = userId,
                Offset = offset,
                PageSize = pageSize
            });


            var totalItems = await multi.ReadFirstOrDefaultAsync<int>();


            var items = await multi.ReadAsync<BibliotecaItemResponse>();

            var bibliotecaPaginada = new PagedResult<BibliotecaItemResponse>(
                items,
                page,
                pageSize,
                totalItems);

            return bibliotecaPaginada;
        }

        public async Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid userId, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT GameId FROM Libraries WHERE UserId = @UserId AND IsActive = 1";
            return await _dbConnection.QueryAsync<Guid>(sql, new { UserId = userId });
        }
    }
}
