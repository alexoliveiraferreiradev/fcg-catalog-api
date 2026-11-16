using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Elastic.Clients.Elasticsearch;
using Fcg.Catalog.Infrastructure.Search;

namespace Fcg.Catalog.Infrastructure.Tests.Search
{
    /// <summary>
    /// Um Elasticsearch de verdade em container, compartilhado pelos testes da coleção.
    /// Mesma versão major do client (9.x); segurança desligada só para o teste.
    /// Sem Docker, não sobe nada: os testes são pulados na máquina do dev e falham no CI (veja <see cref="RequireDocker"/>).
    /// </summary>
    public sealed class ElasticsearchFixture : IAsyncLifetime
    {
        private const string Image = "docker.elastic.co/elasticsearch/elasticsearch:9.1.0";
        private const int HttpPort = 9200;

        // Limite para a subida: sem ele, um container que não fica pronto trava a suíte para sempre.
        private static readonly TimeSpan StartupTimeout = TimeSpan.FromMinutes(3);

        public const string DockerUnavailableReason =
            "Docker não está em execução: testes com Elasticsearch ignorados. Inicie o Docker para rodá-los.";

        private IContainer? _container;

        /// <summary>
        /// O Testcontainers procura um Docker acessível (variável DOCKER_HOST, Docker Desktop, socket ou npipe)
        /// e deixa a configuração nula quando não encontra nenhum.
        /// </summary>
        public bool DockerIsAvailable { get; } = TestcontainersSettings.OS.DockerEndpointAuthConfig is not null;

        /// <summary>GitHub Actions (e GitLab, CircleCI...) definem CI=true nos runners.</summary>
        public static bool RunningInCi { get; } =
            string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);

        public ElasticsearchClient Client { get; private set; } = null!;

        /// <summary>
        /// Sem Docker: na máquina do dev o teste é pulado; no CI ele falha, para o pipeline não ficar verde
        /// sem ter testado a busca. Chamar no começo de cada teste.
        /// </summary>
        public void RequireDocker()
        {
            if (DockerIsAvailable) return;

            if (RunningInCi)
                throw new InvalidOperationException(
                    "Docker não está disponível no CI (CI=true): os testes com Elasticsearch precisam dele. Verifique o runner.");

            Skip.If(true, DockerUnavailableReason);
        }

        public async Task InitializeAsync()
        {
            if (!DockerIsAvailable) return;

            // Criado só aqui: com o Docker parado, o Build() já lançaria exceção.
            _container = new ContainerBuilder()
                .WithImage(Image)
                .WithPortBinding(HttpPort, assignRandomHostPort: true)
                .WithEnvironment("discovery.type", "single-node")
                .WithEnvironment("xpack.security.enabled", "false")
                .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
                // Sem query string: o ForPath escapa o "?" e a requisição nunca acertaria a URL.
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                    .ForPort(HttpPort)
                    .ForPath("/_cluster/health")))
                .Build();

            using var timeout = new CancellationTokenSource(StartupTimeout);
            await _container.StartAsync(timeout.Token);

            var uri = new Uri($"http://{_container.Hostname}:{_container.GetMappedPublicPort(HttpPort)}");
            Client = new ElasticsearchClient(GameIndex.MapDocument(new ElasticsearchClientSettings(uri)));
        }

        public Task DisposeAsync() => _container?.DisposeAsync().AsTask() ?? Task.CompletedTask;
    }

    [CollectionDefinition(Name)]
    public sealed class ElasticsearchCollection : ICollectionFixture<ElasticsearchFixture>
    {
        public const string Name = "Elasticsearch";
    }
}
