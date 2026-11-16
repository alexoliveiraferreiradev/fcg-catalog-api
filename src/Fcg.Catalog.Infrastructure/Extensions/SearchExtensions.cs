using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Infrastructure.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fcg.Catalog.Infrastructure.Extensions
{
    internal static class SearchExtensions
    {
        public static IServiceCollection AddSearchExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ElasticsearchSettings>()
                .Bind(configuration.GetSection(ElasticsearchSettings.SectionName))
                .Validate(s => s.IsValid(), "ElasticSearchSettings: informe CloudId + ApiKey (Elastic Cloud) ou Url (instância própria).")
                .ValidateOnStart();

            // Singleton: o client é thread-safe e mantém o pool de conexões.
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ElasticsearchSettings>>().Value;

                var clientSettings = string.IsNullOrWhiteSpace(settings.CloudId)
                    ? new ElasticsearchClientSettings(new Uri(settings.Url!))
                    : new ElasticsearchClientSettings(settings.CloudId, new ApiKey(settings.ApiKey!));

                if (string.IsNullOrWhiteSpace(settings.CloudId) && !string.IsNullOrWhiteSpace(settings.ApiKey))
                    clientSettings.Authentication(new ApiKey(settings.ApiKey));

                return new ElasticsearchClient(GameIndex.MapDocument(clientSettings));
            });

            services.AddScoped<IGameDocumentQuery, GameDocumentQuery>();
            services.AddScoped<IGameSearchService, ElasticGameService>();
            services.AddHostedService<GameSearchIndexInitializer>();

            return services;
        }
    }
}
