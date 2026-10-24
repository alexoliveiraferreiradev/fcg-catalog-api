using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class GamePromotionResponse
    {
        public Guid PromotionId { get; set; }
        public Guid GameId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal OriginalPrice { get; init; }
        public decimal CurrentPrice { get; init; } 
        public GameGenre Genre { get; init; }
        public DateTime EndDate { get; init; }
        public string? CoverUrl { get; init; }
        public decimal DiscountPercentage => OriginalPrice > 0 ? Math.Round((OriginalPrice - CurrentPrice) / OriginalPrice * 100, 2) : 0;
    }
}
