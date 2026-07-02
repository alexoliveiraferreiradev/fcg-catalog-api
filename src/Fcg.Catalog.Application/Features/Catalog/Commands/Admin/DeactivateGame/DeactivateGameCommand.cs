using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    public record DeactivateGameCommand(Guid GameId) : IRequest;
}
