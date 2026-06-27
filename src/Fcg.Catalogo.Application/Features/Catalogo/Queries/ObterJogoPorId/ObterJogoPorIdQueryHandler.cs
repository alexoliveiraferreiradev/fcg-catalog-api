using Dapper;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterJogoPorId
{
    public class ObterJogoPorIdQueryHandler : IRequestHandler<ObterJogoPorIdQuery, JogosResponse>
    {
        private readonly IDbConnection _dbConnection;

        public ObterJogoPorIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<JogosResponse> Handle(ObterJogoPorIdQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                     j.Id,
                     j.Nome,    
                     j.Descricao,
                     j.PrecoBase AS PrecoOriginal,
                     COALESCE(
                         (SELECT TOP 1 p.ValorPromocao 
                          FROM Promocoes p 
                          WHERE p.JogoId = j.Id 
                            AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim), 
                         j.PrecoBase
                     ) AS PrecoAtual,
                     j.Genero,
                     j.Ativo
            FROM Jogos j
            WHERE j.Id = @JogoId;";

            return await _dbConnection.QueryFirstOrDefaultAsync<JogosResponse>(sql, new { JogoId = request.jogoId });
        }
    }
}
