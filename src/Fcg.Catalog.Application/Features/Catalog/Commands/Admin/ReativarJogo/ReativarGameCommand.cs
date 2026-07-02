using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReativarJogo
{
    public record ReativarJogoCommand : IRequest
    {
        public Guid GameId { get; init; }
    }
}
