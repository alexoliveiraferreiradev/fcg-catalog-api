using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public class DesativarPromocaoCommandValidator : AbstractValidator<DesativarPromocaoCommand>
    {
        public DesativarPromocaoCommandValidator()
        {
            RuleFor(x => x.PromotionId)
                .NotEmpty().WithMessage(DomainMessages.PromotionNotFound);
        }
    }
}
