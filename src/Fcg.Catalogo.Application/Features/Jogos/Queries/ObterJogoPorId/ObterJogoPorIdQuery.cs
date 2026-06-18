using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.ObterJogoPorId
{
    public record ObterJogoPorIdQuery(Guid jogoId) : IRequest<JogosResponse>;
}
