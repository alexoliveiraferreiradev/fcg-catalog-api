using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class GameUpdatedEvent : INotification
{
    public Guid GameId { get; }

    public GameUpdatedEvent(Guid GameId)
    {
        GameId = GameId;
    }
}
