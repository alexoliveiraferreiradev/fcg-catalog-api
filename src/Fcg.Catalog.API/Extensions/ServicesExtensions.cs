using Dapper;
using Fcg.Catalog.API.Consumers;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.DapperHandlers;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.WebApi.Security;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace Fcg.Catalog.API.Extensions
{
    public static class ServicesExtensions
    {
        public static WebApplicationBuilder AddServicesExtensions(this WebApplicationBuilder builder)
        {
            builder.AddDbContextExtension().AddMassTransitExtension()
                .AddCQRSExtension().AddRedisExtension();
                        
            SqlMapper.AddTypeHandler(new NomeTypeHandler());
            SqlMapper.AddTypeHandler(new DescricaoTypeHandler());
            SqlMapper.AddTypeHandler(new PrecoTypeHandler());

            builder.AddJwtBearerExtension();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("AdminRole"));

                options.AddPolicy("AcessoGeral", policy => policy.RequireRole("AdminRole", "JogadorRole"));
            });

            return builder;
        }
        
        public static WebApplicationBuilder AddDbContextExtension(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("CatalogConnection");
            builder.Services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            return builder;
        }

        public static WebApplicationBuilder AddRedisExtension(this WebApplicationBuilder builder)
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
        public static WebApplicationBuilder AddMassTransitExtension(this WebApplicationBuilder builder) 
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentProcessedEventConsumer>();
                x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    cfg.ReceiveEndpoint("catalog-payment-processed-queue", e =>
                    {
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
                    });
                });
            });

            return builder;
        }
        public static WebApplicationBuilder AddCQRSExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AdicionarJogoCommand).Assembly);
            });
            builder.Services.AddValidatorsFromAssembly(typeof(AdicionarJogoCommand).Assembly);
            return builder;
        }
        public static WebApplicationBuilder AddJwtBearerExtension(this WebApplicationBuilder builder)
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
                    ValidIssuer = jwtSettings.Emissor,
                    ValidAudience = jwtSettings.ValidoEm
                };
            });
            return builder;
        }
    }
}
