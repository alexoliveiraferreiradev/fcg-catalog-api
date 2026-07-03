using Fcg.Catalog.Infrastructure.Persistence;

namespace Fcg.Catalog.API.Extensions
{
    public static class SeedDataExtensions
    {
        public async static Task<WebApplication> SeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CatalogDbContext>();
                    await CatalogDbContextSeed.SeedDataAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocorreu um erro ao alimentar o banco de dados inicial.");
                }
            }
            return app;
        }
    }
}
