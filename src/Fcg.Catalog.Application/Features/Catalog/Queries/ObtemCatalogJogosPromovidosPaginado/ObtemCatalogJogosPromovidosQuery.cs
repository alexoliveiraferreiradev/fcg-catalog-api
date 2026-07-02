using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogJogosPromovidosPaginado
{
    public record ObtemCatalogJogosPromovidosQuery(
        int Pagina = 1,
        int TamanhoPagina = 10,
        GeneroJogo? Genero = null,
        bool? ApenasPromovidos = null) : IRequest<PagedResult<JogoResponse>>;
}
