using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetAllGames
{
    public record GetAllGamesQuery() : IRequest<IEnumerable<GameResponse>>;
}
