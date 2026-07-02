using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class GameReactivatedEvent : INotification
{
    public Guid GameId { get; }

    public GameReactivatedEvent(Guid GameId)
    {
        GameId = GameId;
    }
}
