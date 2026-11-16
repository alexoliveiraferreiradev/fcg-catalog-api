using Elastic.Clients.Elasticsearch;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Infrastructure.Search;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fcg.Catalog.Infrastructure.Tests.Search
{
    /// <summary>
    /// Testes do ElasticGameService contra um Elasticsearch real: o mapping, o fuzzy e o stemmer
    /// só se comportam de verdade no servidor. Cada teste começa com o índice "games" recriado e vazio.
    /// </summary>
    [Collection(ElasticsearchCollection.Name)]
    public class ElasticGameServiceTests : IAsyncLifetime
    {
        private readonly ElasticsearchFixture _fixture;
        private readonly ElasticsearchClient _client;
        private readonly InMemoryGameDocumentQuery _database = new();
        private readonly ElasticGameService _service;

        public ElasticGameServiceTests(ElasticsearchFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _service = new ElasticGameService(_client, _database, NullLogger<ElasticGameService>.Instance);
        }

        public async Task InitializeAsync()
        {
            // Roda antes de cada teste: sem Docker, pula na máquina do dev e falha no CI.
            _fixture.RequireDocker();

            await _client.Indices.DeleteAsync(GameIndex.Name);
            var created = await _client.Indices.CreateAsync<GameDocument>(GameIndex.Name, GameIndex.Configure);
            created.IsValidResponse.Should().BeTrue(created.DebugInformation);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static GameDocument Game(string name, string description = "Descrição do jogo.", GameGenre genre = GameGenre.Indie, bool isActive = true) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Genre = genre,
            OriginalPrice = 50m,
            IsActive = isActive
        };

        // O Elasticsearch só mostra na busca o que foi gravado depois de um refresh (~1s); o teste força na hora.
        private async Task IndexGamesAsync(params GameDocument[] games)
        {
            foreach (var game in games)
            {
                _database.Save(game);
                await _service.IndexAsync(game.Id, CancellationToken.None);
            }
            await _client.Indices.RefreshAsync(GameIndex.Name);
        }

        private async Task<long> CountIndexedAsync()
        {
            await _client.Indices.RefreshAsync(GameIndex.Name);
            return (await _client.CountAsync<GameDocument>(c => c.Indices(GameIndex.Name))).Count;
        }

        private async Task<List<string>> SearchNamesAsync(string term)
        {
            var result = await _service.SearchAsync(term, 1, 10, CancellationToken.None);
            return result.Items.Select(i => i.Name).ToList();
        }

        #region IndexAsync

        [SkippableFact]
        public async Task IndexAsync_DeveGravarJogoNoIndice_UsandoOIdDoJogoComoId()
        {
            var game = Game("Pixel Pals");
            _database.Save(game);

            await _service.IndexAsync(game.Id, CancellationToken.None);

            var stored = await _client.GetAsync<GameDocument>(GameIndex.Name, game.Id.ToString());
            stored.Found.Should().BeTrue();
            stored.Source!.Name.Should().Be("Pixel Pals");
        }

        [SkippableFact]
        public async Task IndexAsync_DeveAtualizarODocumento_QuandoJogoJaEstaIndexado()
        {
            // Regressão: IndexAsync(doc, "games") trata "games" como _id e gravava um documento novo a cada edição.
            var game = Game("Pixel Pals");
            await IndexGamesAsync(game);

            _database.Save(game with { Name = "Pixel Pals Deluxe" });
            await _service.IndexAsync(game.Id, CancellationToken.None);

            (await CountIndexedAsync()).Should().Be(1);
            var stored = await _client.GetAsync<GameDocument>(GameIndex.Name, game.Id.ToString());
            stored.Source!.Name.Should().Be("Pixel Pals Deluxe");
        }

        [SkippableFact]
        public async Task IndexAsync_DeveRemoverDoIndice_QuandoJogoNaoExisteMaisNoBanco()
        {
            var game = Game("Pixel Pals");
            await IndexGamesAsync(game);

            _database.Remove(game.Id);
            await _service.IndexAsync(game.Id, CancellationToken.None);

            (await CountIndexedAsync()).Should().Be(0);
        }

        [SkippableFact]
        public async Task IndexAsync_NaoDeveLancarErro_QuandoJogoNaoExisteNoBancoNemNoIndice()
        {
            var act = () => _service.IndexAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        #endregion

        #region ReindexAllAsync

        [SkippableFact]
        public async Task ReindexAllAsync_DeveCarregarTodosOsJogosDoBanco()
        {
            var games = new[] { Game("Pixel Pals"), Game("Neon Rally"), Game("Byte Garden", isActive: false) };
            foreach (var game in games) _database.Save(game);

            await _service.ReindexAllAsync(CancellationToken.None);

            (await CountIndexedAsync()).Should().Be(3);
            var stored = await _client.GetAsync<GameDocument>(GameIndex.Name, games[1].Id.ToString());
            stored.Source!.Name.Should().Be("Neon Rally");
        }

        [SkippableFact]
        public async Task ReindexAllAsync_NaoDeveLancarErro_QuandoBancoEstaVazio()
        {
            var act = () => _service.ReindexAllAsync(CancellationToken.None);

            await act.Should().NotThrowAsync();
            (await CountIndexedAsync()).Should().Be(0);
        }

        #endregion

        #region SearchAsync

        [SkippableFact]
        public async Task SearchAsync_DeveEncontrarJogo_QuandoTermoTemErroDeDigitacao()
        {
            await IndexGamesAsync(Game("Neon Rally"), Game("Byte Garden"));

            (await SearchNamesAsync("Neom Raly")).Should().Equal("Neon Rally");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveEncontrarJogo_QuandoTermoNaoTemAcento()
        {
            await IndexGamesAsync(Game("Steel Horizon", description: "Mechas em combate na órbita de Júpiter."), Game("Byte Garden"));

            (await SearchNamesAsync("orbita jupiter")).Should().Equal("Steel Horizon");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveEncontrarPelaFormaSingular_QuandoDescricaoEstaNoPlural()
        {
            // "ações" -> "acao" só pelo stemmer: no texto sem stemmer ("acoes" x "acao") a diferença passa do limite do fuzzy.
            await IndexGamesAsync(Game("Steel Horizon", description: "Campanha cheia de ações em equipe."), Game("Byte Garden"));

            (await SearchNamesAsync("ação")).Should().Equal("Steel Horizon");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveEncontrarJogo_QuandoErroDeDigitacaoEstaNoPluralDoNome()
        {
            // "Pxels" está a 2 letras de "pixel": só encontra porque o stemmer tira o "s" (pxel) antes do fuzzy.
            await IndexGamesAsync(Game("Pixel Pals"), Game("Neon Rally"));

            (await SearchNamesAsync("Pxels")).Should().Equal("Pixel Pals");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveEncontrarJogo_PeloGenero()
        {
            await IndexGamesAsync(Game("Neon Rally", genre: GameGenre.Corrida), Game("Byte Garden", genre: GameGenre.Indie));

            (await SearchNamesAsync("corrida")).Should().Equal("Neon Rally");
        }

        [SkippableFact]
        public async Task SearchAsync_NaoDeveRetornarJogosDesativados()
        {
            await IndexGamesAsync(Game("Neon Rally"), Game("Neon Rally Classic", isActive: false));

            (await SearchNamesAsync("neon")).Should().Equal("Neon Rally");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveOrdenarPorRelevancia_ComNomeAntesDaDescricao()
        {
            // A mesma palavra exata nos dois jogos: só o peso do campo (name^3 x description) decide a ordem.
            await IndexGamesAsync(
                Game("Byte Garden", description: "Cuide do jardim do velho Dragon."),
                Game("Dragon Keep", description: "Defenda o castelo."));

            (await SearchNamesAsync("dragon")).Should().Equal("Dragon Keep", "Byte Garden");
        }

        [SkippableFact]
        public async Task SearchAsync_DeveRetornarListaVazia_QuandoNadaCorresponde()
        {
            await IndexGamesAsync(Game("Neon Rally"));

            var result = await _service.SearchAsync("zzzzzz", 1, 10, CancellationToken.None);

            result.Items.Should().BeEmpty();
            result.TotalItems.Should().Be(0);
        }

        [SkippableFact]
        public async Task SearchAsync_DevePaginar_EInformarOTotalDeResultados()
        {
            await IndexGamesAsync(Game("Neon Rally"), Game("Neon Drift"), Game("Neon Nights"), Game("Byte Garden"));

            var page1 = await _service.SearchAsync("neon", 1, 2, CancellationToken.None);
            var page2 = await _service.SearchAsync("neon", 2, 2, CancellationToken.None);

            page1.Items.Should().HaveCount(2);
            page2.Items.Should().HaveCount(1);
            page1.TotalItems.Should().Be(3);
            page1.Items.Select(i => i.Id).Should().NotIntersectWith(page2.Items.Select(i => i.Id));
        }

        [SkippableFact]
        public async Task SearchAsync_DeveDevolverOsDadosDoJogo_ComPrecoPromocionalEmVigor()
        {
            var game = Game("Neon Rally", genre: GameGenre.Corrida) with
            {
                CoverUrl = "/covers/neon-rally.svg",
                OriginalPrice = 49.9m,
                PromotionPrice = 29.9m,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                AverageRating = 4.5,
                RatingCount = 12
            };
            await IndexGamesAsync(game);

            var item = (await _service.SearchAsync("neon", 1, 10, CancellationToken.None)).Items.Single();

            item.Id.Should().Be(game.Id);
            item.Genre.Should().Be(GameGenre.Corrida);
            item.CoverUrl.Should().Be("/covers/neon-rally.svg");
            item.OriginalPrice.Should().Be(49.9m);
            item.CurrentPrice.Should().Be(29.9m);
            item.HasDiscount.Should().BeTrue();
            item.AverageRating.Should().Be(4.5);
            item.RatingCount.Should().Be(12);
        }

        [SkippableFact]
        public async Task SearchAsync_DeveLancarInvalidOperationException_QuandoIndiceNaoExiste()
        {
            await _client.Indices.DeleteAsync(GameIndex.Name);

            var act = () => _service.SearchAsync("neon", 1, 10, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion
    }
}
