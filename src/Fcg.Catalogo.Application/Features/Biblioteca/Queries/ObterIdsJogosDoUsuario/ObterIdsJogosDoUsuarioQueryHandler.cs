using Dapper;
using MediatR;
using System.Data;

namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObterIdsJogosDoUsuario
{
    public class ObterIdsJogosDoUsuarioQueryHandler : IRequestHandler<ObterIdsJogosDoUsuarioQuery, IEnumerable<Guid>>
    {
        private readonly IDbConnection _dbConnection;

        public ObterIdsJogosDoUsuarioQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Guid>> Handle(ObterIdsJogosDoUsuarioQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT JogoId 
                                FROM Bibliotecas 
                                WHERE UsuarioId = @UsuarioId 
                                ORDER BY DataCadastro DESC";

            var listaGuids = await _dbConnection.QueryAsync<Guid>(sql, new { UsuarioId = request.UsuarioId });

            return listaGuids;
        }
    }
}
