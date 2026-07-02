using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromotionDeactivatedEvent : INotification
{
    public Guid GameId { get; }
    public Guid PromotionId { get; }

    public PromotionDeactivatedEvent(Guid GameId, Guid PromotionId)
    {
        GameId = GameId;
        PromotionId = PromotionId;
    }
}
