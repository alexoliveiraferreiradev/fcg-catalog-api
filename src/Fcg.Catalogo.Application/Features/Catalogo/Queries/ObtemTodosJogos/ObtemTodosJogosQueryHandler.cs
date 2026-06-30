using Dapper;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemTodosJogos
{
    public class ObtemTodosJogosQueryHandler : IRequestHandler<ObtemTodosJogosQuery, IEnumerable<JogoResponse>>
    {
        private readonly IDbConnection _dbConnection;

        public ObtemTodosJogosQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<JogoResponse>> Handle(ObtemTodosJogosQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT 
                j.Id, 
                j.Nome, 
                j.Descricao, 
                j.PrecoBase as PrecoOriginal,             
                    COALESCE(
                        (SELECT TOP 1 p.ValorPromocao 
                        FROM Promocoes p 
                        WHERE p.JogoId = j.Id 
                        AND GETUTCDATE() BETWEEN p.DataInicio AND p.DataFim), 
                        j.PrecoBase
                    ) as PrecoAtual,
                j.Ativo,
                j.Genero
            FROM Jogos j ";
            var jogos = await _dbConnection.QueryAsync<JogoResponse>(sql);
           
            return jogos;
        }
    }
}
