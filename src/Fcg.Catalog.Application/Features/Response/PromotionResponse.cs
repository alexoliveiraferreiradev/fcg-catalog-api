namespace Fcg.Catalog.Application.Features.Response
{
    public record PromotionResponse
    {
        public Guid PromotionId { get; init; }
        public Guid GameId { get; init; }
        public decimal ValorPromocao { get; init; }
        public string GameName { get; init; }
        public string GameDescription { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}
