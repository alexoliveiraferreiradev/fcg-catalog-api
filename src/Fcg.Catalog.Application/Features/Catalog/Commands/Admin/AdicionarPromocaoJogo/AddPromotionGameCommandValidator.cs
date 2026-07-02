using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandValidator : AbstractValidator<AdicionarPromocaoJogoCommand>
    {
        public AdicionarPromocaoJogoCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);

            RuleFor(x => x.ValorPromocao)
                .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage(DomainMessages.EndDateInvalid)
                .GreaterThan(DateTime.UtcNow).WithMessage(DomainMessages.EndDateInvalid);
        }
    }
}
