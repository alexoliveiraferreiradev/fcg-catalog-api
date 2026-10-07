using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetGamesByIdAdmin
{
    public record GetGamesByIdAdminQueryGuid(Guid GameId) : IRequest<GameResponse>;
}
