using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada
{
    public record ObtemBibliotecaPaginadaQuery(Guid UsuarioId, int Pagina =1, int TamanhoPagina = 10) : IRequest<PagedResult<BibliotecaItemResponse>>;
}
