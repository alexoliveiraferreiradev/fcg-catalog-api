using Dapper;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.VerificaDuplicidadeDoNome
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
            FROM Jogos 
            WHERE Nome = @NomeJogo";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { NomeJogo = request.NomeJogo });
        }
    }
}
