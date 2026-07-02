namespace Fcg.Catalog.Application.Features.Response
{
    public record PromocaoResponse
    {
        public Guid PromotionId { get; init; }
        public Guid GameId { get; init; }
        public decimal ValorPromocao { get; init; }
        public string NomeJogo { get; init; }
        public string DescricaoJogo { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}
