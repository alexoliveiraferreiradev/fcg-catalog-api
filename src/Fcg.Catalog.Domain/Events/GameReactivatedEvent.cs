using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class GameReactivatedEvent : INotification
{
    public Guid GameId { get; }

    public GameReactivatedEvent(Guid gameId)
    {
        GameId = gameId;
    }
}
