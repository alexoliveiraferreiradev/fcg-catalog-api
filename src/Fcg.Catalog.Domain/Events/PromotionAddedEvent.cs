using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromotionAddedEvent : INotification
{
    public Guid GameId { get; }
    public Guid PromotionId { get; }

    public PromotionAddedEvent(Guid GameId, Guid PromotionId)
    {
        GameId = GameId;
        PromotionId = PromotionId;
    }
}
