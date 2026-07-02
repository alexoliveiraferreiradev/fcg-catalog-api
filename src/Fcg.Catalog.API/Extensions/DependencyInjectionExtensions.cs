using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Catalog.Infrastructure.Repository;
using Fcg.Core.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fcg.Catalog.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebApplicationBuilder AddDependencyInjection(this WebApplicationBuilder builder)
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
            builder.Services.AddScoped<IJogoRepository, JogoRepository>();
            builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
            return builder;
        }
    }
}
