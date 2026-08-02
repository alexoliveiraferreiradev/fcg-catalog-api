using Fcg.Catalog.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    internal static class CachingExtensions
    {
        public static IServiceCollection AddCacheExtension(this IServiceCollection services,IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            ArgumentNullException.ThrowIfNull(redisConfig, nameof(RedisSettings));
            services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.RedisSectionName));

            var host = string.IsNullOrEmpty(redisConfig.Host) ? "localhost" : redisConfig.Host;
            var port = redisConfig.Port == 0 ? 6379 : redisConfig.Port;

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { host, port } },
                Password = redisConfig.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ReconnectRetryPolicy = new ExponentialRetry(5000, 30000)
            };

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
                options.InstanceName = redisConfig.InstanceName;
            });

            return services;
        }
    }
}
