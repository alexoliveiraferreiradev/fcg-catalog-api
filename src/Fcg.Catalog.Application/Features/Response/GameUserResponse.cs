using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public record JogoUsuarioResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal PrecoAtual { get; init; }
        public GameGenre Genre { get; init; }
        public bool TemDesconto { get; init; }
    }
}
