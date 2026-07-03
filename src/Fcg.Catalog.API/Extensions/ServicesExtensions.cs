using Dapper;
using Fcg.Catalog.API.Consumers;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.DapperHandlers;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Catalog.Infrastructure.Repository;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
            builder.JsonExtensions()
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
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));                    
                    cfg.ReceiveEndpoint("catalog-payment-processed-queue", e =>
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
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
            return builder;
        }
    }
}
