using Dapper;
using Fcg.Catalog.API.Consumers;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.MessageBroker;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Catalog.Infrastructure.Queries;
using Fcg.Catalog.Infrastructure.Queries.DapperHandlers;
using Fcg.Catalog.Infrastructure.Repositories;
using Fcg.Catalog.Infrastructure.Worker;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Data;
using System.Text;

namespace Fcg.Catalog.API.Extensions
{
    public static class ServicesExtensions
    {

        public static WebApplicationBuilder AddServicesExtensions(this WebApplicationBuilder builder)
        {
            builder
                .HealthCheckExtension()
                .JsonExtensions()
                .AddSerilogExtension()
                .AddDbContextExtension()
                .AddMassTransitExtension()
                .AddCQRSExtension()
                .AddRedisExtension()
                .AddJwtBearerExtension();



            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("AdminRole"));

                options.AddPolicy("GeneralAccess", policy => policy.RequireRole("AdminRole", "PlayerRole"));
            });

            builder.AddDependencyInjection();

            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new DescriptionTypeHandler());
            SqlMapper.AddTypeHandler(new PriceTypeHandler());

            return builder;
        }

        private static WebApplicationBuilder JsonExtensions(this WebApplicationBuilder builder)
        {
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
            return builder;
        }

        private static WebApplicationBuilder HealthCheckExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<CatalogDbContext>(
                name: "database-healthcheck",
                tags: new[] { "ready" })
                .AddRedis(
                    builder.Configuration.GetConnectionString("Redis")!,
                    name: "redis-healthcheck",
                    tags: new[] { "ready" });
            return builder;
        }

        private static WebApplicationBuilder AddSerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }

        private static WebApplicationBuilder AddDbContextExtension(this WebApplicationBuilder builder)
        {
            var dbConfig = builder.Configuration.GetSection(DatabaseSettings.DatabaseSettingsSection).Get<DatabaseSettings>();
            ArgumentNullException.ThrowIfNull(dbConfig, nameof(DatabaseSettings));

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $"{dbConfig.Host},{dbConfig.Port}",
                InitialCatalog = dbConfig.DatabaseName,
                UserID = dbConfig.Username,
                Password = dbConfig.Password,
                TrustServerCertificate = true,
                Encrypt = false
            };


            builder.Services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionStringBuilder.ConnectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });
            return builder;
        }

        private static WebApplicationBuilder AddRedisExtension(this WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            ArgumentNullException.ThrowIfNull(redisConfig, nameof(RedisSettings));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.RedisSectionName));

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { redisConfig.Host, redisConfig.Port } },
                Password = redisConfig.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ReconnectRetryPolicy = new ExponentialRetry(5000, 30000)
            };

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
                options.InstanceName = redisConfig.InstanceName;
            });

            return builder;
        }
        private static WebApplicationBuilder AddMassTransitExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<RabbitMqSettings>().BindConfiguration(RabbitMqSettings.SectionName)
           .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(Program).Assembly);
                x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });
                x.AddConfigureEndpointsCallback((context, name, cfg) =>
                {
                    cfg.UseEntityFrameworkOutbox<CatalogDbContext>(context);
                });
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.ReceiveEndpoint(rabbitMqConfig.CatalogPaymentFailedQueue, e =>
                    {
                        e.UseEntityFrameworkOutbox<CatalogDbContext>(context);
                        e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(rabbitMqConfig.CatalogPaymentProcessedQueue, e =>
                    {
                        e.UseEntityFrameworkOutbox<CatalogDbContext>(context);
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
                    });
                });
            });

            return builder;
        }
        private static WebApplicationBuilder AddCQRSExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AddGameCommand).Assembly);
            });
            builder.Services.AddValidatorsFromAssembly(typeof(AddGameCommand).Assembly);
            return builder;
        }
        private static WebApplicationBuilder AddJwtBearerExtension(this WebApplicationBuilder builder)
        {
            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            builder.Services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });
            return builder;
        }

        private static WebApplicationBuilder AddDependencyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
                options.InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value;
            });

            builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<CatalogDbContext>().Database.GetDbConnection());
            builder.Services.AddScoped<CatalogDbContext>();
            builder.Services.AddHostedService<DeactivateInvalidPromotionWorker>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
            builder.Services.AddScoped<IGameQueryRepository, GameQueryRepository>();
            builder.Services.AddScoped<IPromotionQueryRepository, PromotionQueryRepository>();
            builder.Services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
            builder.Services.AddScoped<ILibraryQueryRepository, LibraryQueryRepository>();
            return builder;
        }
    }
}
