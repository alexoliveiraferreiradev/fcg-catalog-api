using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    public record DesativarJogoCommand(Guid GameId) : IRequest;
}
