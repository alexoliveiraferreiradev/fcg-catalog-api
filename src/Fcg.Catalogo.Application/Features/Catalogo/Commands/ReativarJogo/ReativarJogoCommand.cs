using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.ReativarJogo
{
    public record ReativarJogoCommand : IRequest
    {
        public Guid JogoId { get; init; }
    }
}
