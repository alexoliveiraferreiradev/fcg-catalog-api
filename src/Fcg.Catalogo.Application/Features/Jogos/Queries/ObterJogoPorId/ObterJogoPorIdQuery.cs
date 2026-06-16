using Fcg.Catalogo.Application.Dtos.Jogos;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.ObterJogoPorId
{
    public record ObterJogoPorIdQuery(Guid jogoId) : IRequest<JogosResponse>;
}
