using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Enum;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoJogosPromovidosPaginado
{
    public record ObtemCatalogoJogosPromovidosQuery(
        int Pagina = 1,
        int TamanhoPagina = 10,
        GeneroJogo? Genero = null,
        bool? ApenasPromovidos = null) : IRequest<PagedResult<JogoResponse>>;
}
