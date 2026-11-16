using Fcg.Catalog.Domain.Enum;
using System.Text.Json.Serialization;

namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>
    /// Jogo como fica guardado no índice "games". Guarda a promoção (preço e período) em vez do preço atual:
    /// o preço atual é calculado na leitura, então a promoção que vence não deixa o índice desatualizado.
    /// </summary>
    public sealed record GameDocument
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

        // Gravado pelo nome ("RPG", "MundoAberto") para entrar na busca textual.
        [JsonConverter(typeof(JsonStringEnumConverter<GameGenre>))]
        public GameGenre Genre { get; init; }

        public string? CoverUrl { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal? PromotionPrice { get; init; }
        public DateTime? PromotionStartDate { get; init; }
        public DateTime? PromotionEndDate { get; init; }
        public bool IsActive { get; init; }
        public double? AverageRating { get; init; }
        public int RatingCount { get; init; }

        public decimal CurrentPriceAt(DateTime utcNow) =>
            PromotionPrice.HasValue && utcNow >= PromotionStartDate && utcNow <= PromotionEndDate
                ? PromotionPrice.Value
                : OriginalPrice;
    }
}
