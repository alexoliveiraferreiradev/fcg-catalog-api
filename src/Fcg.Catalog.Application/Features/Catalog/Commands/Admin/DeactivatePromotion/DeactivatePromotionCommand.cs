using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public record DeactivatePromotionCommand : IRequest
    {
        public Guid PromotionId { get; init; }
    }
}
