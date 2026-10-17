using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public record GameUserResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal CurrentPrice { get; init; }
        public GameGenre Genre { get; init; }
        public string? CoverUrl { get; init; }
        public bool HasDiscount => CurrentPrice < OriginalPrice;        
    }
}
