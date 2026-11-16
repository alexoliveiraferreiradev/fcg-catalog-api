using Elastic.Clients.Elasticsearch;
using Fcg.Catalog.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>
    /// Na subida da API, cria o índice "games" se ele não existir e carrega os jogos que já estão no banco.
    /// Pode rodar em várias réplicas ao mesmo tempo: só a que criar o índice faz a carga.
    /// </summary>
    internal sealed class GameSearchIndexInitializer : IHostedService
    {
        private readonly ElasticsearchClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GameSearchIndexInitializer> _logger;

        public GameSearchIndexInitializer(ElasticsearchClient client, IServiceScopeFactory scopeFactory, ILogger<GameSearchIndexInitializer> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // A busca fora do ar não deve impedir a API de subir: o catálogo pelo SQL continua funcionando.
            try
            {
                var exists = await _client.Indices.ExistsAsync(GameIndex.Name, cancellationToken);
                if (exists.Exists) return;

                var created = await _client.Indices.CreateAsync<GameDocument>(GameIndex.Name, GameIndex.Configure, cancellationToken);
                if (!created.IsValidResponse)
                {
                    _logger.LogWarning("[CatalogAPI] [Search] Índice {Index} não criado: {Erro}", GameIndex.Name, created.DebugInformation);
                    return;
                }

                _logger.LogInformation("[CatalogAPI] [Search] Índice {Index} criado. Iniciando carga inicial.", GameIndex.Name);

                using var scope = _scopeFactory.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IGameSearchService>().ReindexAllAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[CatalogAPI] [Search] Falha ao preparar o índice {Index}. A busca pode ficar indisponível.", GameIndex.Name);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
