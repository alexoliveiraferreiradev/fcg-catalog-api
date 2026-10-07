using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetAllGamesAdmin
{
    public record GetAllGamesAdminQuery() : IRequest<IEnumerable<GameResponse>>;
}
