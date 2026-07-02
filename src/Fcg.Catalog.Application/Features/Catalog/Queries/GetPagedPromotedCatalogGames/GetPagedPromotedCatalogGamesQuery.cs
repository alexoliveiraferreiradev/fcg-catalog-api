using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames
{
    public record ObtemCatalogJogosPromovidosQuery(
        int Page = 1,
        int TamanhoPagina = 10,
        GameGenre? Genre = null,
        bool? OnlyPromoted = null) : IRequest<PagedResult<GameResponse>>;
}
