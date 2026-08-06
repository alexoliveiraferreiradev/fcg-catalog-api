using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    internal static class ObservabilityExtension
    {
        public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            var connectionString = redisConfig != null && !string.IsNullOrEmpty(redisConfig.Host)
                ? $"{redisConfig.Host}:{redisConfig.Port},password={redisConfig.Password}"
                : "localhost:6379,password=secret_password";

            services.AddHealthChecks()
                 .AddCheck("live", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
                .AddDbContextCheck<CatalogDbContext>(
                name: "database-healthcheck",
                tags: new[] { "ready" })
                .AddRedis(
                    connectionString,
                    name: "redis-healthcheck",
                    tags: new[] { "ready" });

            return services;
        }
    }
}
