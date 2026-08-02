using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.Persistence;
using Serilog;

namespace Fcg.Catalog.API.Extensions
{
    public static class ObservabilityExtensions
    {
        public static WebApplicationBuilder AddObservabilityExtension(this WebApplicationBuilder builder)
        {
            builder.AddHealthCheckExtension().AddSerilogExtension();
            return builder;
        }

        #region Health Check
        private static WebApplicationBuilder AddHealthCheckExtension(this WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            var connectionString = redisConfig != null && !string.IsNullOrEmpty(redisConfig.Host)
                ? $"{redisConfig.Host}:{redisConfig.Port},password={redisConfig.Password}"
                : "localhost:6379,password=secret_password";

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<CatalogDbContext>(
                name: "database-healthcheck",
                tags: new[] { "ready" })
                .AddRedis(
                    connectionString,
                    name: "redis-healthcheck",
                    tags: new[] { "ready" });
            return builder;
        }
        #endregion

        #region Serilog 
        private static WebApplicationBuilder AddSerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }
        #endregion
    }
}
