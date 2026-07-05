using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface IGameQueryRepository
    {
        Task<PagedResult<GameUserResponse>> GetPagedCatalogAsync(GameGenre? gameGenre,bool? OnlyPromoted ,int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<GameResponse?> GetGameByIdAsync(Guid gameId, CancellationToken cancellationToken);
        Task<IEnumerable<GameResponse>> GetAllGamesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<GameResponse>> GetGamesByIdsAsync(IEnumerable<Guid> jogosIds, CancellationToken cancellationToken);
    }
}
