using MediatR;

namespace Fcg.Catalog.Domain.Events
{
    public class GameAddedEvent : INotification
    {
        public Guid GameId { get; }

        public GameAddedEvent(Guid GameId)
        {
            GameId = GameId;
        }
    }
}
