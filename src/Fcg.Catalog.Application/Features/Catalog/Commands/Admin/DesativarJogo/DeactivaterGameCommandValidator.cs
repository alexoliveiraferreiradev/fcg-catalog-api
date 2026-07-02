using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandValidator : AbstractValidator<DesativarJogoCommand>
    {
        public DesativarJogoCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);
        }
    }
}
