namespace Fcg.Catalog.Infrastructure.Search
{
    /// <summary>Fonte dos documentos do índice: lê os jogos do banco já no formato do índice.</summary>
    internal interface IGameDocumentQuery
    {
        Task<GameDocument?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken);
        Task<IEnumerable<GameDocument>> GetAllAsync(CancellationToken cancellationToken);
    }
}
