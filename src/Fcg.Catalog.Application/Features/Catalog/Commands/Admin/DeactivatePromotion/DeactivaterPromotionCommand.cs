using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public record DesativarPromocaoCommand : IRequest
    {
        public Guid PromotionId { get; init; }
    }
}
