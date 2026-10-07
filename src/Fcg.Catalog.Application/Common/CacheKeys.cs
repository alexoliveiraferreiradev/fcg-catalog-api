using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Common
{
    public static class CacheKeys
    {
        public const string CatalogGames = "catalog:game";
        public const string CatalogPages = "catalog:pag:";
        public const string CatalogPromotion = "catalog:promotion";
        public const string CatalogPromotionPages = "catalog:promopag:";

        public static string PagedCatalog(int page, int pageSize, GameGenre? genre, bool onlyPromoted)
            => $"{CatalogPages}{PagedSuffix(page, pageSize, genre, onlyPromoted)}";

        public static string PagedPromotedCatalog(int page, int pageSize, GameGenre? genre, bool onlyPromoted)
            => $"{CatalogPromotionPages}{PagedSuffix(page, pageSize, genre, onlyPromoted)}";

        private static string PagedSuffix(int page, int pageSize, GameGenre? genre, bool onlyPromoted)
        {
            var g = genre.HasValue ? genre.Value.ToString() : "genres";
            var p = onlyPromoted ? "yes" : "no";
            return $"p{page}:t{pageSize}:g_{g}:prom_{p}";
        }
    }
}
