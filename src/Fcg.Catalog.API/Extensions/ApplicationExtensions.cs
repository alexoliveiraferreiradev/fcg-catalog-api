using Fcg.Catalog.API.Middleware;
using Fcg.Catalog.Infrastructure.Persistence;
using Serilog;

namespace Fcg.Catalog.API.Extensions
{
    public static class ApplicationExtensions
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
        public static WebApplication AddAppConfiguration(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            return app;
        }
    }
}
