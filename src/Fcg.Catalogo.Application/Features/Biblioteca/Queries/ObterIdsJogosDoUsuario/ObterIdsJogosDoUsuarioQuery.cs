using MediatR;

namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObterIdsJogosDoUsuario
{
    public record ObterIdsJogosDoUsuarioQuery(Guid UsuarioId) : IRequest<IEnumerable<Guid>>;
}
