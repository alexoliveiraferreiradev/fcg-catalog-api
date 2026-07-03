using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public class DeactivatePromotionCommandValidator : AbstractValidator<DeactivatePromotionCommand>
    {
        public DeactivatePromotionCommandValidator()
        {
            RuleFor(x => x.PromotionId)
                .NotEmpty().WithMessage(DomainMessages.PromotionNotFound);
        }
    }
}
