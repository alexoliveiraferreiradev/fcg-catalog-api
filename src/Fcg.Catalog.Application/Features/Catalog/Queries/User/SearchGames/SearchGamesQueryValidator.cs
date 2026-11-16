using Fcg.Catalog.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.SearchGames
{
    public class SearchGamesQueryValidator : AbstractValidator<SearchGamesQuery>
    {
        public SearchGamesQueryValidator()
        {
            RuleFor(x => x.Term)
                .Cascade(CascadeMode.Stop)
                .Must(term => !string.IsNullOrWhiteSpace(term)).WithMessage(CatalogMessages.SearchTermRequired)
                .Must(term => term.Trim().Length <= SearchGamesQuery.MaxTermLength).WithMessage(CatalogMessages.SearchTermLength);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage(CatalogMessages.SearchPageInvalid);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, SearchGamesQuery.MaxPageSize).WithMessage(CatalogMessages.SearchPageSizeInvalid);

            RuleFor(x => x)
                .Must(x => (long)x.Page * x.PageSize <= SearchGamesQuery.MaxResultWindow).WithMessage(CatalogMessages.SearchPageLimit)
                .When(x => x.Page >= 1 && x.PageSize is >= 1 and <= SearchGamesQuery.MaxPageSize)
                .OverridePropertyName(nameof(SearchGamesQuery.Page));
        }
    }
}
