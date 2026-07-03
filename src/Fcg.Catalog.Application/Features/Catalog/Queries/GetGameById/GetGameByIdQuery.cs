using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetGameById
{
    public record GetGameByIdQuery(Guid GameId) : IRequest<GameResponse>;
}
