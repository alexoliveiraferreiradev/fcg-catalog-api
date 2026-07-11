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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
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
            var sqlConnection = builder.Configuration.GetConnectionString("CatalogConnection");
            builder.Services.AddHealthChecks()
                .AddSqlServer(sqlConnection!)
                .AddRedis(
                    builder.Configuration.GetConnectionString("Redis")!,
                    name: "redis-healthcheck");
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
            var connectionString = builder.Configuration.GetConnectionString("CatalogConnection");
            builder.Services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            return builder;
        }

        private static WebApplicationBuilder AddRedisExtension(this WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisOptions>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(redisConfig.Configuration, true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig.Configuration;
                options.InstanceName = redisConfig.InstanceName;
            });

            return builder;
        }
        private static WebApplicationBuilder AddMassTransitExtension(this WebApplicationBuilder builder) 
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentProcessedEventConsumer>();
                x.AddConsumer<PaymentFailedEventConsumer>();
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
                    var rabbitSection = builder.Configuration.GetSection(RabbitMqQueueOptions.SectionName);
                    var options = rabbitSection.Get<RabbitMqQueueOptions>();

                    if (options == null || string.IsNullOrEmpty(options.CatalogPaymentProcessedQueue))
                    {
                        throw new Exception("Não foi configurado as queues para o rabbitmq");
                    }

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    cfg.ReceiveEndpoint(options.CatalogPaymentFailedQueue, e =>
                    {
                        e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(options.CatalogPaymentProcessedQueue, e =>
                    {
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
