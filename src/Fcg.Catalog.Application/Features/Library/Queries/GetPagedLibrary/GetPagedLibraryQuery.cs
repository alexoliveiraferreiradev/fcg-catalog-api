using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary
{
    public record GetPagedLibraryQuery(Guid UserId, int Page =1, int TamanhoPagina = 10) : IRequest<PagedResult<BibliotecaItemResponse>>;
}
