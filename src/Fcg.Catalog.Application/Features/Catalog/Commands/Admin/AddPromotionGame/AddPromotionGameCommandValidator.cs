using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame;
using Fcg.Core.Abstractions.Resources;
using FluentValidation;

public class AddPromotionGameCommandValidator : AbstractValidator<AddPromotionGameCommand>
{
    public AddPromotionGameCommandValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage(DomainMessages.GameNotFound);

        RuleFor(x => x.PromotionValue)
            .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage(DomainMessages.EndDateInvalid)
            .GreaterThan(DateTime.UtcNow).WithMessage(DomainMessages.EndDateInvalid);
    }
}