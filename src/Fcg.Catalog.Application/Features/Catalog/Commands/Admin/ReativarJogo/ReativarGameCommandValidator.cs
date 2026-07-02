using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReativarJogo
{
    public class ReativarJogoCommandValidator : AbstractValidator<ReativarJogoCommand>
    {
        public ReativarJogoCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);
        }
    }
}
