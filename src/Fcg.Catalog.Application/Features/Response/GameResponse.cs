using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public record GameResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal CurrentPrice { get; init; }
        public GameGenre Genre { get; init; }
        public bool IsActive { get; init; }
        public bool HasDiscount => CurrentPrice < OriginalPrice;
    }
}
