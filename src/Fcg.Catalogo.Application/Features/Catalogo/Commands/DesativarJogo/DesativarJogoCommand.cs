using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarJogo
{
    public record DesativarJogoCommand : IRequest
    {
        public Guid JogoId { get; init; }
    }
}
