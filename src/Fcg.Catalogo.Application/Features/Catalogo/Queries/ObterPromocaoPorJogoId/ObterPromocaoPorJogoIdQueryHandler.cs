using Dapper;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterPromocaoPorJogoId
{
    public class ObterPromocaoPorJogoIdQueryHandler : IRequestHandler<ObterPromocaoPorJogoIdQuery, PromocaoResponse>
    {
        private readonly IDbConnection _dbConnection;

        public ObterPromocaoPorJogoIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<PromocaoResponse> Handle(ObterPromocaoPorJogoIdQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT 
                p.Id AS PromocaoId,
                p.JogoId,
                p.ValorPromocao,
                j.Nome AS NomeJogo,
                j.Descricao AS DescricaoJogo,
                p.DataInicio,
                p.DataFim
            FROM Promocoes p
            INNER JOIN Jogos j ON p.JogoId = j.Id
            WHERE p.Id = @PromocaoId;";
            return await _dbConnection.QueryFirstOrDefaultAsync<PromocaoResponse>(sql, new { PromocaoId = request.promocaoId });
        }
    }
}
