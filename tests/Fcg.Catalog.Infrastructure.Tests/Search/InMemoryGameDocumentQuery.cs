using Fcg.Catalog.Infrastructure.Search;

namespace Fcg.Catalog.Infrastructure.Tests.Search
{
    /// <summary>Banco falso: o serviço lê daqui o que leria do SQL.</summary>
    internal sealed class InMemoryGameDocumentQuery : IGameDocumentQuery
    {
        private readonly Dictionary<Guid, GameDocument> _games = new();

        public void Save(GameDocument game) => _games[game.Id] = game;

        public void Remove(Guid gameId) => _games.Remove(gameId);

        public Task<GameDocument?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken) =>
            Task.FromResult(_games.GetValueOrDefault(gameId));

        public Task<IEnumerable<GameDocument>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IEnumerable<GameDocument>>(_games.Values.ToList());
    }
}
