using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromotionUpdatedEvent : INotification
{
    public Guid GameId { get; }
    public Guid PromotionId { get; }

    public PromotionUpdatedEvent(Guid gameId, Guid promotionId)
    {
        GameId = gameId;
        PromotionId = promotionId;
    }
}
