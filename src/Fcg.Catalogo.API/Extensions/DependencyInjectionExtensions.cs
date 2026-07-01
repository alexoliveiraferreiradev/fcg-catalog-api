using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Infrastructure.Caching;
using Fcg.Catalogo.Infrastructure.Persistence;
using Fcg.Catalogo.Infrastructure.Repository;
using Fcg.Core.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fcg.Catalogo.API.Extensions
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

            builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<CatalogoDbContext>().Database.GetDbConnection());
            builder.Services.AddScoped<CatalogoDbContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IJogoRepository, JogoRepository>();
            builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
            return builder;
        }
    }
}
