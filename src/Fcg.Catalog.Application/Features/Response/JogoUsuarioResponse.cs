using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public record JogoUsuarioResponse
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal PrecoAtual { get; init; }
        public GeneroJogo Genero { get; init; }
        public bool TemDesconto { get; init; }
    }
}
