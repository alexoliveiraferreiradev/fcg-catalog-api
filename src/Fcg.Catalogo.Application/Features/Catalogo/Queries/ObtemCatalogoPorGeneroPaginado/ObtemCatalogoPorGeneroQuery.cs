using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoPorGeneroPaginado
{
    public record ObtemCatalogoPorGeneroQuery(
        int Pagina = 1,
        int TamanhoPagina = 10,
        GeneroJogo? Genero = null) : IRequest<PagedResult<JogoResponse>>;
}
