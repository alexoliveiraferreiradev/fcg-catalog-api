using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObtemTodosJogos
{
    public record ObtemTodosJogosQuery() : IRequest<IEnumerable<JogoResponse>>;
}
