using Fcg.Catalog.Infrastructure.MessageBroker;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Core.WebApi.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    internal static class MessageBrokerExtensions
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

            services.AddOptions<AzureServiceBusSettings>().BindConfiguration(AzureServiceBusSettings.SectionName)
           .ValidateDataAnnotations().ValidateOnStart();

            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var serviceBusConfig = context.GetRequiredService<IOptions<AzureServiceBusSettings>>().Value;
                    cfg.Host(serviceBusConfig.ConnectionString);
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.Message<IGameCreatedIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.GameCreated));
                    cfg.Message<IGameDeactiveIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.GameDeactivated));
                    cfg.Message<IGameReactiveIntegrationEvent>(m => m.SetEntityName(ServiceBusTopics.GameReactivated));
                });
            });

            return services;
        }
    }
}
