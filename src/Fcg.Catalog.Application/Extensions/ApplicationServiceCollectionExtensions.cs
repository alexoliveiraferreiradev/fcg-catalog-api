using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Application.IntegrationEvent;
using Fcg.Core.Abstractions.Application;
using FluentValidation;
using MediatR;
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
            services.AddTransient(typeof(IPipelineBehavior<,>),
                                  typeof(ValidationBehavior<,>));
            services.AddScoped<RepublishGamesEvent>();
            return services;
        }
    }
}
