using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalog
{
    public record ObtemCatalogPaginadosQuery(
        int Pagina = 1,
        int TamanhoPagina = 10,
        GameGenre? Genre = null,
        bool? ApenasPromovidos = null) : IRequest<PagedResult<JogoResponse>>;
}
