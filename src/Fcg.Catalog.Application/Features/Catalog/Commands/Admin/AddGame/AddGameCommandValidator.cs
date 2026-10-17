using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Domain.Resources;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame
{
    public class AddGameCommandValidator : AbstractValidator<AddGameCommand>
    {
        public AddGameCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.GameNameRequired)
                .Length(3, 100).WithMessage(DomainMessages.GameNameLengthInvalid)
                .Matches(GameTextPatterns.Name)
                .NotEqual("nome do jogo").WithMessage(DomainMessages.GameNameNotReal);
            

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(DomainMessages.GameDescriptionRequired)
                .Length(5, 500).WithMessage(DomainMessages.GameDescriptionLengthInvalid)
                .Matches(GameTextPatterns.Description)
                .NotEqual("descrição do jogo").WithMessage(DomainMessages.GameDescriptionNotReal);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage(DomainMessages.InvalidValue);

            RuleFor(x => x.Genre)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(DomainMessages.GameGenreInvalid);

            RuleFor(x => x.CoverUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(CatalogMessages.CoverUrlRequired)
                .MaximumLength(CoverUrl.MaxLength).WithMessage(CatalogMessages.CoverUrlLengthInvalid)
                .Must(v => CoverUrl.IsValidFormat(v!)).WithMessage(CatalogMessages.CoverUrlFormatInvalid)
                .When(x => x.CoverUrl is not null);
        }
    }
}
