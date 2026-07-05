using Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface ILibraryQueryRepository
    {
        Task<PagedResult<BibliotecaItemResponse>> GetPagedLibraryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetPurchasedGamesByUser(Guid userId, CancellationToken cancellationToken);
    }
}
