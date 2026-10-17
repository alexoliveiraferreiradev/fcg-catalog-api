using Fcg.Catalog.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameCovers
{
    public class GetGameCoversQueryValidator : AbstractValidator<GetGameCoversQuery>
    {
        public GetGameCoversQueryValidator()
        {
            RuleFor(x => x.Ids)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(CatalogMessages.GameCoversIdsRequired)
                .Must(ids => ids.Distinct().Count() <= GetGameCoversQuery.MaxIds).WithMessage(CatalogMessages.GameCoversIdsLimit);
        }
    }
}
