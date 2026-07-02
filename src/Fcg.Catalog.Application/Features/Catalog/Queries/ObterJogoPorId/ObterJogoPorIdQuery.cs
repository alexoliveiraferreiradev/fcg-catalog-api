using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObterJogoPorId
{
    public record ObterJogoPorIdQuery(Guid jogoId) : IRequest<JogoResponse>;
}
