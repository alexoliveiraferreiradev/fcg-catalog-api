namespace Fcg.Catalog.Application.Features.Response
{
    public record PromocaoResponse
    {
        public Guid PromocaoId { get; init; }
        public Guid JogoId { get; init; }
        public decimal ValorPromocao { get; init; }
        public string NomeJogo { get; init; }
        public string DescricaoJogo { get; init; }
        public DateTime DataInicio { get; init; }
        public DateTime DataFim { get; init; }
    }
}
