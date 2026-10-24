using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame;
using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Domain.Resources;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Resources;
using FluentValidation;

public class UpdateGameValidator : AbstractValidator<UpdateGameCommand>{
    public UpdateGameValidator()
    {
        RuleFor(x => x.NewName)
                .NotEmpty().WithMessage(DomainMessages.GameNameRequired)
                .Length(3, 100).WithMessage(DomainMessages.GameNameLengthInvalid)
                .Matches(GameTextPatterns.Name)
                .NotEqual("novo nome do jogo").WithMessage(DomainMessages.GameNameNotReal);

        RuleFor(x => x.NewDescription)
            .NotEmpty().WithMessage(DomainMessages.GameDescriptionRequired)
            .Length(5, 500).WithMessage(DomainMessages.GameDescriptionLengthInvalid)
            .Matches(GameTextPatterns.Description)
            .NotEqual("nova descrição do jogo").WithMessage(DomainMessages.GameDescriptionNotReal);

        RuleFor(x => x.NewPrice)
            .GreaterThan(0).WithMessage(DomainMessages.InvalidValue);

        RuleFor(x => x.NewGenre)
            .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(DomainMessages.GameGenreInvalid);

        RuleFor(x => x.NewCoverUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(CatalogMessages.CoverUrlRequired)
            .MaximumLength(CoverUrl.MaxLength).WithMessage(CatalogMessages.CoverUrlLengthInvalid)
            .Must(v => CoverUrl.IsValidFormat(v!)).WithMessage(CatalogMessages.CoverUrlFormatInvalid)
            .When(x => x.NewCoverUrl is not null);
    }
}