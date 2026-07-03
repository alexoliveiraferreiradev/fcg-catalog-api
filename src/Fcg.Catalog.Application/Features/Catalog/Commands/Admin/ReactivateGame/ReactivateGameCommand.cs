using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReactivateGame
{
    public record ReactivateGameCommand : IRequest
    {
        public Guid GameId { get; init; }
    }
}
