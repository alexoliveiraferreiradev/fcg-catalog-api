using Fcg.Catalog.Application.Extensions;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Infrastructure.Extensions;
using FluentValidation;

namespace Fcg.Catalog.API.Extensions
{
    public static class ServicesExtensions
    {
        public static WebApplicationBuilder AddServicesExtensions(this WebApplicationBuilder builder)
        {
            builder.AddObservabilityExtension()
                   .AddAuthorizationExtension()
                   .AddPresentationExtension();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddAplicationServices();

            return builder;
        }               
    }
}
