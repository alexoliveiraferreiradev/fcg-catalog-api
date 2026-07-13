using Fcg.Catalog.Domain.Enum;
using System.Text.Json.Serialization;

namespace Fcg.Catalog.Application.Features.Response
{
    public record GameUserResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        [JsonIgnore]
        public decimal OriginalPrice { get; set; }
        public decimal CurrentPrice { get; init; }
        public GameGenre Genre { get; init; }
        public bool HasDiscount => CurrentPrice < OriginalPrice;        
    }
}
