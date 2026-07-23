using Fcg.Catalog.API.Endpoints.Admin;
using Fcg.Catalog.API.Endpoints.Anonymous;
using Fcg.Catalog.API.Endpoints.User;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.WebApi.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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
            app.ConfigureEndpoints();
            app.UseSwaggerDocumentation();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            return app;
        }

        private static WebApplication ConfigureEndpoints(this WebApplication app)
        {
            #region Game Endpoint
            app.MapGamesEndpoints();
            app.MapPromotionsEndpoints();
            app.MapCatalogEndpoints();
            #endregion

            app.MapLibraryUserEndpoints();
            app.MapOrderEndpoints();

            #region Health Check
            app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
            app.MapHealthChecks("/health/readiness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            #endregion
            return app;
        }
    }
}
