using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Fcg.Catalog.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>
    /// Na subida da API, deixa o índice "games" pronto para a busca:
    /// <list type="bullet">
    /// <item>não existe: cria com o mapping atual e carrega os jogos do banco;</item>
    /// <item>existe com outro mapping (versão antiga, ou criado automaticamente pelo Elasticsearch
    /// quando um jogo foi gravado com o índice apagado): apaga, recria e carrega;</item>
    /// <item>existe com o mapping atual, mas vazio (carga anterior falhou): carrega;</item>
    /// <item>existe com o mapping atual e com jogos: não faz nada.</item>
    /// </list>
    /// No rollout as réplicas sobem uma de cada vez: a primeira corrige o índice e as seguintes o encontram pronto.
    /// </summary>
    internal sealed class GameSearchIndexInitializer : IHostedService
    {
        internal enum IndexState { Missing, Outdated, Empty, Ready }

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
                var state = await GetIndexStateAsync(cancellationToken);
                switch (state)
                {
                    case IndexState.Ready:
                        _logger.LogInformation("[CatalogAPI] [Search] Índice {Index} pronto (mapping v{Version}).", GameIndex.Name, GameIndex.MappingVersion);
                        return;

                    case IndexState.Empty:
                        _logger.LogWarning("[CatalogAPI] [Search] Índice {Index} está vazio. Carregando os jogos do banco.", GameIndex.Name);
                        await ReindexAllAsync(cancellationToken);
                        return;

                    case IndexState.Outdated:
                        _logger.LogWarning("[CatalogAPI] [Search] Índice {Index} com mapping diferente da v{Version}. Recriando.", GameIndex.Name, GameIndex.MappingVersion);
                        var deleted = await _client.Indices.DeleteAsync(GameIndex.Name, cancellationToken);
                        if (!deleted.IsValidResponse)
                            throw new InvalidOperationException($"Falha ao apagar o índice {GameIndex.Name}: {deleted.DebugInformation}");
                        await CreateAndLoadAsync(cancellationToken);
                        return;

                    case IndexState.Missing:
                        await CreateAndLoadAsync(cancellationToken);
                        return;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[CatalogAPI] [Search] Falha ao preparar o índice {Index}. A busca pode ficar indisponível.", GameIndex.Name);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        internal async Task<IndexState> GetIndexStateAsync(CancellationToken cancellationToken)
        {
            // O client não lança exceção quando o Elasticsearch está fora do ar: devolve resposta inválida.
            // Só o 404 significa "índice não existe"; qualquer outra falha é erro, e não motivo para criar o índice.
            var exists = await _client.Indices.ExistsAsync(GameIndex.Name, cancellationToken);
            if (!exists.IsValidResponse && exists.ApiCallDetails.HttpStatusCode != 404)
                throw new InvalidOperationException($"Elasticsearch indisponível ao verificar o índice {GameIndex.Name}: {exists.DebugInformation}");
            if (!exists.Exists) return IndexState.Missing;

            var mapping = await _client.Indices.GetMappingAsync(new GetMappingRequest(GameIndex.Name), cancellationToken);
            if (!mapping.IsValidResponse)
                throw new InvalidOperationException($"Falha ao ler o mapping do índice {GameIndex.Name}: {mapping.DebugInformation}");

            var meta = mapping.Mappings.TryGetValue(GameIndex.Name, out var record) ? record.Mappings.Meta : null;
            if (ReadMappingVersion(meta) != GameIndex.MappingVersion) return IndexState.Outdated;

            var count = await _client.CountAsync<GameDocument>(c => c.Indices(GameIndex.Name), cancellationToken);
            if (!count.IsValidResponse)
                throw new InvalidOperationException($"Falha ao contar os documentos do índice {GameIndex.Name}: {count.DebugInformation}");

            return count.Count == 0 ? IndexState.Empty : IndexState.Ready;
        }

        // O _meta volta do Elasticsearch como JSON solto: a versão pode chegar como número ou texto.
        private static int? ReadMappingVersion(IDictionary<string, object>? meta)
        {
            if (meta is null || !meta.TryGetValue(GameIndex.MappingVersionKey, out var value)) return null;

            return value switch
            {
                JsonElement { ValueKind: JsonValueKind.Number } json when json.TryGetInt32(out var number) => number,
                JsonElement { ValueKind: JsonValueKind.String } json when int.TryParse(json.GetString(), out var number) => number,
                int number => number,
                long number => (int)number,
                _ => int.TryParse(value?.ToString(), out var number) ? number : null
            };
        }

        private async Task CreateAndLoadAsync(CancellationToken cancellationToken)
        {
            var created = await _client.Indices.CreateAsync<GameDocument>(GameIndex.Name, GameIndex.Configure, cancellationToken);
            if (!created.IsValidResponse)
            {
                // Outra réplica criou primeiro (resource_already_exists_exception) ou o Elasticsearch recusou o mapping.
                _logger.LogWarning("[CatalogAPI] [Search] Índice {Index} não criado: {Erro}", GameIndex.Name, created.DebugInformation);
                return;
            }

            _logger.LogInformation("[CatalogAPI] [Search] Índice {Index} criado com o mapping v{Version}. Iniciando carga.", GameIndex.Name, GameIndex.MappingVersion);
            await ReindexAllAsync(cancellationToken);
        }

        private async Task ReindexAllAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IGameSearchService>().ReindexAllAsync(cancellationToken);
        }
    }
}
