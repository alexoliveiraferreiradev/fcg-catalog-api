using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterPromocaoPorJogoId
{
    public record ObterPromocaoPorJogoIdQuery(Guid promocaoId) :IRequest<PromocaoResponse>;
}
