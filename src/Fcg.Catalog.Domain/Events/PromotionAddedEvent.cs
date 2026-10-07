using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromotionAddedEvent : INotification
{
    public Guid GameId { get; }
    public Guid PromotionId { get; }

    public PromotionAddedEvent(Guid gameId, Guid promotionId)
    {
        GameId = gameId;
        PromotionId = promotionId;
    }
}
