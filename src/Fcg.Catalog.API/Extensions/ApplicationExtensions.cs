using Fcg.Catalog.API.Endpoints.Admin;
using Fcg.Catalog.API.Endpoints.Anonymous;
using Fcg.Catalog.Application.IntegrationEvent;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.WebApi.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
                    var seeded = await CatalogDbContextSeed.SeedDataAsync(context);

                    if (seeded)
                    {
                        var republishEvent = services.GetRequiredService<RepublishGamesEvent>();
                        await republishEvent.Handle();
                    }
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

            #region Health Check
            app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
            app.MapHealthChecks("/health/readiness", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });
            #endregion
            return app;
        }
    }
}
