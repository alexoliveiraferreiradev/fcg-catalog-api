using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Infrastructure.Search;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fcg.Catalog.Infrastructure.Tests.Search
{
    /// <summary>
    /// O que acontece com o índice quando a API sobe (rollout, restart do pod), contra um Elasticsearch real.
    /// Cada teste começa sem o índice "games".
    /// </summary>
    [Collection(ElasticsearchCollection.Name)]
    public class GameSearchIndexInitializerTests : IAsyncLifetime
    {
        private readonly ElasticsearchFixture _fixture;
        private readonly ElasticsearchClient _client;
        private readonly InMemoryGameDocumentQuery _database = new();
        private readonly GameSearchIndexInitializer _initializer;

        public GameSearchIndexInitializerTests(ElasticsearchFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;

            // Mesmo caminho da API: o initializer pega o IGameSearchService num escopo novo.
            var services = new ServiceCollection()
                .AddScoped<IGameSearchService>(_ => new ElasticGameService(_client, _database, NullLogger<ElasticGameService>.Instance))
                .BuildServiceProvider();
            _initializer = new GameSearchIndexInitializer(_client, services.GetRequiredService<IServiceScopeFactory>(), NullLogger<GameSearchIndexInitializer>.Instance);
        }

        public async Task InitializeAsync()
        {
            _fixture.RequireDocker();
            await _client.Indices.DeleteAsync(GameIndex.Name);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static GameDocument Game(string name) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = "Descrição do jogo.",
            Genre = GameGenre.Indie,
            OriginalPrice = 50m,
            IsActive = true
        };

        private void SaveInDatabase(params GameDocument[] games)
        {
            foreach (var game in games) _database.Save(game);
        }

        private async Task<long> CountIndexedAsync()
        {
            await _client.Indices.RefreshAsync(GameIndex.Name);
            return (await _client.CountAsync<GameDocument>(c => c.Indices(GameIndex.Name))).Count;
        }

        private async Task<TypeMapping> GetMappingAsync()
        {
            var response = await _client.Indices.GetMappingAsync(new GetMappingRequest(GameIndex.Name));
            response.IsValidResponse.Should().BeTrue(response.DebugInformation);
            return response.Mappings[GameIndex.Name].Mappings;
        }

        private async Task<string?> GetMappingVersionAsync()
        {
            var meta = (await GetMappingAsync()).Meta;
            return meta is not null && meta.TryGetValue(GameIndex.MappingVersionKey, out var version) ? version?.ToString() : null;
        }

        // Índice com o mapping atual, mas gravado com outra versão no _meta (como um índice de uma versão anterior da API).
        private Task CreateIndexWithMappingVersionAsync(int version) =>
            _client.Indices.CreateAsync<GameDocument>(GameIndex.Name, index =>
            {
                GameIndex.Configure(index);
                index.Mappings(m => m.Meta(new Dictionary<string, object> { [GameIndex.MappingVersionKey] = version }));
            });

        [SkippableFact]
        public async Task StartAsync_DeveCriarOIndiceECarregarOsJogos_QuandoIndiceNaoExiste()
        {
            SaveInDatabase(Game("Pixel Pals"), Game("Neon Rally"));

            await _initializer.StartAsync(CancellationToken.None);

            (await GetMappingVersionAsync()).Should().Be(GameIndex.MappingVersion.ToString());
            (await CountIndexedAsync()).Should().Be(2);
        }

        [SkippableFact]
        public async Task StartAsync_DeveRecriarOIndice_QuandoElasticsearchCriouOIndiceSozinho()
        {
            // Índice apagado com a API no ar: a edição de um jogo grava no "games" e o Elasticsearch cria o índice
            // na hora, com mapeamento dinâmico (sem analyzer de acento, sem stemmer) e só com aquele jogo.
            var editedWhileIndexWasGone = Game("Pixel Pals");
            await _client.IndexAsync(editedWhileIndexWasGone, i => i.Index(GameIndex.Name).Id(editedWhileIndexWasGone.Id.ToString()));
            (await GetMappingVersionAsync()).Should().BeNull("o índice criado pelo Elasticsearch não tem _meta");
            SaveInDatabase(editedWhileIndexWasGone, Game("Neon Rally"), Game("Byte Garden"));

            await _initializer.StartAsync(CancellationToken.None);

            (await GetMappingVersionAsync()).Should().Be(GameIndex.MappingVersion.ToString());
            (await GetMappingAsync()).Properties!.Should().ContainKey("name")
                .WhoseValue.Should().BeOfType<TextProperty>()
                .Which.Fields!.Should().ContainKey(GameIndex.StemmedSubField);
            (await CountIndexedAsync()).Should().Be(3);
        }

        [SkippableFact]
        public async Task StartAsync_DeveRecriarOIndice_QuandoMappingEDeUmaVersaoAnterior()
        {
            await CreateIndexWithMappingVersionAsync(GameIndex.MappingVersion - 1);
            var removedFromDatabase = Game("Jogo Antigo");
            await _client.IndexAsync(removedFromDatabase, i => i.Index(GameIndex.Name).Id(removedFromDatabase.Id.ToString()));
            SaveInDatabase(Game("Pixel Pals"));

            await _initializer.StartAsync(CancellationToken.None);

            (await GetMappingVersionAsync()).Should().Be(GameIndex.MappingVersion.ToString());
            (await CountIndexedAsync()).Should().Be(1, "o índice foi recriado só com o que está no banco");
            (await _client.ExistsAsync(GameIndex.Name, removedFromDatabase.Id.ToString())).Exists.Should().BeFalse();
        }

        [SkippableFact]
        public async Task StartAsync_DeveCarregarOsJogos_QuandoIndiceAtualEstaVazio()
        {
            // Ex.: o índice foi criado mas a carga falhou (SQL fora do ar naquele momento).
            await CreateIndexWithMappingVersionAsync(GameIndex.MappingVersion);
            SaveInDatabase(Game("Pixel Pals"), Game("Neon Rally"));

            await _initializer.StartAsync(CancellationToken.None);

            (await CountIndexedAsync()).Should().Be(2);
        }

        [SkippableFact]
        public async Task StartAsync_NaoDeveMexerNoIndice_QuandoIndiceAtualJaTemJogos()
        {
            // Caso comum de rollout: nada a fazer. O documento "sentinela" só existe no índice;
            // se o índice fosse recriado ou recarregado, ele sumiria ou os jogos do banco apareceriam.
            await CreateIndexWithMappingVersionAsync(GameIndex.MappingVersion);
            var sentinel = Game("Sentinela");
            await _client.IndexAsync(sentinel, i => i.Index(GameIndex.Name).Id(sentinel.Id.ToString()));
            await _client.Indices.RefreshAsync(GameIndex.Name);
            SaveInDatabase(Game("Pixel Pals"), Game("Neon Rally"));

            await _initializer.StartAsync(CancellationToken.None);

            (await CountIndexedAsync()).Should().Be(1);
            (await _client.ExistsAsync(GameIndex.Name, sentinel.Id.ToString())).Exists.Should().BeTrue();
        }

        [SkippableFact]
        public async Task StartAsync_NaoDeveLancarErro_QuandoCargaDoBancoFalha()
        {
            // Ex.: SQL fora do ar na subida. A API sobe mesmo assim; o índice fica vazio e a próxima subida recarrega.
            var services = new ServiceCollection()
                .AddScoped<IGameSearchService, FailingGameSearchService>()
                .BuildServiceProvider();
            var initializer = new GameSearchIndexInitializer(_client, services.GetRequiredService<IServiceScopeFactory>(), NullLogger<GameSearchIndexInitializer>.Instance);

            var act = () => initializer.StartAsync(CancellationToken.None);

            await act.Should().NotThrowAsync();
            (await CountIndexedAsync()).Should().Be(0);
        }

        private sealed class FailingGameSearchService : IGameSearchService
        {
            public Task IndexAsync(Guid gameId, CancellationToken cancellationToken) => throw new NotSupportedException();
            public Task ReindexAllAsync(CancellationToken cancellationToken) => throw new InvalidOperationException("SQL fora do ar");
            public Task<Fcg.Core.Abstractions.Common.PagedResult<Application.Features.Response.GameUserResponse>> SearchAsync(
                string term, int page, int pageSize, CancellationToken cancellationToken) => throw new NotSupportedException();
        }
    }

    /// <summary>Sem Elasticsearch: não precisa de Docker.</summary>
    public class GameSearchIndexInitializerWithoutElasticsearchTests
    {
        private static GameSearchIndexInitializer InitializerPointingToNowhere()
        {
            var unreachable = new ElasticsearchClient(GameIndex.MapDocument(
                new ElasticsearchClientSettings(new Uri("http://127.0.0.1:1")).RequestTimeout(TimeSpan.FromSeconds(2))));
            var services = new ServiceCollection().BuildServiceProvider();
            return new GameSearchIndexInitializer(unreachable, services.GetRequiredService<IServiceScopeFactory>(), NullLogger<GameSearchIndexInitializer>.Instance);
        }

        [Fact]
        public async Task GetIndexStateAsync_DeveLancarErro_QuandoElasticsearchEstaForaDoAr()
        {
            // Fora do ar não é o mesmo que "índice não existe": não pode tentar criar o índice nem dizer que ele sumiu.
            var act = () => InitializerPointingToNowhere().GetIndexStateAsync(CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*indisponível*");
        }

        [Fact]
        public async Task StartAsync_NaoDeveLancarErro_QuandoElasticsearchEstaForaDoAr()
        {
            // A API precisa subir mesmo sem a busca: o catálogo pelo SQL continua funcionando.
            var act = () => InitializerPointingToNowhere().StartAsync(CancellationToken.None);

            await act.Should().NotThrowAsync();
        }
    }
}
