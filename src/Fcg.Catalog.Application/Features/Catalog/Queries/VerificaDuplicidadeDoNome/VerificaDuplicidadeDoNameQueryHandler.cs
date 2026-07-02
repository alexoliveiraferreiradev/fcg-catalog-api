using Dapper;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.VerificaDuplicidadeDoNome
{
    public class VerificaDuplicidadeDoNomeQueryHandler : IRequestHandler<VerificaDuplicidadeDoNomeQuery,bool>
    {
        private readonly IDbConnection _dbConnection;

        public VerificaDuplicidadeDoNomeQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;   
        }

        public async Task<bool> Handle(VerificaDuplicidadeDoNomeQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT CAST(CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS BIT) 
            FROM Games 
            WHERE Name = @NomeJogo";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { NomeJogo = request.NomeJogo });
        }
    }
}
