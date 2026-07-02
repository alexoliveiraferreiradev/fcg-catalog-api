using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class GameUpdatedEvent : INotification
{
    public Guid GameId { get; }

    public GameUpdatedEvent(Guid gameId)
    {
        GameId = gameId;
    }
}
