using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarJogo
{
    public class AtualizarJogoCommandValidator : AbstractValidator<UpdateGameCommand>
    {
        public AtualizarJogoCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(DomainMessages.GameNotFound);

            RuleFor(x => x.NovoNome)
                .NotEmpty().WithMessage(DomainMessages.GameNameRequired)
                .Length(3, 20).WithMessage(DomainMessages.GameNameLengthInvalid)
                .Matches(@"[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]").WithMessage(DomainMessages.GameNameNotReal);

            RuleFor(x => x.NovaDescricao)
                .NotEmpty().WithMessage(DomainMessages.GameDescriptionRequired)
                .Length(5, 100).WithMessage(DomainMessages.GameDescriptionLengthInvalid)
                .Matches(@"[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]").WithMessage(DomainMessages.GameDescriptionNotReal);

            RuleFor(x => x.NovoPreco)
                .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

            RuleFor(x => x.NovoGenero)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(DomainMessages.GameGenreInvalid);
        }
    }
}
