using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Caching;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Catalog.Infrastructure.Queries;
using Fcg.Catalog.Infrastructure.Queries.DapperHandlers;
using Fcg.Catalog.Infrastructure.Repositories;
using Fcg.Catalog.Infrastructure.Worker;
using Fcg.Core.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContextExtension(configuration);
            services.AddCacheExtension(configuration);
            services.AddMassTransitExtension(configuration);
            services.AddHealthCheckExtension(configuration);    
            services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<CatalogDbContext>().Database.GetDbConnection());
            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new DescriptionTypeHandler());
            SqlMapper.AddTypeHandler(new PriceTypeHandler());
            services.AddScoped<CatalogDbContext>();
            services.AddHostedService<DeactivateInvalidPromotionWorker>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameQueryRepository, GameQueryRepository>();
            services.AddScoped<IPromotionQueryRepository, PromotionQueryRepository>();
            return services;
        }
    }
}
