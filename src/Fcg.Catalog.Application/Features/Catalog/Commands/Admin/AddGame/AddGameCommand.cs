using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame
{
    public record AddGameCommand : IRequest<GameResponse>
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public GameGenre Genre { get; init; }
    }
}
