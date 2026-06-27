using Fcg.Catalogo.Domain.Enum;

namespace Fcg.Catalogo.Application.Features.Response
{
    public record JogosResponse
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal PrecoOriginal { get; init; }
        public decimal PrecoAtual { get; init; }
        public GeneroJogo Genero { get; init; }
        public bool Ativo { get; init; }
        public bool TemDesconto => PrecoAtual < PrecoOriginal;
    }
}
