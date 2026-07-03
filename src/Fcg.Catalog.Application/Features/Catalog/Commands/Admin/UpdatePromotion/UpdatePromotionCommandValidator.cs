using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public class UpdatePromotionCommandValidator : AbstractValidator<UpdatePromotionCommand>
    {
        public UpdatePromotionCommandValidator()
        {
            RuleFor(x => x.PromotionId)
                .NotEmpty().WithMessage(DomainMessages.PromotionNotFound);

            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);

            RuleFor(x => x.NovoValorPromocao)
                .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

            RuleFor(x => x.NovaDataFim)
                .GreaterThan(DateTime.UtcNow).WithMessage(DomainMessages.EndDateInvalid);
        }
    }
}
