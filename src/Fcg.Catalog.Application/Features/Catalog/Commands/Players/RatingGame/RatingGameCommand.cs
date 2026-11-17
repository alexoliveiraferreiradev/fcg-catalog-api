using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Players.RatingGame
{
    /// <summary>Nota que veio da biblioteca do usuário (evento game-rated).</summary>
    public record RatingGameCommand(Guid GameId, Guid UserId, double Rating, DateTime OccurredAt) : IRequest;
}
