using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Library.Queries.ObtemBibliotecaPaginada
{
    public record ObtemBibliotecaPaginadaQuery(Guid UserId, int Pagina =1, int TamanhoPagina = 10) : IRequest<PagedResult<BibliotecaItemResponse>>;
}
