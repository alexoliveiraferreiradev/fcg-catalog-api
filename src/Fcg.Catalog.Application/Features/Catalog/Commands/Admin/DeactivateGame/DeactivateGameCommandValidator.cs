using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    public class DeactivateGameCommandValidator : AbstractValidator<DeactivateGameCommand>
    {
        public DeactivateGameCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);
        }
    }
}
