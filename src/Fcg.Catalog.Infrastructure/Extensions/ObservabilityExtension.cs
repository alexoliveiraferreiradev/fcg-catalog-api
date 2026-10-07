using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.WebApi.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    internal static class ObservabilityExtension
    {
        public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services,
            IConfiguration configuration)
        {            
            services.AddHealthChecks()
                 .AddCheck("live", () => HealthCheckResult.Healthy(), tags: new[] { HealthCheckTags.Live })
                .AddDbContextCheck<CatalogDbContext>(
                name: "database-healthcheck",
                tags: new[] { HealthCheckTags.Ready })
                .AddRedis(
                     sp => sp.GetRequiredService<IConnectionMultiplexer>(),
                    name: "redis-healthcheck",
                    tags: new[] { HealthCheckTags.Ready });

            return services;
        }
    }
}
