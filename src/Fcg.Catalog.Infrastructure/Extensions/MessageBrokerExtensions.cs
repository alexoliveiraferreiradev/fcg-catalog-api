using Fcg.Catalog.Infrastructure.Consumers;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Catalog.Infrastructure.MessageBroker;
using Fcg.Catalog.Infrastructure.Persistence;
using Fcg.Core.SharedContracts.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Core.SharedContracts.Messaging;
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
                x.AddConsumer<RatingGameConsumer>();
                x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var serviceBusConfig = context.GetRequiredService<IOptions<AzureServiceBusSettings>>().Value;
                    cfg.Host(serviceBusConfig.ConnectionString);
                    // Falhas transitórias (deadlock, conexão) tentam de novo; regra de negócio inválida vai direto para a fila de erro.
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)).Ignore<DomainException>());

                    cfg.Message<IGameCreatedIntegrationEvent>(m => m.SetEntityName(Topics.GameCreated));
                    cfg.Message<IGameDeactiveIntegrationEvent>(m => m.SetEntityName(Topics.GameDeactivated));
                    cfg.Message<IGameReactiveIntegrationEvent>(m => m.SetEntityName(Topics.GameReactivated));
                    cfg.Message<IGameRatingIntegrationEvent>(m => m.SetEntityName(Topics.GameRated));

                    // Inbox do outbox: a mesma mensagem entregue duas vezes não conta a nota duas vezes.
                    cfg.ReceiveEndpoint(serviceBusConfig.CatalogGameRatedIntegrationQueue, e =>
                    {
                        e.UseEntityFrameworkOutbox<CatalogDbContext>(context);
                        e.ConfigureConsumer<RatingGameConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
