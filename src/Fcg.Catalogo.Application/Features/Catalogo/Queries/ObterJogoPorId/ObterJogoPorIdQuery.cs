using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterJogoPorId
{
    public record ObterJogoPorIdQuery(Guid jogoId) : IRequest<JogosResponse>;
}
