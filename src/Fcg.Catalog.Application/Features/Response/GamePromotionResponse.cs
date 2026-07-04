using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class GamePromotionResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal CurrentPrice { get; init; }
        public GameGenre Genre { get; init; }
        public decimal DiscountPercentage => OriginalPrice > 0 ? Math.Round((OriginalPrice - CurrentPrice) / OriginalPrice * 100, 2) : 0;
    }
}
