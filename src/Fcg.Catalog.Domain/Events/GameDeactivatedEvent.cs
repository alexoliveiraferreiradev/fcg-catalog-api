using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class GameDeactivatedEvent : INotification
{
    public Guid GameId { get; }

    public GameDeactivatedEvent(Guid gameId)
    {
        GameId = gameId;
    }
}
