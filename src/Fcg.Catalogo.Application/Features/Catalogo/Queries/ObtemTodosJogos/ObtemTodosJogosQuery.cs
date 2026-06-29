using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemTodosJogos
{
    public record ObtemTodosJogosQuery() : IRequest<IEnumerable<JogoResponse>>;
}
