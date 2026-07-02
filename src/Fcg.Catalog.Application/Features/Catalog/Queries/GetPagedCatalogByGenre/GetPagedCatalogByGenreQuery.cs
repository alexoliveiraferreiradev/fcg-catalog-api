using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalogByGenre
{
    public record GetPagedCatalogByGenreQuery(
        int Page = 1,
        int PageSize = 10,
        GameGenre? Genre = null) : IRequest<PagedResult<GameResponse>>;
}
