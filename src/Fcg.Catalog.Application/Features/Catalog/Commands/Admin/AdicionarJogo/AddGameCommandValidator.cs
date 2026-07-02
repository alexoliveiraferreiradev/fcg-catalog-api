using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo
{
    public class AdicionarJogoCommandValidator : AbstractValidator<AddGameCommand>
    {
        public AdicionarJogoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.GameNameRequired)
                .Length(3, 20).WithMessage(DomainMessages.GameNameLengthInvalid)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Name do Game").WithMessage(DomainMessages.GameNameNotReal);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(DomainMessages.GameDescriptionRequired)
                .Length(5, 100).WithMessage(DomainMessages.GameDescriptionLengthInvalid)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Descrição do Game").WithMessage(DomainMessages.GameDescriptionNotReal);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

            RuleFor(x => x.Genre)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(DomainMessages.GameGenreInvalid);
        }
    }
}
