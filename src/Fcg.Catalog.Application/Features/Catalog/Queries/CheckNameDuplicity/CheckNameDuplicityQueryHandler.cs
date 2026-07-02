using Dapper;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.CheckNameDuplicity
{
    public class CheckNameDuplicityQueryHandler : IRequestHandler<CheckNameDuplicityQuery, bool>
    {
        private readonly IDbConnection _dbConnection;

        public CheckNameDuplicityQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;   
        }

        public async Task<bool> Handle(CheckNameDuplicityQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT CAST(CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS BIT) 
            FROM Jogos 
            WHERE Nome = @GameName";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { GameName = request.GameName });
        }
    }
}
