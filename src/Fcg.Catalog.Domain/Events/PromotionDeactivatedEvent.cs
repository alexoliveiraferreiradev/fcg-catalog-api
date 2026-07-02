using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromotionDeactivatedEvent : INotification
{
    public Guid GameId { get; }
    public Guid PromotionId { get; }

    public PromotionDeactivatedEvent(Guid gameId, Guid promotionId)
    {
        GameId = gameId;
        PromotionId = promotionId;
    }
}
