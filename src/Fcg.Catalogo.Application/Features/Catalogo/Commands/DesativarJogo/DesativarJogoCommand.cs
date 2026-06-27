using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarJogo
{
    public record DesativarJogoCommand(Guid jogoId) : IRequest;
}
