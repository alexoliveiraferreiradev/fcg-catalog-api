namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>
    /// Conexão com o Elasticsearch. Em nuvem (Elastic Cloud) use CloudId + ApiKey;
    /// local (docker) use Url, com ApiKey opcional.
    /// </summary>
    public class ElasticsearchSettings
    {
        public const string SectionName = "ElasticSearchSettings";

        public string? CloudId { get; init; }
        public string? Url { get; init; }
        public string? ApiKey { get; init; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(CloudId) ? !string.IsNullOrWhiteSpace(ApiKey) : Uri.TryCreate(Url, UriKind.Absolute, out _);
    }
}
