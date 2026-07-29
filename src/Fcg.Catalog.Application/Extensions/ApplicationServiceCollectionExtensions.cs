using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Catalog.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {     
        public static IServiceCollection AddAplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AddGameCommand).Assembly);
            });
            services.AddValidatorsFromAssembly(typeof(AddGameCommand).Assembly);
            return services;
        }
    }
}
