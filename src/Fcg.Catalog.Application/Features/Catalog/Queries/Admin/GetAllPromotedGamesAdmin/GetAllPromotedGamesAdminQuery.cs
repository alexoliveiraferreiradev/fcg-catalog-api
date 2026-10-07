using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetAllPromotedGamesAdmin
{
    public record GetAllPromotedGamesAdminQuery(): IRequest<IEnumerable<GamePromotionResponse>>;
}
