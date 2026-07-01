using Fcg.Catalogo.Domain.Repositories;
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
            builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<CatalogoDbContext>().Database.GetDbConnection());
            builder.Services.AddScoped<CatalogoDbContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJogoRepository, JogoRepository>();
            builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
            return builder;
        }
    }
}
