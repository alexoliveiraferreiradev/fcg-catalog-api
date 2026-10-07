using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReactivateGame
{
    public class ReactivateGameCommandValidator : AbstractValidator<ReactivateGameCommand>
    {
        public ReactivateGameCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);
        }
    }
}
