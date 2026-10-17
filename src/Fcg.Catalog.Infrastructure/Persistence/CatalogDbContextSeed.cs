using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Catalog.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Persistence
{
    public static class CatalogDbContextSeed
    {
        /// <summary>
        /// Cadastra o catálogo de exemplo (CatalogSeedData) quando a tabela de jogos está vazia.
        /// As promoções terminam alguns dias depois do seed, para o destaque não nascer vencido.
        /// </summary>
        /// <returns>true quando cadastrou; o chamador republica os jogos para os outros serviços.</returns>
        public static async Task<bool> SeedDataAsync(CatalogDbContext context)
        {
            if (await context.Games.AnyAsync())
            {
                return false;
            }

            var now = DateTime.UtcNow;
            foreach (var seed in CatalogSeedData.Games)
            {
                var game = new Game(new Name(seed.Name), new Description(seed.Description), new Price(seed.Price), seed.Genre, new CoverUrl(seed.CoverUrl));

                if (seed.Promotion is not null)
                {
                    game.AddPromotion(new Price(seed.Promotion.Price), new Period(now, now.AddDays(seed.Promotion.DaysToEnd)));
                }

                context.Games.Add(game);
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}
