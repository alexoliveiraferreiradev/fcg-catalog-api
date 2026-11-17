using Elastic.Clients.Elasticsearch;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Infrastructure.Search
{
    internal class ElasticGameService : IGameSearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly IGameDocumentQuery _gameDocumentQuery;
        private readonly ILogger<ElasticGameService> _logger;

        public ElasticGameService(ElasticsearchClient client, IGameDocumentQuery gameDocumentQuery, ILogger<ElasticGameService> logger)
        {
            _client = client;
            _gameDocumentQuery = gameDocumentQuery;
            _logger = logger;
        }

        public async Task IndexAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var document = await _gameDocumentQuery.GetByIdAsync(gameId, cancellationToken);
            if (document is null)
            {
                var deleted = await _client.DeleteAsync(new DeleteRequest(GameIndex.Name, gameId.ToString()), cancellationToken);
                if (!deleted.IsValidResponse && deleted.Result != Result.NotFound)
                    throw new InvalidOperationException($"Falha ao remover o jogo {gameId} do índice: {deleted.DebugInformation}");
                return;
            }

            
            var response = await _client.IndexAsync(document, i => i.Index(GameIndex.Name).Id(document.Id.ToString()), cancellationToken);
            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Falha ao indexar o jogo {gameId}: {response.DebugInformation}");
        }

        public async Task ReindexAllAsync(CancellationToken cancellationToken)
        {
            var documents = (await _gameDocumentQuery.GetAllAsync(cancellationToken)).ToList();
            if (documents.Count == 0) return;

            var response = await _client.BulkAsync(b => b.Index(GameIndex.Name).IndexMany(documents), cancellationToken);
            if (!response.IsValidResponse || response.Errors)
                throw new InvalidOperationException($"Falha na carga do índice {GameIndex.Name}: {response.DebugInformation}");

            _logger.LogInformation("[CatalogAPI] [Search] {Total} jogos carregados no índice {Index}.", documents.Count, GameIndex.Name);
        }

        public async Task<PagedResult<GameUserResponse>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken)
        {
            var response = await _client.SearchAsync<GameDocument>(s => s
                .Indices(GameIndex.Name)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q.Bool(b => b
                    .Must(m => m.MultiMatch(mm => mm
                        .Query(term)
                        .Fields(GameIndex.SearchFields)
                        .Fuzziness(new Fuzziness("AUTO"))   
                        .PrefixLength(1)))                  
                    .Filter(f => f.Term(t => t.Field(d => d.IsActive).Value(true))))),                
                cancellationToken);

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Falha na busca por '{term}': {response.DebugInformation}");

            var now = DateTime.UtcNow;
            var items = response.Documents.Select(d => ToResponse(d, now)).ToList();
            return new PagedResult<GameUserResponse>(items, page, pageSize, (int)response.Total);
        }

        private static GameUserResponse ToResponse(GameDocument document, DateTime utcNow) => new()
        {
            Id = document.Id,
            Name = document.Name,
            Description = document.Description,
            Genre = document.Genre,
            CoverUrl = document.CoverUrl,
            OriginalPrice = document.OriginalPrice,
            CurrentPrice = document.CurrentPriceAt(utcNow),
            AverageRating = document.AverageRating,
            RatingCount = document.RatingCount
        };
    }
}
